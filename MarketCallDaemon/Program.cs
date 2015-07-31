using MarketCallLibs.DataAccess;
using OpinionMiner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketCallDaemon
{
    class Program
    {
        private static readonly int DELAY_HOURS = 12;
        private static readonly SQLiteDao Dao = new SQLiteDao();

        static void Main(string[] args)
        {
            var delay = TimeSpan.FromHours(DELAY_HOURS);

            while(true)
            {
                try
                {
                    Output("Beginning Update Cycle.");
                    UpdateDb();
                }
                catch (Exception ex)
                {
                    Output(ex.ToString());
                }
                finally
                {
                    Output("Cycle complete.  Sleeping for {0} hours", delay.Hours.ToString());
                    System.Threading.Thread.Sleep(delay);
                }
            }
        }

        private static void UpdateDb()
        {
            var stopDate = Dao.GetLatestOpinionDate();
            Output("Last discovered opinion was {0}", stopDate.ToString());

            var miner = new StockChaseOpinionMiner();

            var opinions = miner.GetOpinions(stopDate).ToList();
            Output("Found {0} new opinions.  Inserting.", opinions.Count.ToString());

            Dao.Insert(opinions);
        }

        private static void Output(string format, params string[] parameters)
        {
            var timeStamp = string.Format("[{0}]> ", DateTime.Now.ToString());
            Console.Out.WriteLine(timeStamp + format, parameters);
        }
    }
}
