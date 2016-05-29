using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bets.Data.Models
{
	public class GoalscorerModel : PlayerModel
	{
		public string TeamFlag { get; set; }
		public int GoalsScored { get; set; }
		public string ExternalID { get; set; }
		public bool BetMade { get; set; }
		public bool BetExpired { get; set; }
		public int Points { get; set; }
		public bool OnlySelected { get; set; }

        private static string GetShortName(string name, bool force)
        {
            string shortName = "";

            if (name.Length >= 14 && name.Contains(' '))
            {
                string[] nameParts = name.Split(new char[] { ' ' });

                //add initials
                for (int i = 0; i < nameParts.Length - 1; i += 1)
                {
                    //if name is 3 chars or less, add it
                    if (!force && nameParts[i].Length <= 3)
                    {
                        shortName += nameParts[i];
                    }
                    //otherwise add only initial
                    else
                    {
                        // if last name is long, do not add initial
                        if (nameParts[nameParts.Length - 1].Length < 10)
                        {
                            shortName += nameParts[i][0] + ".";
                        }
                    }
                }

                //add last name
                shortName += " " + nameParts[nameParts.Length - 1];
            }
            else
            {
                shortName = name;
            }

            return shortName;
        }

        /// <summary>
        /// If name is long, returns it as J. Doe (from John Doe)
        /// </summary>
        public string ShortName
        {
            get
            {
                var shortName = GetShortName(Name, false);

                if (shortName.Length >= 14)
                    shortName = GetShortName(Name, true);

                return shortName;
            }
        }
    }
}
