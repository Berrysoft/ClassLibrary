using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Berrysoft.Data
{
    #region Interfaces
    /// <summary>
    /// Defines methods that map keys to values.
    /// </summary>
    /// <typeparam name="TKey">Type of the keys.</typeparam>
    /// <typeparam name="TElement">Type of the elements.</typeparam>
    public interface IMutableLookup<TKey, TElement> : ILookup<TKey, TElement>
    {
        /// <summary>
        /// A collection of keys.
        /// </summary>
        ICollection<TKey> Keys { get; }
        /// <summary>
        /// Adds the specified key and element.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <param name="element">The specified element.</param>
        void Add(TKey key, TElement element);
        /// <summary>
        /// Removes the specified key and its elements.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        bool Remove(TKey key);
        /// <summary>
        /// Removes the specified key and element.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <param name="element">The specified element.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        bool Remove(TKey key, TElement element);
        /// <summary>
        /// Clear the <see cref="IMutableLookup{TKey, TElement}"/>.
        /// </summary>
        void Clear();
        /// <summary>
        /// Gets the <see cref="IEnumerable{T}"/> sequence of values by a specified key.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <param name="elements">When this method returns, contains the elements associated with the specified key, if the key is found; otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the <see cref="IMutableLookup{TKey, TElement}"/> contains elements with the specified key; otherwise, <see langword="false"/>.</returns>
        bool TryGetElements(TKey key, out IEnumerable<TElement> elements);
    }
    /// <summary>
    /// Represents a collection of objects that have a common key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key of the <see cref="ICountableGrouping{TKey, TElement}"/>.</typeparam>
    /// <typeparam name="TElement">The type of the values in the <see cref="ICountableGrouping{TKey, TElement}"/>.</typeparam>
    internal interface ICountableGrouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        /// <summary>
        /// Count of the elements.
        /// </summary>
        int Count { get; }
    }
    #endregion
    /// <summary>
    /// Represents a collection of keys each mapped to one or more values.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the <see cref="Lookup{TKey, TElement}"/>.</typeparam>
    /// <typeparam name="TElement">The type of the elements of each value in the <see cref="Lookup{TKey, TElement}"/>.</typeparam>
    [Serializable]
    public class Lookup<TKey, TElement> : IMutableLookup<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<ICountableGrouping<TKey, TElement>>
    {
        private Dictionary<TKey, Grouping> dic;
        /// <summary>
        /// Initialize a new instance of <see cref="Lookup{TKey, TElement}"/> class.
        /// </summary>
        public Lookup()
            : this(0, null)
        { }
        /// <summary>
        /// Initialize a new instance of <see cref="Lookup{TKey, TElement}"/> class.
        /// </summary>
        /// <param name="capacity">The initial number of keys that the <see cref="IMutableLookup{TKey, TElement}"/> can contain.</param>
        public Lookup(int capacity)
            : this(capacity, null)
        { }
        /// <summary>
        /// Initialize a new instance of <see cref="Lookup{TKey, TElement}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer of the keys.</param>
        public Lookup(IEqualityComparer<TKey> comparer)
            : this(0, comparer)
        { }
        /// <summary>
        /// Initialize a new instance of <see cref="Lookup{TKey, TElement}"/> class.
        /// </summary>
        /// <param name="capacity">The initial number of keys that the <see cref="IMutableLookup{TKey, TElement}"/> can contain.</param>
        /// <param name="comparer">The comparer of the keys.</param>
        public Lookup(int capacity, IEqualityComparer<TKey> comparer)
        {
            dic = new Dictionary<TKey, Grouping>(capacity, comparer);
        }
        /// <summary>
        /// Initialize a new instance of <see cref="Lookup{TKey, TElement}"/> class.
        /// </summary>
        /// <param name="lookup">A lookup source.</param>
        public Lookup(ILookup<TKey, TElement> lookup)
            : this(lookup, null)
        { }
        /// <summary>
        /// Initialize a new instance of <see cref="Lookup{TKey, TElement}"/> class.
        /// </summary>
        /// <param name="lookup">A lookup source.</param>
        /// <param name="comparer">The comparer of the keys.</param>
        public Lookup(ILookup<TKey, TElement> lookup, IEqualityComparer<TKey> comparer)
        {
            switch (lookup ?? throw ExceptionHelper.ArgumentNull(nameof(lookup)))
            {
                case Lookup<TKey, TElement> lkp:
                    dic = new Dictionary<TKey, Grouping>(lkp.dic, comparer);
                    break;
                default:
                    dic = new Dictionary<TKey, Grouping>(lookup.Count, comparer);
                    foreach (var grouping in lookup)
                    {
                        foreach (var item in grouping)
                        {
                            Add(grouping.Key, item);
                        }
                    }
                    break;
            }
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
        /// <summary>
        /// Gets the number of key/value collection pairs in the <see cref="Lookup{TKey, TElement}"/>.
        /// </summary>
        public int Count => dic.Count;
        /// <summary>
        /// A collection of keys.
        /// </summary>
        public ICollection<TKey> Keys => dic.Keys;
        /// <summary>
        /// A collection of keys.
        /// </summary>
        IEnumerable<TElement> ILookup<TKey, TElement>.this[TKey key] => this[key];
        /// <summary>
        /// Adds the specified key and element.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <param name="element">The specified element.</param>
        public void Add(TKey key,TElement element)
        {
            if(!dic.ContainsKey(key))
            {
                dic[key] = new Grouping(key);
            }
            dic[key].AddElement(element);
        }
        /// <summary>
        /// Removes the specified key and its elements.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        public bool Remove(TKey key)
        {
            return dic.Remove(key);
        }
        /// <summary>
        /// Removes the specified key and element.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <param name="element">The specified element.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        public bool Remove(TKey key, TElement element)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key].RemoveElement(element);
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
        /// <summary>
        /// Clear the <see cref="IMutableLookup{TKey, TElement}"/>.
        /// </summary>
        public void Clear() => dic.Clear();
        /// <summary>
        /// Gets the <see cref="IEnumerable{T}"/> sequence of values by a specified key.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <param name="elements">When this method returns, contains the elements associated with the specified key, if the key is found; otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the <see cref="IMutableLookup{TKey, TElement}"/> contains elements with the specified key; otherwise, <see langword="false"/>.</returns>
        public bool TryGetElements(TKey key, out ICollection<TElement> elements)
        {
            bool result = dic.TryGetValue(key, out var grouping);
            elements = grouping;
            return result;
        }
        /// <summary>
        /// Gets the <see cref="IEnumerable{T}"/> sequence of values by a specified key.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <param name="elements">When this method returns, contains the elements associated with the specified key, if the key is found; otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the <see cref="IMutableLookup{TKey, TElement}"/> contains elements with the specified key; otherwise, <see langword="false"/>.</returns>
        public bool TryGetElements(TKey key, out IEnumerable<TElement> elements)
        {
            bool result = dic.TryGetValue(key, out var grouping);
            elements = grouping;
            return result;
        }
        /// <summary>
        /// Returns a generic enumerator that iterates through the <see cref="Lookup{TKey, TElement}"/>.
        /// </summary>
        /// <returns>An enumerator for the <see cref="Lookup{TKey, TElement}"/>.</returns>
        public IEnumerator<KeyValuePair<TKey, TElement>> GetEnumerator()
        {
            foreach(var grouping in dic)
            {
                foreach(var item in grouping.Value)
                {
                    yield return new KeyValuePair<TKey, TElement>(grouping.Key, item);
                }
            }
        }
        /// <summary>
        /// Returns a generic enumerator that iterates through the <see cref="Lookup{TKey, TElement}"/>.
        /// </summary>
        /// <returns>An enumerator for the <see cref="Lookup{TKey, TElement}"/>.</returns>
        IEnumerator<IGrouping<TKey, TElement>> IEnumerable<IGrouping<TKey, TElement>>.GetEnumerator() => GetEnumeratorInternal();
        /// <summary>
        /// Returns a generic enumerator that iterates through the <see cref="Lookup{TKey, TElement}"/>.
        /// </summary>
        /// <returns>An enumerator for the <see cref="Lookup{TKey, TElement}"/>.</returns>
        IEnumerator<ICountableGrouping<TKey, TElement>> IEnumerable<ICountableGrouping<TKey, TElement>>.GetEnumerator() => GetEnumeratorInternal();
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Lookup{TKey, TElement}"/>. This class cannot be inherited.
        /// </summary>
        /// <returns>An enumerator for the <see cref="Lookup{TKey, TElement}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();
        /// <summary>
        /// Returns a generic enumerator that iterates through the <see cref="Lookup{TKey, TElement}"/>.
        /// </summary>
        /// <returns>An enumerator for the <see cref="Lookup{TKey, TElement}"/>.</returns>
        private IEnumerator<Grouping> GetEnumeratorInternal()
        {
            foreach (var item in dic)
            {
                yield return item.Value;
            }
        }
        /// <summary>
        /// Represents a key and a sequence of elements.
        /// </summary>
        private class Grouping : ICountableGrouping<TKey, TElement>, ICollection<TElement>
        {
            private readonly TKey key;
            private readonly Collection<TElement> collection;
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
            /// <summary>
            /// Count of the elements.
            /// </summary>
            public int Count => collection.Count;
            /// <summary>
            /// The <see cref="Grouping"/> is not read-only.
            /// </summary>
            public bool IsReadOnly => true;
            /// <summary>
            /// Adds element to the grouping.
            /// </summary>
            /// <param name="item">The specified element.</param>
            internal void AddElement(TElement item) => collection.Add(item);
            /// <summary>
            /// This function is not supported.
            /// </summary>
            /// <param name="item">The specified element.</param>
            /// <exception cref="NotSupportedException"/>
            public void Add(TElement item) => throw ExceptionHelper.NotSupported();
            /// <summary>
            /// Clears the elements.
            /// </summary>
            internal void ClearElements() => collection.Clear();
            /// <summary>
            /// This function is not supported.
            /// </summary>
            /// <exception cref="NotSupportedException"/>
            public void Clear() => throw ExceptionHelper.NotSupported();
            /// <summary>
            /// Removes the specified element.
            /// </summary>
            /// <param name="item">The specified element.</param>
            /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
            internal bool RemoveElement(TElement item) => collection.Remove(item);
            /// <summary>
            /// This function is not supported.
            /// </summary>
            /// <param name="item">The specified element.</param>
            /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="NotSupportedException"/>
            public bool Remove(TElement item) => throw ExceptionHelper.NotSupported();
            /// <summary>
            /// Determines whether a specified key is in the <see cref="Grouping"/>.
            /// </summary>
            /// <param name="item">The element to find in the <see cref="Grouping"/>.</param>
            /// <returns><see langword="true"/> if <paramref name="item"/> is in the <see cref="Grouping"/>; otherwise, <see langword="false"/>.</returns>
            public bool Contains(TElement item) => collection.Contains(item);
            /// <summary>
            /// Copies the entire <see cref="Collection{T}"/> to a compatible one-dimensional <see cref="Array"/>, starting at the specified index of the target array.
            /// </summary>
            /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="Collection{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
            /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
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
