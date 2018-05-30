using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Berrysoft.Data
{
    internal class Lookup<TKey, TElement> : ILookup<TKey, TElement>
    {
        private Dictionary<TKey, HashSet<TElement>> dic;
        public Lookup(Dictionary<TKey, HashSet<TElement>> dic)
        {
            this.dic = dic;
        }
        public IEnumerable<TElement> this[TKey key] => dic[key];
        public int Count => dic.Count;
        public bool Contains(TKey key) => dic.ContainsKey(key);
        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            foreach (var item in dic)
            {
                yield return new Grouping(item.Key, item.Value);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal class Grouping : IGrouping<TKey, TElement>
        {
            private TKey key;
            private HashSet<TElement> elements;
            public Grouping(TKey key, HashSet<TElement> elements)
            {
                this.key = key;
                this.elements = elements;
            }
            public TKey Key => key;
            public IEnumerator<TElement> GetEnumerator() => elements.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
