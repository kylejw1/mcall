using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace MarketCallLibs
{
    public class Quandl
    {
        private string GetCsv(string symbol)
        {
            const string format = "https://www.quandl.com/api/v1/datasets/WIKI/{0}.csv?auth_token=RCGV6u7H8oxw58FW9Esk";

            try {
                symbol = Regex.Match(symbol, @"\w*"/*"\w+\.*\w*"*/).Value.ToString().Trim();
            } catch {}

            WebClient wc = new WebClient();
            return wc.DownloadString(string.Format(format, symbol));
        }

        //https://www.quandl.com/api/v1/datasets/WIKI/AAPL.csv?auth_token=RCGV6u7H8oxw58FW9Esk
        public StockStat GetStats(string symbol, IEnumerable<DateTime> dates)
        {
            var csv = GetCsv(symbol);

            dates = dates.Select(d => new DateTime(d.Year, d.Month, d.Day));

            var rows = csv.Split(new char[] { '\n', '\r' }).ToList();

            var headers = rows.First().Split(new char[]{','}).Select(h => h.Trim().ToLowerInvariant()).ToList();
            int dateIndex = headers.IndexOf("date");
            int closeIndex = headers.IndexOf("close");
            int volumeIndex = headers.IndexOf("volume");

            rows.RemoveAt(0);

            foreach (var row in rows)
            {
                var cols = row.Split(new char[] { ',' });

                var date = DateTime.Parse(cols[dateIndex]);

                if (!dates.Contains(date))
                    continue;

                var close = double.Parse(cols[closeIndex]);
                //var volume = double.Parse(cols[closeIndex]);
            }



            return null;
        }
    }
}
