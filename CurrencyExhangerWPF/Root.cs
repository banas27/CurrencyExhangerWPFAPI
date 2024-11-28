using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExhangerWPF
{
    public class Root // API Return rates in a rates it Return All Currency Name With Value
    {
        public Rates rates { get; set; } // get all Record in rates and Set in Rate Class as Currency Name Wise
        public long timestamp;
        public string license;
    }
}
