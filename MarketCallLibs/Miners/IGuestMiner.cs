using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketCallLibs
{
    public interface IGuestMiner
    {
        IEnumerable<Guest> GetGuests();
    }
}
