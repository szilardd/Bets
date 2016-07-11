using Bets.Data;
using Bets.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Bets.Helpers
{
    public class AddMatchesHelper
    {
        private static string _FullTimeCode = "FT";
        private static string _FutureGameResult = "?";

        /// <summary>
        /// Needs a path for the XML (source of the matches) and a roundID (to which round to insert the new matches)
        /// </summary>
        public bool AddMatchesToRound(string m_strFilePath, int roundID)
        {
            TeamRepository teamRepo = new TeamRepository();
            List<MatchModel> Matches = new List<MatchModel>();
            string xmlStr;

            //Opens the XML
            using (var wc = new WebClient())
            {
                xmlStr = wc.DownloadString(m_strFilePath);
            }
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlStr);

            //Gets all Matches From the XML by variables (these have to be changed if other XML will be used)
            XmlNodeList allMatches = xmlDoc.DocumentElement.SelectNodes("Competition[@compno='1585'][@id='SOC']/Match");

            //Loop through all the matches and build MatchModels
            foreach (XmlNode singleMatch in allMatches)
            {
                //Declare variable for each field that has to be included for easier debugging
                var TeamsSplit = singleMatch.SelectSingleNode("Name").InnerText.Split(new string[] { " v " }, StringSplitOptions.None);
                string FirstTeam = TeamsSplit[0].Trim();
                string SecondTeam = TeamsSplit[1].Trim();
                int FirstID = teamRepo.GetTeamByName(FirstTeam).TeamID;
                int SecondID = teamRepo.GetTeamByName(SecondTeam).TeamID;
                double Points1 = Convert.ToDouble(singleMatch.SelectSingleNode("Bet[@type='SM']/line[@name='odds1']").InnerText) * 100;
                double PointsX = Convert.ToDouble(singleMatch.SelectSingleNode("Bet[@type='SM']/line[@name='oddsdraw']").InnerText) * 100;
                double Points2 = Convert.ToDouble(singleMatch.SelectSingleNode("Bet[@type='SM']/line[@name='odds2']").InnerText) * 100;
                DateTime Date = DateTime.ParseExact(singleMatch.SelectSingleNode("Date").InnerText, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                DateTime OurDate = Date.AddHours(7);
                //Add the Match
                Matches.Add(new MatchModel()
                {
                    FirstTeamID = FirstID,
                    SecondTeamID = SecondID,
                    FirstTeamName = FirstTeam,
                    SecondTeamName = SecondTeam,
                    Date = OurDate,
                    Points1 = Convert.ToInt32(Points1),
                    PointsX = Convert.ToInt32(PointsX),
                    Points2 = Convert.ToInt32(Points2),
                    RoundID = roundID
                });
            }
            return (new MatchRepository().AddMatches(Matches));
        }

        /// <summary>
        /// Will parse livescore.com and get the result for the matches. Then it'll compare the team names to the one we have in the Matches table. When there's a match it'll insert the result. 
        /// </summary>
        public List<MatchModel> GetMatchResultsHelper()
        {
            //Get all the matches for the current round
            List<MatchModel> matches = new MatchRepository().GetMatchesForToday().ToList();

            //If there are no matches, do not go further
            if (matches.Count == 0)
            {
                Trace.TraceInformation("No matches take place today");
                return new List<MatchModel>();
            }

            var data = GetData();
            
            List<MatchModel> matchesWithResults = new List<MatchModel>();

            //Clean the html
            data = data.Replace("N.Ireland", "Northern Ireland");
            var matchesFromHtml = GetMatchResultsFromHtml(data);
            
            Trace.TraceInformation("Result from LiveScore");
            Trace.TraceInformation(matchesFromHtml.ToJSON());

            //Loop through the matches and search for them within the clean html 
            foreach (var match in matches)
            {
                var logPrefix = $"Match ID {match.ID} ({match.FirstTeamName} - {match.SecondTeamName})";

                if (match.FirstTeamGoals == null && match.SecondTeamGoals == null)
                {
                    Trace.TraceInformation($"{logPrefix} - Determining result - match date {match.Date}");

                    var matchResult = matchesFromHtml.FirstOrDefault(e => e.FirstTeamName == match.FirstTeamName && e.SecondTeamName == match.SecondTeamName);

                    if (matchResult != null)
                    {
                        var minuteOrResult = matchResult.MinuteOrResult;

                        // only add result if the match has ended
                        if (minuteOrResult == _FullTimeCode)
                        {
                            match.FirstTeamGoals = matchResult.FirstTeamGoals;
                            match.SecondTeamGoals = matchResult.SecondTeamGoals;

                            Trace.TraceInformation($"{logPrefix} - Found result - {match.FirstTeamGoals}:{match.SecondTeamGoals}");

                            matchesWithResults.Add(match);
                        }
                        else
                        {
                            Trace.TraceInformation($"{logPrefix} - Match hasn't ended yet - {minuteOrResult}");
                        }
                    }
                    else
                    {
                        Trace.TraceInformation($"{logPrefix} - No result found");
                    }
                }
                else
                {
                    Trace.TraceInformation($"{logPrefix} - Ignoring because already has result - {match.FirstTeamGoals}:{match.SecondTeamGoals}");
                }         
            }

            return matchesWithResults;
        }

        private static string GetData()
        {
            //URL from where we get the results
            string urlAddress = "http://www.livescore.com/soccer/";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            string data = "";
            
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream receiveStream = response.GetResponseStream())
                    {
                        StreamReader readStream = null;

                        if (response.CharacterSet == null)
                        {
                            readStream = new StreamReader(receiveStream);
                        }
                        else
                        {
                            readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                        }

                        data = readStream.ReadToEnd();

                        readStream.Dispose();
                    }
                }
            }

            return data;
        }

        /// <summary>
        ///  Sample HTML for a finished match
        ///
        ///     <div class="min"> FT </div> 
        ///     <div class="ply tright name"> Czech Republic </div> 
        ///     <div class="sco"> <a href="/euro/match/?match=1-1695441">2 - 2</a> </div> 
        ///     <div class="ply name"> Croatia</div>
        ///
        ///
        ///  Sample HTML for an ongoing match
        ///
        ///     <div class="min"><img src="http://cdn3.livescore.com/web/img/flash.gif" alt="live"> 1' </div> 
        ///     <div class="ply tright name"> Yanbian </div> 
        ///     <div class="sco"> 0 - 0 </div> 
        ///     <div class="ply name"> Guangzhou Evergrande </div> 
        /// </summary>
        private static List<MatchResultModel> GetMatchResultsFromHtml(string html)
        {
            var pattern =

            "<div class=\"min\">(<img.*?>)? *(?<Result>.*?) *<\\/div>" +                        // result or minute (may contain <img> tag or other HTML elements)
            ".*?name\"> *(?<FirstTeamName>.*?) *<\\/div>" +                                     // first team name
            ".*?" +                                                                             // junk
            "class=\"sco\">.*?(?<FirstTeamGoals>[0-9]|\\?) - (?<SecondTeamGoals>[0-9]|\\?)" +   // score
            ".*?" +                                                                             // junk
            "name\"> ?(?<SecondTeamName>.*?)<\\/div>";                                          // second team name

            var regex = new Regex(pattern);
            var results = new List<MatchResultModel>();

            foreach (System.Text.RegularExpressions.Match matchResult in regex.Matches(html))
            {
                var firstTeamGoals = matchResult.Groups["FirstTeamGoals"].Value.Trim();

                // if goal number is ?, the match hasn't started yet
                // these matches need to be identified by the regex as further matches may have ended
                if (firstTeamGoals != _FutureGameResult)
                {
                    results.Add(new MatchResultModel
                    {
                        MinuteOrResult = matchResult.Groups["Result"].Value.Trim(),
                        FirstTeamName = matchResult.Groups["FirstTeamName"].Value.Trim(),
                        SecondTeamName = matchResult.Groups["SecondTeamName"].Value.Trim(),
                        FirstTeamGoals = Convert.ToInt32(firstTeamGoals.Trim()),
                        SecondTeamGoals = Convert.ToInt32(matchResult.Groups["SecondTeamGoals"].Value.Trim())
                    });
                }
            }

            return results;
        }
    }
}