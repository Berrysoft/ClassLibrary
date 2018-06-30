using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Berrysoft.Data
{
    #region Interfaces
    /// <summary>
    /// Exposes members of a multi-value map.
    /// </summary>
    /// <typeparam name="TKey1">Type of key1.</typeparam>
    /// <typeparam name="TKey2">Type of key2.</typeparam>
    public interface IMultiMap<TKey1, TKey2> : ICollection<KeyPair<TKey1, TKey2>>
    {
        /// <summary>
        /// Gets an <see cref="ICollection{TKey1}"/> containing the keys in the <see cref="IMultiMap{TKey1, TKey2}"/>.
        /// </summary>
        ICollection<TKey1> Keys1 { get; }
        /// <summary>
        /// Gets an <see cref="ICollection{TKey2}"/> containing the keys in the <see cref="IMultiMap{TKey1, TKey2}"/>.
        /// </summary>
        ICollection<TKey2> Keys2 { get; }
        /// <summary>
        /// Get <see cref="ICollection{TKey2}"/> of key2 with the specified key1.
        /// </summary>
        /// <param name="key">The key1 of the key2 to get.</param>
        /// <returns>An <see cref="ICollection{TKey2}"/> of key2.</returns>
        ICollection<TKey2> GetValuesFromKey1(TKey1 key);
        /// <summary>
        /// Get <see cref="ICollection{TKey1}"/> of key1 with the specified key2.
        /// </summary>
        /// <param name="key">The key2 of the key1 to get.</param>
        /// <returns>An <see cref="ICollection{TKey1}"/> of key1.</returns>
        ICollection<TKey1> GetValuesFromKey2(TKey2 key);
        /// <summary>
        /// Get <see cref="ICollection{TKey2}"/> of key2 with the specified key1.
        /// </summary>
        /// <param name="key">The key1 of the key2 to get.</param>
        /// <param name="values">When the method returns, contains an <see cref="ICollection{TKey2}"/> of key2, if the key1 is found; otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the <see cref="IMultiMap{TKey1, TKey2}"/> contains elements with the specified key1; otherwise, <see langword="false"/>.</returns>
        bool TryGetValuesFromKey1(TKey1 key, out ICollection<TKey2> values);
        /// <summary>
        /// Get <see cref="ICollection{TKey1}"/> of key1 with the specified key2.
        /// </summary>
        /// <param name="key">The key2 of the key1 to get.</param>
        /// <param name="values">When the method returns, contains an <see cref="ICollection{TKey1}"/> of key1, if the key2 is found; otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the <see cref="IMultiMap{TKey1, TKey2}"/> contains elements with the specified key2; otherwise, <see langword="false"/>.</returns>
        bool TryGetValuesFromKey2(TKey2 key, out ICollection<TKey1> values);
        /// <summary>
        /// Adds the specified key1 and key2.
        /// </summary>
        /// <param name="key1">The key1 of the element to add.</param>
        /// <param name="key2">The key2 of the element to add.</param>
        void Add(TKey1 key1, TKey2 key2);
        /// <summary>
        /// Adds the specified key1 and key2.
        /// </summary>
        /// <param name="key1">The key1 of the element to add.</param>
        /// <param name="key2">The key2 of the element to add.</param>
        /// <returns><see langword="true"/> if adds successfully; otherwise, <see langword="false"/>.</returns>
        bool TryAdd(TKey1 key1, TKey2 key2);
        /// <summary>
        /// Determines whether the <see cref="IMultiMap{TKey1, TKey2}"/> contains the specified key1.
        /// </summary>
        /// <param name="key">The specified key1.</param>
        /// <returns><see langword="true"/> if it contains; otherwise, <see langword="false"/>.</returns>
        bool ContainsKey1(TKey1 key);
        /// <summary>
        /// Determines whether the <see cref="IMultiMap{TKey1, TKey2}"/> contains the specified key2.
        /// </summary>
        /// <param name="key">The specified key2.</param>
        /// <returns><see langword="true"/> if it contains; otherwise, <see langword="false"/>.</returns>
        bool ContainsKey2(TKey2 key);
        /// <summary>
        /// Determines whether the <see cref="IMultiMap{TKey1, TKey2}"/> contains the element with specified key1 and key2.
        /// </summary>
        /// <param name="key1">The specified key1.</param>
        /// <param name="key2">The specified key2.</param>
        /// <returns><see langword="true"/> if it contains; otherwise, <see langword="false"/>.</returns>
        bool Contains(TKey1 key1, TKey2 key2);
        /// <summary>
        /// Removes elements with specified key1.
        /// </summary>
        /// <param name="key">The specified key1.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        bool RemoveKey1(TKey1 key);
        /// <summary>
        /// Removes elements with specified key2.
        /// </summary>
        /// <param name="key">The specified key2.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        bool RemoveKey2(TKey2 key);
        /// <summary>
        /// Removes an element with specified key1 and key2.
        /// </summary>
        /// <param name="key1">The specified key1.</param>
        /// <param name="key2">The specified key2.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        bool Remove(TKey1 key1, TKey2 key2);
    }
    #endregion
    public static partial class Enumerable
    {
        /// <summary>
        /// Get <see cref="MultiMap{TKey1, TKey2}"/> from <see cref="IEnumerable{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">Type of elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey1">Type of key1.</typeparam>
        /// <typeparam name="TKey2">Type of key2.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key1Selector">Selector of key1.</param>
        /// <param name="key2Selector">Selector of key2.</param>
        /// <returns>An instance of <see cref="MultiMap{TKey1, TKey2}"/>.</returns>
        public static MultiMap<TKey1, TKey2> ToMultiMap<TSource, TKey1, TKey2>(this IEnumerable<TSource> source, Func<TSource, TKey1> key1Selector, Func<TSource, TKey2> key2Selector)
            => ToMultiMap(source, key1Selector, key2Selector, null, null);
        /// <summary>
        /// Get <see cref="MultiMap{TKey1, TKey2}"/> from <see cref="IEnumerable{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">Type of elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey1">Type of key1.</typeparam>
        /// <typeparam name="TKey2">Type of key2.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key1Selector">Selector of key1.</param>
        /// <param name="key2Selector">Selector of key2.</param>
        /// <param name="comparer1">Comparer of key1.</param>
        /// <param name="comparer2">Comparer of key2.</param>
        /// <returns>An instance of <see cref="MultiMap{TKey1, TKey2}"/>.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> or <paramref name="key1Selector"/> or <paramref name="key2Selector"/> is <see langword="null"/>.</exception>
        public static MultiMap<TKey1, TKey2> ToMultiMap<TSource, TKey1, TKey2>(this IEnumerable<TSource> source, Func<TSource, TKey1> key1Selector, Func<TSource, TKey2> key2Selector, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
        {
            if (source == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(source));
            }
            if (key1Selector == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(key1Selector));
            }
            if (key2Selector == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(key2Selector));
            }
            MultiMap<TKey1, TKey2> lookup = new MultiMap<TKey1, TKey2>(comparer1, comparer2);
            foreach (TSource item in source)
            {
                lookup.Add(key1Selector(item), key2Selector(item));
            }
            return lookup;
        }
    }
    /// <summary>
    /// Represents a multi-value map.
    /// </summary>
    /// <typeparam name="TKey1">Type of key1.</typeparam>
    /// <typeparam name="TKey2">Type of key2.</typeparam>
    [Serializable]
    [DebuggerTypeProxy(typeof(MultiMapDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    public class MultiMap<TKey1, TKey2> : IMultiMap<TKey1, TKey2>
    {
        private Lookup<TKey1, TKey2> dic;
        private Lookup<TKey2, TKey1> rev;
        /// <summary>
        /// Initialize an instance of <see cref="MultiMap{TKey1, TKey2}"/>.
        /// </summary>
        public MultiMap()
            : this(0, null, null)
        { }
        /// <summary>
        /// Initialize an instance of <see cref="MultiMap{TKey1, TKey2}"/>.
        /// </summary>
        /// <param name="capacity">The number of elements that the new map can initially store.</param>
        public MultiMap(int capacity)
            : this(capacity, null, null)
        { }
        /// <summary>
        /// Initialize an instance of <see cref="MultiMap{TKey1, TKey2}"/>.
        /// </summary>
        /// <param name="comparer1">Comparer of key1.</param>
        /// <param name="comparer2">Comparer of key2.</param>
        public MultiMap(IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
            : this(0, comparer1, comparer2)
        { }
        /// <summary>
        /// Initialize an instance of <see cref="MultiMap{TKey1, TKey2}"/>.
        /// </summary>
        /// <param name="capacity">The number of elements that the new map can initially store.</param>
        /// <param name="comparer1">Comparer of key1.</param>
        /// <param name="comparer2">Comparer of key2.</param>
        public MultiMap(int capacity, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
        {
            this.dic = new Lookup<TKey1, TKey2>(capacity, comparer1);
            this.rev = new Lookup<TKey2, TKey1>(capacity, comparer2);
        }
        /// <summary>
        /// Initialize an instance of <see cref="MultiMap{TKey1, TKey2}"/>.
        /// </summary>
        /// <param name="map">Existed map.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="map"/> is <see langword="null"/>.</exception>
        public MultiMap(IMultiMap<TKey1, TKey2> map)
            : this(map, null, null)
        { }
        /// <summary>
        /// Initialize an instance of <see cref="MultiMap{TKey1, TKey2}"/>.
        /// </summary>
        /// <param name="map">Existed map.</param>
        /// <param name="comparer1">Comparer of key1.</param>
        /// <param name="comparer2">Comparer of key2.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="map"/> is <see langword="null"/>.</exception>
        public MultiMap(IMultiMap<TKey1, TKey2> map, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
            : this(map?.Count ?? 0, comparer1, comparer2)
        {
            foreach (var item in map ?? throw ExceptionHelper.ArgumentNull(nameof(map)))
            {
                Add(item.Key1, item.Key2);
            }
        }
        /// <summary>
        /// Count of elements.
        /// </summary>
        public int Count => dic.Sum<ICountableGrouping<TKey1, TKey2>>(pair => pair.Count);
        /// <summary>
        /// Gets a value indicating whether the <see cref="MultiMap{TKey1, TKey2}"/> is read-only.
        /// </summary>
        /// <value><see langword="false"/></value>
        public bool IsReadOnly => false;
        /// <summary>
        /// Gets an <see cref="ICollection{TKey1}"/> containing the keys in the <see cref="MultiMap{TKey1, TKey2}"/>.
        /// </summary>
        public ICollection<TKey1> Keys1 => dic.Keys;
        /// <summary>
        /// Gets an <see cref="ICollection{TKey2}"/> containing the keys in the <see cref="MultiMap{TKey1, TKey2}"/>.
        /// </summary>
        public ICollection<TKey2> Keys2 => rev.Keys;
        /// <summary>
        /// Inserts an elenemt with specified key1 and key2.
        /// </summary>
        /// <param name="key1">The specified key1.</param>
        /// <param name="key2">The specified key2.</param>
        /// <param name="add">Determines whether this is an add method.</param>
        /// <returns><see langword="true"/> if inserts successfully; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="key1"/> or <paramref name="key2"/> is <see langword="null"/> and <paramref name="add"/> is <see langword="true"/>.</exception>
        /// <exception cref="ArgumentException">When the specified element is existed and <paramref name="add"/> is <see langword="true"/>.</exception>
        private bool Insert(TKey1 key1, TKey2 key2, bool add)
        {
            if (key1 == null)
            {
                if (add)
                {
                    throw ExceptionHelper.ArgumentNull(nameof(key1));
                }
                else
                {
                    return false;
                }
            }
            if (key2 == null)
            {
                if (add)
                {
                    throw ExceptionHelper.ArgumentNull(nameof(key2));
                }
                else
                {
                    return false;
                }
            }
            if (dic.Contains(key1))
            {
                if (dic[key1].Contains(key2))
                {
                    if (add)
                    {
                        throw ExceptionHelper.PairExisted();
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    dic.Add(key1, key2);
                }
            }
            else
            {
                dic.Add(key1, key2);
            }
            if (rev.Contains(key2))
            {
                rev.Add(key2, key1);
            }
            else
            {
                rev.Add(key2, key1);
            }
            return true;
        }
        /// <summary>
        /// Adds the specified key1 and key2.
        /// </summary>
        /// <param name="key1">The key1 of the element to add.</param>
        /// <param name="key2">The key2 of the element to add.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="key1"/> or <paramref name="key2"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">When the specified element is existed.</exception>
        public void Add(TKey1 key1, TKey2 key2) => Insert(key1, key2, true);
        /// <summary>
        /// Adds an element with specified key1 and key2.
        /// </summary>
        /// <param name="item">The element.</param>
        /// <exception cref="ArgumentException">When the specified element is existed.</exception>
        public void Add(KeyPair<TKey1, TKey2> item) => Add(item.Key1, item.Key2);
        /// <summary>
        /// Adds the specified key1 and key2.
        /// </summary>
        /// <param name="key1">The key1 of the element to add.</param>
        /// <param name="key2">The key2 of the element to add.</param>
        /// <returns><see langword="true"/> if inserts successfully; otherwise, <see langword="false"/>.</returns>
        public bool TryAdd(TKey1 key1, TKey2 key2) => Insert(key1, key2, false);
        /// <summary>
        /// Get <see cref="ICollection{TKey2}"/> of key2 with the specified key1.
        /// </summary>
        /// <param name="key">The key1 of the key2 to get.</param>
        /// <returns>An <see cref="ICollection{TKey2}"/> of key2.</returns>
        public ICollection<TKey2> GetValuesFromKey1(TKey1 key) => dic[key];
        /// <summary>
        /// Get <see cref="ICollection{TKey1}"/> of key1 with the specified key2.
        /// </summary>
        /// <param name="key">The key2 of the key1 to get.</param>
        /// <returns>An <see cref="ICollection{TKey1}"/> of key1.</returns>
        public ICollection<TKey1> GetValuesFromKey2(TKey2 key) => rev[key];
        /// <summary>
        /// Get <see cref="ICollection{TKey2}"/> of key2 with the specified key1.
        /// </summary>
        /// <param name="key">The key1 of the key2 to get.</param>
        /// <param name="values">When the method returns, contains an <see cref="ICollection{TKey2}"/> of key2, if the key1 is found; otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the <see cref="IMultiMap{TKey1, TKey2}"/> contains elements with the specified key1; otherwise, <see langword="false"/>.</returns>
        public bool TryGetValuesFromKey1(TKey1 key, out ICollection<TKey2> values) => dic.TryGetElements(key, out values);
        /// <summary>
        /// Get <see cref="ICollection{TKey1}"/> of key1 with the specified key2.
        /// </summary>
        /// <param name="key">The key2 of the key1 to get.</param>
        /// <param name="values">When the method returns, contains an <see cref="ICollection{TKey1}"/> of key1, if the key2 is found; otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the <see cref="IMultiMap{TKey1, TKey2}"/> contains elements with the specified key2; otherwise, <see langword="false"/>.</returns>
        public bool TryGetValuesFromKey2(TKey2 key, out ICollection<TKey1> values) => rev.TryGetElements(key, out values);
        /// <summary>
        /// Determines whether the <see cref="MultiMap{TKey1, TKey2}"/> contains the specified key1.
        /// </summary>
        /// <param name="key">The specified key1.</param>
        /// <returns><see langword="true"/> if it contains; otherwise, <see langword="false"/>.</returns>
        public bool ContainsKey1(TKey1 key) => dic.Contains(key);
        /// <summary>
        /// Determines whether the <see cref="MultiMap{TKey1, TKey2}"/> contains the specified key2.
        /// </summary>
        /// <param name="key">The specified key2.</param>
        /// <returns><see langword="true"/> if it contains; otherwise, <see langword="false"/>.</returns>
        public bool ContainsKey2(TKey2 key) => rev.Contains(key);
        /// <summary>
        /// Determines whether the <see cref="MultiMap{TKey1, TKey2}"/> contains the element with specified key1 and key2.
        /// </summary>
        /// <param name="key1">The specified key1.</param>
        /// <param name="key2">The specified key2.</param>
        /// <returns><see langword="true"/> if it contains; otherwise, <see langword="false"/>.</returns>
        public bool Contains(TKey1 key1, TKey2 key2)
        {
            if (key1 == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(key1));
            }
            if (key2 == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(key2));
            }
            return dic.Contains(key1) && dic[key1].Contains(key2);
        }
        /// <summary>
        /// Determines whether the <see cref="MultiMap{TKey1, TKey2}"/> contains the element with specified key1 and key2.
        /// </summary>
        /// <param name="item">The element.</param>
        /// <returns><see langword="true"/> if it contains; otherwise, <see langword="false"/>.</returns>
        public bool Contains(KeyPair<TKey1, TKey2> item) => Contains(item.Key1, item.Key2);
        /// <summary>
        /// Removes elements with specified key1.
        /// </summary>
        /// <param name="key">The specified key1.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        public bool RemoveKey1(TKey1 key)
        {
            ICollection<TKey2> values = dic[key];
            if (dic.Remove(key))
            {
                foreach (TKey2 value in values)
                {
                    rev.Remove(value, key);
                    if (rev[value].Count == 0)
                    {
                        rev.Remove(value);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Removes elements with specified key2.
        /// </summary>
        /// <param name="key">The specified key2.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        public bool RemoveKey2(TKey2 key)
        {
            ICollection<TKey1> values = rev[key];
            if (rev.Remove(key))
            {
                foreach (TKey1 value in values)
                {
                    dic.Remove(value, key);
                    if (dic[value].Count == 0)
                    {
                        dic.Remove(value);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Removes an element with specified key1 and key2.
        /// </summary>
        /// <param name="key1">The specified key1.</param>
        /// <param name="key2">The specified key2.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        public bool Remove(TKey1 key1, TKey2 key2)
        {
            if (dic.Remove(key1, key2) && rev.Remove(key2, key1))
            {
                if (dic[key1].Count == 0)
                {
                    if (!dic.Remove(key1))
                    {
                        return false;
                    }
                }
                if (rev[key2].Count == 0)
                {
                    if (!rev.Remove(key2))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Removes an element with specified key1 and key2.
        /// </summary>
        /// <param name="item">The element.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        public bool Remove(KeyPair<TKey1, TKey2> item) => Remove(item.Key1, item.Key2);
        /// <summary>
        /// Clear all elements.
        /// </summary>
        public void Clear()
        {
            dic.Clear();
            rev.Clear();
        }
        /// <summary>
        /// Copies the entire <see cref="Map{TKey1, TKey2}"/> to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="Map{TKey1, TKey2}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="arrayIndex"/> is smaller than zero or larger than the length of <paramref name="array"/>.</exception>
        /// <exception cref="ArgumentException">When <paramref name="array"/> is too small.</exception>
        public void CopyTo(KeyPair<TKey1, TKey2>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(array));
            }
            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw ExceptionHelper.ArgumentOutOfRange(nameof(arrayIndex));
            }
            if (array.Length - arrayIndex < dic.Count)
            {
                throw ExceptionHelper.ArrayTooSmall();
            }
            foreach (var item in this)
            {
                array[arrayIndex++] = item;
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="MultiMap{TKey1, TKey2}"/>.
        /// </summary>
        /// <returns>An enumerator for the <see cref="MultiMap{TKey1, TKey2}"/>.</returns>
        public IEnumerator<KeyPair<TKey1, TKey2>> GetEnumerator()
        {
            foreach (var item in (IMutableLookup<TKey1, TKey2>)dic)
            {
                foreach (var value in item)
                {
                    yield return new KeyPair<TKey1, TKey2>(item.Key, value);
                }
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="MultiMap{TKey1, TKey2}"/>.
        /// </summary>
        /// <returns>An enumerator for the <see cref="MultiMap{TKey1, TKey2}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        /// <summary>
        /// Get <see cref="IMutableLookup{TKey, TElement}"/> with key1 as key and key2 as value.
        /// </summary>
        /// <returns>An instance of <see cref="IMutableLookup{TKey, TElement}"/>.</returns>
        public ILookup<TKey1, TKey2> ToLookupFromKey1() => dic;
        /// <summary>
        /// Get <see cref="IMutableLookup{TKey, TElement}"/> with key2 as key and key1 as value.
        /// </summary>
        /// <returns>An instance of <see cref="IMutableLookup{TKey, TElement}"/>.</returns>
        public ILookup<TKey2, TKey1> ToLookupFromKey2() => rev;
    }
}
