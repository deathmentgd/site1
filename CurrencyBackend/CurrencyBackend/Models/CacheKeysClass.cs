using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyBackend.Models
{    
    public static class CacheKeys
    {
        public static string AvailableCurrencyList { get { return "_AvailableCurrencyList"; } }
        public static string TrackedCurrencyList { get { return "_TrackedCurrencyList"; } }
        public static string TrackedDateValues { get { return "_TrackedDateValues"; } }
    }
}
