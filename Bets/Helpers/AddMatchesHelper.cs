using Bets.Data;
using Bets.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
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

                //Add the Match
                Matches.Add(new MatchModel()
                {
                    FirstTeamID = FirstID,
                    SecondTeamID = SecondID,
                    FirstTeamName = FirstTeam,
                    SecondTeamName = SecondTeam,
                    Date = DateTime.ParseExact(singleMatch.SelectSingleNode("Date").InnerText, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                    Points1 = Convert.ToInt32(Points1),
                    PointsX = Convert.ToInt32(PointsX),
                    Points2 = Convert.ToInt32(Points2),
                    RoundID = roundID
                });
            }
            return (new MatchRepository().AddMatches(Matches));
        }
    }
}