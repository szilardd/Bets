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

namespace Bets.Helpers
{
    public class AddMatchesHelper
    {
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
            //URL from where we get the results
            string urlAddress = "http://www.livescore.com/soccer/";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string data = "";
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
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

                response.Close();
                readStream.Close();
            }
            //Get all the matches for the current round
            IQueryable<MatchModel> Matches = new MatchRepository().GetMatchesForCurrentRound();
            List<MatchModel> MatchesWithResults = new List<MatchModel>();

            //Clean the html
            string Results = UnHtml(data);
            Results = Results.Replace("N.Ireland", "Northern Ireland");
            //Loop through the matches and search for them within the clean html 
            foreach (var Match in Matches)
            {
                if(Match.FirstTeamGoals != null && Match.SecondTeamGoals != null)
                {
                    string FirstTeam = Match.FirstTeamName;
                    string SecondTeam = Match.SecondTeamName;

                    int pFrom = Results.IndexOf(FirstTeam) + FirstTeam.Length;
                    int pTo = Results.LastIndexOf(SecondTeam);

                    //Only add the result if it finds both teams in relative close distance from each other, split the string between them to get both teams score
                    if (pTo - pFrom > 0 && Results.IndexOf(FirstTeam) >= 0 && Results.LastIndexOf(SecondTeam) > 0 && pTo - pFrom < 10)
                    {
                        String result = Results.Substring(pFrom, pTo - pFrom);
                        Match.FirstTeamGoals = Convert.ToInt32(result.Split('-')[0].Trim());
                        Match.SecondTeamGoals = Convert.ToInt32(result.Split('-')[1].Trim());
                        MatchesWithResults.Add(Match);
                    }
                }            
            }
            return MatchesWithResults;
        }

        public static String UnHtml(String html)
        {
            html = HttpUtility.UrlDecode(html);
            html = HttpUtility.HtmlDecode(html);

            html = RemoveTag(html, "<!--", "-->");
            html = RemoveTag(html, "<script", "</script>");
            html = RemoveTag(html, "<style", "</style>");
            html = RemoveTag(html, "<span", "</span>");
            html = RemoveTag(html, "<select", "</select>");
            html = RemoveTag(html, "<link", "/>");
            html = RemoveTag(html, "<meta", "/>");
            html = RemoveTag(html, "<title>", "</title>");
            html = RemoveTag(html, "<img", "/>");
            html = RemoveTag(html, "</div> <div class=\"sco\">", "\">");
            html = RemoveTag(html, "<a href", "\">");
            html = RemoveTag(html, "</a>", "name\">");
            html = RemoveTag(html, "</div> <div class=\"star hidden\" data-type=\"star\"><i class=\"ico ico-star\"></i></div> </div> <div class=\"row-gray even\"", "class=\"ply tright name\">");
            html = RemoveTag(html, "</div> <div class=\"star hidden\" data-type=\"star\"><i class=\"ico ico-star\"></i></div> </div> <div class=\"row-gray", "class=\"ply tright name\">");
            html = RemoveTag(html, "</div> <div class=\"star hidden\" data-type=\"star\"><i class=\"ico ico-star\"></i></div>", "</strong>");
            html = RemoveTag(html, "<!DOCTYPE html>", "<head>");
            html = RemoveTag(html, "</div>", "</html>");

            html = SingleSpacedTrim(html);

            return html;
        }

        private static String RemoveTag(String html, String startTag, String endTag)
        {
            Boolean bAgain;
            do
            {
                bAgain = false;
                Int32 startTagPos = html.IndexOf(startTag, 0, StringComparison.CurrentCultureIgnoreCase);
                if (startTagPos < 0)
                    continue;
                Int32 endTagPos = html.IndexOf(endTag, startTagPos + 1, StringComparison.CurrentCultureIgnoreCase);
                if (endTagPos <= startTagPos)
                    continue;
                html = html.Remove(startTagPos, endTagPos - startTagPos + endTag.Length);
                bAgain = true;
            } while (bAgain);
            return html;
        }

        private static String SingleSpacedTrim(String inString)
        {
            StringBuilder sb = new StringBuilder();
            Boolean inBlanks = false;
            foreach (Char c in inString)
            {
                switch (c)
                {
                    case '\r':
                    case '\n':
                    case '\t':
                    case ' ':
                        if (!inBlanks)
                        {
                            inBlanks = true;
                            sb.Append(' ');
                        }
                        continue;
                    default:
                        inBlanks = false;
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString().Trim();
        }
    }
}