
using MarketCallLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionMiner
{
    public interface IOpinionMiner
    {
        IEnumerable<Opinion> GetOpinions(DateTime stop, int max);
    }
}
