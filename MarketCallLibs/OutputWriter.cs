using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketCallLibs
{
    public class OutputWriter
    {
        public static void WriteAll(string path, Dictionary<Guest, IEnumerable<ICsvOutputtable>> data)
        {
            foreach(var kvp in data) {

                var fileName = kvp.Key.Name + ".txt";

                if (!kvp.Key.Name.Trim().Equals(kvp.Value.First().ToString().Trim(), StringComparison.InvariantCultureIgnoreCase))
                    fileName = "CHECK_NAME " + kvp.Value.First().ToString().Trim() + " VS " + fileName;

                using (var tw = File.CreateText(path + "/" + fileName))
                {
                    tw.WriteLine(kvp.Value.First().GetHeaderCsv());

                    foreach (var row in kvp.Value.Select(r => r.ToCsv()))
                    {
                        tw.WriteLine(row);
                    }
                }
            }
        }
    }
}
