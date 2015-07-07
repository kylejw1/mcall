using MarketCallLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpinionMiner
{
    public class OpinionStore
    {
        private DateTime _lastUpdate = new DateTime();
        private string _path;

        public HashSet<Opinion> Opinions = new HashSet<Opinion>();

        public DateTime LastUpdate
        {
            get
            {
                return _lastUpdate;
            }
        }

//        public update

        public int Update(IMiner miner)
        {
            var opinions = miner.GetOpinions(_lastUpdate, int.MaxValue).ToList();

            opinions.RemoveAll(o => Opinions.Contains(o));

            if (opinions.Any())
            {
                Opinion.Save(opinions.ToList(), _path + "/" + Guid.NewGuid().ToString() + ".xml");
                _lastUpdate = opinions.Max(o => o.Date);
                File.WriteAllLines(_path + "/" + "config.txt", new string[] { _lastUpdate.ToString() });
            }

            foreach(var o in opinions)
                Opinions.Add(o);

            return opinions.Count;
        }

        public OpinionStore(string path = "opinionStore/")
        {
            _path = path;

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
 
            if (File.Exists(_path + "/" + "config.txt"))
            {
                try
                {
                    var lines = File.ReadAllLines(_path + "/" + "config.txt");
                    _lastUpdate = DateTime.Parse(lines.First());
                }
                catch { }
            }

            var files = Directory.GetFiles(_path).ToList();
            files.RemoveAll(f => f.Contains("config"));

            foreach (var file in files) 
            {
                try
                {
                    var opinionSet = Opinion.Load(file);
                    foreach (var o in opinionSet)
                        Opinions.Add(o);
                }
                catch { }
            }

            Opinions.RemoveWhere(o => null == o);
        }
    }
}
