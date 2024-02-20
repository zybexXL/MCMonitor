using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MCMonitor
{
    public class SafeDictionary<T1, T2> : Dictionary<T1, T2>
    {
        public SafeDictionary(IEqualityComparer<T1> comparer) : base(comparer) { }

        public new T2 this[T1 key] { 
            get { return TryGetValue(key, out T2 value) ? value : default(T2); }
            set { base[key] = value; } }
    }
}
