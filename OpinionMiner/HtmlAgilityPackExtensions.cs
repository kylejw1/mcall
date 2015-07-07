using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OpinionMiner
{
    public static class HtmlAgilityPackExtensions
    {
        public static string DecodedInnerText(this HtmlNode node) {
            try
            {
                return HttpUtility.HtmlDecode(node.InnerText).Trim();
            }
            catch
            {
                return "";
            }
        }
    }
}
