using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpinionMiner;
using MarketCallLibs;
using System.IO;

namespace MarketCall
{
    class Program
    {
        static void Main(string[] args)
        {
            var q = new Quandl();

            var dates = new List<DateTime>();
            for (int i = 0; i < 5; i++) {
                 dates.Add(DateTime.Now - TimeSpan.FromDays(100) + TimeSpan.FromDays(i));
            }

            var stats = q.GetStats("AAPL.Q-T", dates);

            var guestMiners = new List<IGuestMiner>()
            {
                new MarketCallGuestMiner(),
                new MarketCallTonightGuestMiner()
            };

            var guests = new List<Guest>();
            foreach (var guestMiner in guestMiners)
            {
                Console.Out.WriteLine("Requesting guests from {0}...", guestMiner.ToString());

                var g = guestMiner.GetGuests().ToList();
                if (null == g || g.Count == 0)
                {
                    Console.Out.WriteLine("Unable to obtain guest list.");
                    continue;
                }

                guests.AddRange(g);
            }

            guests = guests.Distinct().ToList();

            Console.Out.WriteLine("Found {0} guests: {1}", guests.Count, string.Join(", ", guests.Select(g => g.Name)));

            if (guests.Count == 0)
            {
                Console.Out.WriteLine("Cannot continue without guest list");
                Console.ReadLine();
                return;
            }

            var miner = new StockChaseMiner();

            Console.Out.WriteLine("Opening opinion storage...");
            var opinionStore = new OpinionStore();
            Console.Out.WriteLine("Opinion store contains {0} opinions. Last updated {1}.  Updating...", opinionStore.Opinions.Count, opinionStore.LastUpdate.ToShortDateString());
            var newOpinionCount = opinionStore.Update(miner);
            Console.Out.WriteLine("Found {0} new opinions.", newOpinionCount);

            Console.Out.WriteLine("Matching...");
            var result = MatchHelpers.GetOpinionHistory(guests, opinionStore.Opinions);

            foreach (var r in result)
            {
                Console.Out.WriteLine("{0} => {1}", r.Key, r.Value.First().ToString());
            }

            var now = DateTime.Now;
            int index = 2;
            var directoryBase = "output/" + now.ToString("dd_MM_yyyy");
            string directory = directoryBase;
            while (Directory.Exists(directory))
            {
                directory = directoryBase + " (" + index++ + ")";
            }

            Directory.CreateDirectory(directory);

            Console.Out.WriteLine("Writing results to {0}", directory);

            OutputWriter.WriteAll(directory, result);

            Console.Out.WriteLine(Environment.NewLine + "Complete.  Press any key to exit");

            Console.ReadLine();
        }
    }
}
