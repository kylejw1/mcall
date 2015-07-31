using DuoVia.FuzzyStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MarketCallLibs.Helpers
{
    public class NameHelpers
    {
        public static string Normalize(string name)
        {
            name = Regex.Replace(name, @"[ ]{2,}", " ");
            name = Regex.Replace(name, @"[^a-zA-Z ]", "").ToLower().Trim();

            return name;
        }

        public static IEnumerable<Opinion> RemoveWeakNameMatches(IEnumerable<Opinion> opinions, string expectedName)
        {
            Dictionary<string, int> ScoreCache = new Dictionary<string, int>();

            foreach(var name in opinions.Select(o => o.Expert))
            {
                if (!ScoreCache.ContainsKey(name))
                {
                    ScoreCache[name] = LevenshteinDistanceExtensions.LevenshteinDistance(name, expectedName, false);
                }
            }

            var min = ScoreCache.Values.Min();

            var bestMatch = ScoreCache.First(s => s.Value == min).Key;

            var opinionList = opinions.Where(o => o.Expert.Equals(bestMatch));

            return opinionList;
        }
    }
}
