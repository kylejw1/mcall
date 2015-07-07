﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketCallLibs
{
    public class Guest
    {
        public string Name = "";
        public string Company = "";

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode().Equals(obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Company.GetHashCode();
        }
    }
}