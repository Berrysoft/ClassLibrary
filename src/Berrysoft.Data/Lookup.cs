using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Berrysoft.Data
{
    public interface ILookup<TKey, TElement> : System.Linq.ILookup<TKey, TElement>
    {
        ICollection<TKey> Keys { get; }
        void Add(TKey key, TElement element);
        bool Remove(TKey key);
        bool Remove(TKey key, TElement element);
        void Clear();
        bool TryGetElements(TKey key, out IEnumerable<TElement> elements);
    }
    internal interface IGrouping<TKey, TElement> : System.Linq.IGrouping<TKey, TElement>
    {
        int Count { get; }
    }
    /// <summary>
    /// Represents a collection of keys each mapped to one or more values.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the <see cref="Lookup{TKey, TElement}"/>.</typeparam>
    /// <typeparam name="TElement">The type of the elements of each value in the <see cref="Lookup{TKey, TElement}"/>.</typeparam>
    [Serializable]
    public class Lookup<TKey, TElement> : ILookup<TKey, TElement>, IEnumerable<IGrouping<TKey, TElement>>
    {
        private Dictionary<TKey, Grouping> dic;
        /// <summary>
        /// Initialize a new instance of <see cref="Lookup{TKey, TElement}"/> class.
        /// </summary>
        public Lookup()
            : this(0, null)
        { }
        public Lookup(int capacity)
            : this(capacity, null)
        { }
        public Lookup(int capacity, IEqualityComparer<TKey> comparer)
        {
            dic = new Dictionary<TKey, Grouping>(capacity, comparer);
        }
        /// <summary>
        /// Gets the collection of values indexed by the specified key.
        /// </summary>
        /// <param name="key">The key of the desired collection of values.</param>
        /// <returns>The collection of values indexed by the specified key.</returns>
        public ICollection<TElement> this[TKey key]
        {
            get
            {
                if (!dic.ContainsKey(key))
                {
                    dic[key] = new Grouping(key);
                }
                return dic[key];
            }
        }
        IEnumerable<TElement> System.Linq.ILookup<TKey, TElement>.this[TKey key] => this[key];
        /// <summary>
        /// Gets the number of key/value collection pairs in the <see cref="Lookup{TKey, TElement}"/>.
        /// </summary>
        public int Count => dic.Count;
        public ICollection<TKey> Keys => dic.Keys;
        public void Add(TKey key,TElement element)
        {
            if(!dic.ContainsKey(key))
            {
                dic[key] = new Grouping(key);
            }
            dic[key].Add(element);
        }
        public bool Remove(TKey key)
        {
            return dic.Remove(key);
        }
        public bool Remove(TKey key, TElement element)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key].Remove(element);
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Determines whether a specified key is in the <see cref="Lookup{TKey, TElement}"/>.
        /// </summary>
        /// <param name="key">The key to find in the <see cref="Lookup{TKey, TElement}"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="key"/> is in the <see cref="Lookup{TKey, TElement}"/>; otherwise, <see langword="false"/>.</returns>
        public bool Contains(TKey key) => dic.ContainsKey(key);
        public void Clear() => dic.Clear();
        public bool TryGetElements(TKey key, out ICollection<TElement> elements)
        {
            bool result = dic.TryGetValue(key, out var grouping);
            elements = grouping;
            return result;
        }
        bool ILookup<TKey,TElement>.TryGetElements(TKey key, out IEnumerable<TElement> elements)
        {
            bool result = dic.TryGetValue(key, out var grouping);
            elements = grouping;
            return result;
        }
        /// <summary>
        /// Returns a generic enumerator that iterates through the <see cref="Lookup{TKey, TElement}"/>.
        /// </summary>
        /// <returns>An enumerator for the <see cref="Lookup{TKey, TElement}"/>.</returns>
        public IEnumerator<System.Linq.IGrouping<TKey, TElement>> GetEnumerator()
        {
            foreach (var item in dic)
            {
                yield return item.Value;
            }
        }
        IEnumerator<IGrouping<TKey, TElement>> IEnumerable<IGrouping<TKey, TElement>>.GetEnumerator()
        {
            foreach (var item in dic)
            {
                yield return item.Value;
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
        private class Grouping : IGrouping<TKey, TElement>, ICollection<TElement>, IReadOnlyCollection<TElement>
        {
            private TKey key;
            private Collection<TElement> collection;
            /// <summary>
            /// Initialize a new instance of <see cref="Grouping"/> class.
            /// </summary>
            /// <param name="key"></param>
            public Grouping(TKey key)
            {
                this.key = key;
                this.collection = new Collection<TElement>();
            }
            /// <summary>
            /// The key of the <see cref="Grouping"/>.
            /// </summary>
            public TKey Key => key;
            public int Count => collection.Count;
            public bool IsReadOnly => true;
            public void Add(TElement item) => collection.Add(item);
            public void Clear() => collection.Clear();
            public bool Remove(TElement item) => collection.Remove(item);
            public bool Contains(TElement item) => collection.Contains(item);
            public void CopyTo(TElement[] array, int arrayIndex)
            {
                if (array == null)
                    throw ExceptionHelper.ArgumentNull(nameof(array));
                if (arrayIndex < 0)
                    throw ExceptionHelper.ArgumentOutOfRange(nameof(arrayIndex));
                if (arrayIndex > array.Length)
                    throw ExceptionHelper.ArgumentOutOfRange(nameof(arrayIndex));
                if (array.Length - arrayIndex < collection.Count)
                    throw ExceptionHelper.ArrayTooSmall();
                collection.CopyTo(array, arrayIndex);
            }
            /// <summary>
            /// Returns a generic enumerator that iterates through the <see cref="Grouping"/>.
            /// </summary>
            /// <returns>An enumerator for the <see cref="Grouping"/>.</returns>
            public IEnumerator<TElement> GetEnumerator() => collection.GetEnumerator();
            /// <summary>
            /// Returns a generic enumerator that iterates through the <see cref="Grouping"/>.
            /// </summary>
            /// <returns>An enumerator for the <see cref="Grouping"/>.</returns>
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
