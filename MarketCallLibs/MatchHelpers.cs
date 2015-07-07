using DuoVia.FuzzyStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarketCallLibs
{
    public class MatchHelpers
    {
        public static string Match(string test, IEnumerable<string> set)
        {
            var closest = set.First();
            var closestValue = int.MaxValue;

            foreach (var item in set) {

                var cleanedItem = Regex.Replace(item, @"\([^)]+\)", "").Trim();

                var value = LevenshteinDistanceExtensions.LevenshteinDistance(cleanedItem, test);
                if (value < closestValue) {
                    closest = item;
                    closestValue = value;
                }
            }

            return closest;
        }

        public static Dictionary<Guest, IEnumerable<ICsvOutputtable>> GetOpinionHistory(IEnumerable<Guest> guests, IEnumerable<Opinion> allOpinions)
        {
            var result = new Dictionary<Guest, IEnumerable<ICsvOutputtable>>();

            foreach (var guest in guests)
            {
                var match = Match(guest.Name, allOpinions.Select(o => o.Expert).Distinct());

                var resultSet = allOpinions.Where(o => o.Expert == match);
                //resultSet = resultSet.Where(o => (o.Signal.ToLowerInvariant().StartsWith("top pick") 
                //            //|| o.Signal.ToLowerInvariant().Contains("strong buy")
                //            ));

                //resultSet = resultSet.OrderByDescending(o => o.Date)
                //            .Take(20);


                result[guest] = new HashSet<Opinion>(
                    resultSet
                            );
            }

            return result;
        }
    }
}
