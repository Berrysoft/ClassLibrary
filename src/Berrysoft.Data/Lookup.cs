using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Berrysoft.Data
{
    /// <summary>
    /// Represents a collection of keys each mapped to one or more values.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the <see cref="Lookup{TKey, TElement}"/>.</typeparam>
    /// <typeparam name="TElement">The type of the elements of each <see cref="HashSet{T}"/> value in the <see cref="Lookup{TKey, TElement}"/>.</typeparam>
    internal class Lookup<TKey, TElement> : ILookup<TKey, TElement>
    {
        private Dictionary<TKey, HashSet<TElement>> dic;
        /// <summary>
        /// Initialize a new instance of <see cref="Lookup{TKey, TElement}"/> class.
        /// </summary>
        /// <param name="dic">The dictionary to resemble.</param>
        public Lookup(Dictionary<TKey, HashSet<TElement>> dic)
        {
            this.dic = dic;
        }
        /// <summary>
        /// Gets the collection of values indexed by the specified key.
        /// </summary>
        /// <param name="key">The key of the desired collection of values.</param>
        /// <returns>The collection of values indexed by the specified key.</returns>
        public IEnumerable<TElement> this[TKey key] => dic[key];
        /// <summary>
        /// Gets the number of key/value collection pairs in the <see cref="Lookup{TKey, TElement}"/>.
        /// </summary>
        public int Count => dic.Count;
        /// <summary>
        /// Determines whether a specified key is in the <see cref="Lookup{TKey, TElement}"/>.
        /// </summary>
        /// <param name="key">The key to find in the <see cref="Lookup{TKey, TElement}"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="key"/> is in the <see cref="Lookup{TKey, TElement}"/>; otherwise, <see langword="false"/>.</returns>
        public bool Contains(TKey key) => dic.ContainsKey(key);
        /// <summary>
        /// Returns a generic enumerator that iterates through the <see cref="Lookup{TKey, TElement}"/>.
        /// </summary>
        /// <returns>An enumerator for the <see cref="Lookup{TKey, TElement}"/>.</returns>
        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            foreach (var item in dic)
            {
                yield return new Grouping(item.Key, item.Value);
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Lookup{TKey, TElement}"/>. This class cannot be inherited.
        /// </summary>
        /// <returns>An enumerator for the <see cref="Lookup{TKey, TElement}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        /// <summary>
        /// Represents a key and a sequence of elements.
        /// </summary>
        internal class Grouping : IGrouping<TKey, TElement>
        {
            private TKey key;
            private HashSet<TElement> elements;
            /// <summary>
            /// Initialize a new instance of <see cref="Grouping"/> class.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="elements"></param>
            public Grouping(TKey key, HashSet<TElement> elements)
            {
                this.key = key;
                this.elements = elements;
            }
            /// <summary>
            /// The key of the <see cref="Grouping"/>.
            /// </summary>
            public TKey Key => key;
            /// <summary>
            /// Returns a generic enumerator that iterates through the <see cref="Grouping"/>.
            /// </summary>
            /// <returns>An enumerator for the <see cref="Grouping"/>.</returns>
            public IEnumerator<TElement> GetEnumerator() => elements.GetEnumerator();
            /// <summary>
            /// Returns a generic enumerator that iterates through the <see cref="Grouping"/>.
            /// </summary>
            /// <returns>An enumerator for the <see cref="Grouping"/>.</returns>
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
