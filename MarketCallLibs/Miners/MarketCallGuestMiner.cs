using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MarketCallLibs
{
    public class MarketCallGuestMiner : IGuestMiner
    {
        protected string _format = "http://www.bnn.ca/WebServices/Cache/AjaxServices.svc/GetNewMarketCallUpcomingGuests?showID=280&numDays=8";

        public IEnumerable<Guest> GetGuests()
        {
            WebClient wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.Host, "www.bnn.ca");

            var result = wc.DownloadString(string.Format(_format));

            var json = JArray.Parse(HttpUtility.HtmlDecode(result));

            var guests = ParseGuests(json);

            return guests;
        }

        private IEnumerable<Guest> ParseGuests(JArray json)
        {
            var guests = new List<Guest>();

            foreach (var guest in json)
            {
                var name = guest["GuestName"].ToString().Trim();
                var dateStr = guest["Date"].ToString().Trim();
                var date = DateTime.Parse(dateStr);

                guests.Add(new Guest(name, date));
            }

            return guests;
        }

        public override string ToString()
        {
            return "MarketCall";
        }
    }
}
