using MarketCallLibs.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MarketCallLibs
{
    public class Opinion : ICsvOutputtable
    {
        public readonly DateTime Date;
        public readonly string Signal;
        public readonly string Company;
        public readonly string Expert;
        public readonly string OpinionString;
        public readonly decimal Price;
        public readonly string Symbol;

        public override bool Equals(object obj)
        {
            return this.GetHashCode().Equals(obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            int hash = Date.GetHashCode() ^ Signal.GetHashCode();
            hash = hash ^ Company.GetHashCode();
            hash = hash ^ Expert.GetHashCode();
            hash = hash ^ OpinionString.GetHashCode();
            hash = hash ^ Price.GetHashCode();
            hash = hash ^ Symbol.GetHashCode();

            return hash;
        }

        public Opinion(DateTime date, string signal, string company, string expert, string opinionString, decimal price, string symbol)
        {
            Date = date;
            Signal = signal;
            Company = company;

            Expert = NameHelpers.Normalize(expert);
            OpinionString = opinionString;
            Price = price;
            Symbol = symbol;
        }

        public override string ToString()
        {
            return Expert;
            //return String.Format("[{0}] {3} {1} {2}", Date.ToShortDateString(), Expert, Price, Symbol);
        }

        public static void Save(List<Opinion> opinions, string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Opinion>));
            TextWriter tw = new StreamWriter(path);
            xs.Serialize(tw, opinions);
        }

        public static List<Opinion> Load(string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Opinion>));
            using (var sr = new StreamReader(path))
            {
                var loaded = (List<Opinion>)xs.Deserialize(sr);

                return loaded;
            }
        }

        private static string RemoveControlCharacters(string inString)
        {
            if (inString == null) return null;
            StringBuilder newString = new StringBuilder();
            char ch;
            for (int i = 0; i < inString.Length; i++)
            {
                ch = inString[i];
                if (!char.IsControl(ch))
                {
                    newString.Append(ch);
                }
            }
            return newString.ToString();
        }

        public string ToCsv()
        {
            var objs = new List<Object>();

            objs.Add(Date.ToShortDateString());
            objs.Add(Symbol);
            //objs.Add(Expert);
            objs.Add(Signal);
            objs.Add(Price);
            objs.Add(Company);
            objs.Add(OpinionString);

            return string.Join(",", objs.Select(o => RemoveControlCharacters(o.ToString().Replace(",", ";"))));
        }


        public string GetHeaderCsv()
        {
            return "date,symbol,signal,price,company,opinion";
            //return "date,symbol,expert,signal,price,company,opinion";
        }
    }
}
