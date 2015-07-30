using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarketCallLibs
{
    public class MarketCallTonightGuestMiner : MarketCallGuestMiner
    {
        public MarketCallTonightGuestMiner() : base()
        {
            this._format = "http://www.bnn.ca/WebServices/Cache/AjaxServices.svc/GetNewMarketCallUpcomingGuests?showID=315&numDays=7";
        }

        public override string ToString()
        {
            return "MarketCallTonight";
        }
    }
}
