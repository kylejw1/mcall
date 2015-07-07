using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarketCallLibs
{
    public class StockStat
    {
        public string _symbol = "";
        public string _company = "";
        public Dictionary<DateTime, double> _changePct = new Dictionary<DateTime,double>();

    }
}
