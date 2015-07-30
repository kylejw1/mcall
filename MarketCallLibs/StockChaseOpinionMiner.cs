using HtmlAgilityPack;
using MarketCallLibs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace OpinionMiner
{
    public class StockChaseOpinionMiner : IOpinionMiner
    {
        private string _format = "http://www.stockchase.com/opinions/recent/sort/date/page/{0}/direction/desc/max/120";
        private int _failures = 0;

        private HtmlDocument RequestPage(int index)
        {
            try
            {

                WebClient wc = new WebClient();
                wc.Headers.Add(HttpRequestHeader.Host, "www.stockchase.com");
                wc.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

                string result = wc.DownloadString(string.Format(_format, index));

                var doc = new HtmlDocument();
                doc.LoadHtml(result);
                //doc.Load("saved/" + index + ".html");

                //doc.Save("test.html");

                return doc;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private decimal PriceToDecimal(string price)
        {
            var match = Regex.Match(price, @"(\$[0-9,]+(\.[0-9]{2})?)");
            var value = match.Value;
            value = value.Replace("$", "");

            return Decimal.Parse(value);
        }

        private IEnumerable<Opinion> ParseOpinions(HtmlDocument doc)
        {
            var opinions = new List<Opinion>();
            if (null == doc)
                return opinions;

            var rows = doc.DocumentNode.SelectNodes("//div[@id='opinions']//tr");

            // remove header
            rows.RemoveAt(0);

            foreach (var row in rows)
            {
                try
                {
                    var date = row.SelectSingleNode(".//*[@class='date']").DecodedInnerText();
                    var signal = row.SelectSingleNode(".//*[contains(@class,'OpinionSignal')]").DecodedInnerText();
                    var company = row.SelectSingleNode(".//*[contains(@class,'company')]").DecodedInnerText();
                    var companyHtml = row.SelectSingleNode(".//*[contains(@class,'company')]").InnerHtml;
                    var expert = row.SelectSingleNode(".//*[contains(@class,'expert')]").DecodedInnerText();
                    var opinionString = row.SelectSingleNode(".//*[contains(@class,'opinion')]").DecodedInnerText();
                    var price = row.SelectSingleNode(".//*[contains(@class,'price')]").DecodedInnerText();

                    var dateTime = DateTime.Parse(date);
                    var priceValue = PriceToDecimal(price);
                    string symbol = "";
                    try {
                        symbol = Regex.Match(companyHtml, "<br>(?<sym>[^<]*)</a>").Groups["sym"].Value.Trim();
                    } catch {}

                    var opinion = new Opinion
                    (
                        dateTime,
                        signal,
                        company,
                        expert,
                        opinionString,
                        priceValue,
                        symbol                       
                    );
                    opinions.Add(opinion);
                }
                catch(Exception ex)
                {
                    _failures++;
                }
            }

            return opinions;
        }

        public IEnumerable<Opinion> GetOpinions(DateTime stop, int max)
        {
            _failures = 0;
            var allOpinions = new ConcurrentBag<Opinion>();

            Parallel.For(1, 2000, new ParallelOptions { MaxDegreeOfParallelism = 20 }, i =>
               {
                   if (System.IO.File.Exists("saved/" + i + ".html"))
                       return;

                   var page = RequestPage(i);
                   var opinions = ParseOpinions(page);
                   if (null == opinions || !opinions.Any())
                       return;

                   foreach (var o in opinions)
                   {
                       allOpinions.Add(o);
                   }
                   page.Save("saved/" + i + ".html");

                   System.Threading.Thread.Sleep(20);
               });

            //int emptyCount = 0;
            //for (int index = 1; index < max; index++)
            //{
            //    var page = RequestPage(index);
            //    var opinions = ParseOpinions(page);

            //    allOpinions.AddRange(opinions);
            //    //TODO: Check for duplicates
            //    if (opinions.Any(o => o.Date < stop) )
            //        break;

            //    if (!opinions.Any())
            //        emptyCount++;

            //    if (emptyCount > 10)
            //        break;
            //}

            return allOpinions;
        }
    }
}
