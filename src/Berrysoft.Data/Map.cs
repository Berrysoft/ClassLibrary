using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Berrysoft.Data
{
    #region Interfaces
    /// <summary>
    /// Exposes members of a two-way dictionary.
    /// </summary>
    /// <typeparam name="TKey1">Type of key1.</typeparam>
    /// <typeparam name="TKey2">Type of key2.</typeparam>
    public interface IMap<TKey1, TKey2> : ICollection<KeyPair<TKey1, TKey2>>
    {
        /// <summary>
        /// Gets an <see cref="ICollection{TKey1}"/> containing the keys in the <see cref="IMap{TKey1, TKey2}"/>.
        /// </summary>
        ICollection<TKey1> Keys1 { get; }
        /// <summary>
        /// Gets an <see cref="ICollection{TKey2}"/> containing the keys in the <see cref="IMap{TKey1, TKey2}"/>.
        /// </summary>
        ICollection<TKey2> Keys2 { get; }
        /// <summary>
        /// Get key2 with the specified key1.
        /// </summary>
        /// <param name="key">The key1 of the key2 to get.</param>
        /// <returns>Key2 related to the specified key1.</returns>
        TKey2 GetValueFromKey1(TKey1 key);
        /// <summary>
        /// Get key1 with the specified key2.
        /// </summary>
        /// <param name="key">The key2 of the key1 to get.</param>
        /// <returns>Key1 related to the specified key2.</returns>
        TKey1 GetValueFromKey2(TKey2 key);
        /// <summary>
        /// Gets the key2 associated with the specified key1.
        /// </summary>
        /// <param name="key">The key1 of the key2 to get.</param>
        /// <param name="value">When this method returns, contains the key2 associated with the specified key1, if the key1 is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the <see cref="IMap{TKey1, TKey2}"/> contains an element with the specified key1; otherwise, <see langword="false"/>.</returns>
        bool TryGetValueFromKey1(TKey1 key, out TKey2 value);
        /// <summary>
        /// Gets the key1 associated with the specified key2.
        /// </summary>
        /// <param name="key">The key2 of the key1 to get.</param>
        /// <param name="value">When this method returns, contains the key1 associated with the specified key2, if the key2 is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the <see cref="IMap{TKey1, TKey2}"/> contains an element with the specified key2; otherwise, <see langword="false"/>.</returns>
        bool TryGetValueFromKey2(TKey2 key, out TKey1 value);
        /// <summary>
        /// Adds the specified key1 and key2.
        /// </summary>
        /// <param name="key1">The key1 of the element to add.</param>
        /// <param name="key2">The key2 of the element to add.</param>
        void Add(TKey1 key1, TKey2 key2);
        /// <summary>
        /// Sets the specified key1 and key2 related.
        /// </summary>
        /// <param name="key1">The key1 of the element to add.</param>
        /// <param name="key2">The key2 of the element to add.</param>
        void SetPair(TKey1 key1, TKey2 key2);
        /// <summary>
        /// Determines whether the <see cref="IMap{TKey1, TKey2}"/> contains the specified key1.
        /// </summary>
        /// <param name="key">The specified key1.</param>
        /// <returns><see langword="true"/> if it contains; otherwise, <see langword="false"/>.</returns>
        bool ContainsKey1(TKey1 key);
        /// <summary>
        /// Determines whether the <see cref="IMap{TKey1, TKey2}"/> contains the specified key2.
        /// </summary>
        /// <param name="key">The specified key2.</param>
        /// <returns><see langword="true"/> if it contains; otherwise, <see langword="false"/>.</returns>
        bool ContainsKey2(TKey2 key);
        /// <summary>
        /// Determines whether the <see cref="IMap{TKey1, TKey2}"/> contains the element with specified key1 and key2.
        /// </summary>
        /// <param name="key1">The specified key1.</param>
        /// <param name="key2">The specified key2.</param>
        /// <returns><see langword="true"/> if it contains; otherwise, <see langword="false"/>.</returns>
        bool Contains(TKey1 key1, TKey2 key2);
        /// <summary>
        /// Removes an element with specified key1.
        /// </summary>
        /// <param name="key">The specified key1.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        bool RemoveKey1(TKey1 key);
        /// <summary>
        /// Removes an element with specified key2.
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
        /// Get <see cref="Map{TKey1, TKey2}"/> from <see cref="IEnumerable{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">Type of elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey1">Type of key1.</typeparam>
        /// <typeparam name="TKey2">Type of key2.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key1Selector">Selector of key1.</param>
        /// <param name="key2Selector">Selector of key2.</param>
        /// <returns>An instance of <see cref="Map{TKey1, TKey2}"/>.</returns>
        public static Map<TKey1, TKey2> ToMap<TSource, TKey1, TKey2>(this IEnumerable<TSource> source, Func<TSource, TKey1> key1Selector, Func<TSource, TKey2> key2Selector)
            => ToMap(source, key1Selector, key2Selector, null, null);
        /// <summary>
        /// Get <see cref="Map{TKey1, TKey2}"/> from <see cref="IEnumerable{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">Type of elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey1">Type of key1.</typeparam>
        /// <typeparam name="TKey2">Type of key2.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key1Selector">Selector of key1.</param>
        /// <param name="key2Selector">Selector of key2.</param>
        /// <param name="comparer1">Comparer of key1.</param>
        /// <param name="comparer2">Comparer of key2.</param>
        /// <returns>An instance of <see cref="Map{TKey1, TKey2}"/>.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> or <paramref name="key1Selector"/> or <paramref name="key2Selector"/> is <see langword="null"/>.</exception>
        public static Map<TKey1, TKey2> ToMap<TSource, TKey1, TKey2>(this IEnumerable<TSource> source, Func<TSource, TKey1> key1Selector, Func<TSource, TKey2> key2Selector, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
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
            Map<TKey1, TKey2> dictionary = new Map<TKey1, TKey2>(comparer1, comparer2);
            foreach (TSource item in source)
            {
                dictionary.Add(key1Selector(item), key2Selector(item));
            }
            return dictionary;
        }
    }
    /// <summary>
    /// Represents pair with two keys.
    /// </summary>
    /// <typeparam name="TKey1">Type of key1.</typeparam>
    /// <typeparam name="TKey2">Type of key2.</typeparam>
    [Serializable]
    public readonly struct KeyPair<TKey1, TKey2>
    {
        private readonly TKey1 key1;
        private readonly TKey2 key2;
        /// <summary>
        /// First key of the pair.
        /// </summary>
        public TKey1 Key1 => key1;
        /// <summary>
        /// Second key of the pair.
        /// </summary>
        public TKey2 Key2 => key2;
        /// <summary>
        /// Initialize a new instance of <see cref="KeyPair{TKey1, TKey2}"/>.
        /// </summary>
        /// <param name="key1">First key of the pair.</param>
        /// <param name="key2">Second key of the pair.</param>
        public KeyPair(TKey1 key1, TKey2 key2)
        {
            this.key1 = key1;
            this.key2 = key2;
        }
        /// <summary>
        /// Returns a <see cref="string"/> of the two keys.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(16);
            stringBuilder.Append('[');
            if (key1 != null)
            {
                stringBuilder.Append(key1.ToString());
            }
            stringBuilder.Append(", ");
            if (key2 != null)
            {
                stringBuilder.Append(key2.ToString());
            }
            stringBuilder.Append(']');
            return stringBuilder.ToString();
        }
    }
    /// <summary>
    /// Represents a two-way dictionary.
    /// </summary>
    /// <typeparam name="TKey1">Type of key1.</typeparam>
    /// <typeparam name="TKey2">Type of key2.</typeparam>
    [Serializable]
    [DebuggerTypeProxy(typeof(IMapDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    public class Map<TKey1, TKey2> : IMap<TKey1, TKey2>
    {
        private readonly Dictionary<TKey1, TKey2> dic;
        private readonly Dictionary<TKey2, TKey1> rev;
        /// <summary>
        /// Initialize a new instance of <see cref="Map{TKey1, TKey2}"/>.
        /// </summary>
        public Map()
            : this(0, null, null)
        { }
        /// <summary>
        /// Initialize a new instance of <see cref="Map{TKey1, TKey2}"/>.
        /// </summary>
        /// <param name="capacity">The number of elements that the new map can initially store.</param>
        public Map(int capacity)
            : this(capacity, null, null)
        { }
        /// <summary>
        /// Initialize a new instance of <see cref="Map{TKey1, TKey2}"/>.
        /// </summary>
        /// <param name="comparer1">Comparer of key1.</param>
        /// <param name="comparer2">Comparer of key2.</param>
        public Map(IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
            : this(0, comparer1, comparer2)
        { }
        /// <summary>
        /// Initialize a new instance of <see cref="Map{TKey1, TKey2}"/>.
        /// </summary>
        /// <param name="capacity">The number of elements that the new map can initially store.</param>
        /// <param name="comparer1">Comparer of key1.</param>
        /// <param name="comparer2">Comparer of key2.</param>
        public Map(int capacity, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
        {
            this.dic = new Dictionary<TKey1, TKey2>(capacity, comparer1);
            this.rev = new Dictionary<TKey2, TKey1>(capacity, comparer2);
        }
        /// <summary>
        /// Initialize a new instance of <see cref="Map{TKey1, TKey2}"/>.
        /// </summary>
        /// <param name="map">Existed map.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="map"/> is <see langword="null"/>.</exception>
        public Map(IMap<TKey1, TKey2> map)
            : this(map, null, null)
        { }
        /// <summary>
        /// Initialize a new instance of <see cref="Map{TKey1, TKey2}"/>.
        /// </summary>
        /// <param name="map">Existed map.</param>
        /// <param name="comparer1">Comparer of key1.</param>
        /// <param name="comparer2">Comparer of key2.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="map"/> is <see langword="null"/>.</exception>
        public Map(IMap<TKey1, TKey2> map, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
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
        public int Count => dic.Count;
        /// <summary>
        /// Gets a value indicating whether the <see cref="Map{TKey1, TKey2}"/> is read-only.
        /// </summary>
        /// <value><see langword="false"/></value>
        public bool IsReadOnly => false;
        /// <summary>
        /// Gets an <see cref="Dictionary{TKey1, TKey2}.KeyCollection"/> containing the keys in the <see cref="Map{TKey1, TKey2}"/>.
        /// </summary>
        public Dictionary<TKey1, TKey2>.KeyCollection Keys1 => dic.Keys;
        /// <summary>
        /// Gets an <see cref="ICollection{TKey1}"/> containing the keys in the <see cref="Map{TKey1, TKey2}"/>.
        /// </summary>
        ICollection<TKey1> IMap<TKey1, TKey2>.Keys1 => Keys1;
        /// <summary>
        /// Gets an <see cref="Dictionary{TKey2, TKey1}.KeyCollection"/> containing the keys in the <see cref="Map{TKey1, TKey2}"/>.
        /// </summary>
        public Dictionary<TKey2, TKey1>.KeyCollection Keys2 => rev.Keys;
        /// <summary>
        /// Gets an <see cref="ICollection{TKey2}"/> containing the keys in the <see cref="Map{TKey1, TKey2}"/>.
        /// </summary>
        ICollection<TKey2> IMap<TKey1, TKey2>.Keys2 => Keys2;
        /// <summary>
        /// Adds the specified key1 and key2.
        /// </summary>
        /// <param name="key1">The key1 of the element to add.</param>
        /// <param name="key2">The key2 of the element to add.</param>
        public void Add(TKey1 key1, TKey2 key2)
        {
            dic.Add(key1, key2);
            rev.Add(key2, key1);
        }
        /// <summary>
        /// Adds a <see cref="KeyPair{TKey1, TKey2}"/>.
        /// </summary>
        /// <param name="item">The specified key pair.</param>
        public void Add(KeyPair<TKey1, TKey2> item) => Add(item.Key1, item.Key2);
        /// <summary>
        /// Sets the specified key1 and key2 related.
        /// </summary>
        /// <param name="key1">The key1 of the element to add.</param>
        /// <param name="key2">The key2 of the element to add.</param>
        public void SetPair(TKey1 key1, TKey2 key2)
        {
            dic[key1] = key2;
            rev[key2] = key1;
        }
        /// <summary>
        /// Get key2 with the specified key1.
        /// </summary>
        /// <param name="key">The key1 of the key2 to get.</param>
        /// <returns>Key2 related to the specified key1.</returns>
        public TKey2 GetValueFromKey1(TKey1 key) => dic[key];
        /// <summary>
        /// Get key1 with the specified key2.
        /// </summary>
        /// <param name="key">The key2 of the key1 to get.</param>
        /// <returns>Key1 related to the specified key2.</returns>
        public TKey1 GetValueFromKey2(TKey2 key) => rev[key];
        /// <summary>
        /// Gets the key2 associated with the specified key1.
        /// </summary>
        /// <param name="key">The key1 of the key2 to get.</param>
        /// <param name="value">When this method returns, contains the key2 associated with the specified key1, if the key1 is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the <see cref="Map{TKey1, TKey2}"/> contains an element with the specified key1; otherwise, <see langword="false"/>.</returns>
        public bool TryGetValueFromKey1(TKey1 key, out TKey2 value) => dic.TryGetValue(key, out value);
        /// <summary>
        /// Gets the key1 associated with the specified key2.
        /// </summary>
        /// <param name="key">The key2 of the key1 to get.</param>
        /// <param name="value">When this method returns, contains the key1 associated with the specified key2, if the key2 is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the <see cref="Map{TKey1, TKey2}"/> contains an element with the specified key2; otherwise, <see langword="false"/>.</returns>
        public bool TryGetValueFromKey2(TKey2 key, out TKey1 value) => rev.TryGetValue(key, out value);
        /// <summary>
        /// Determines whether the <see cref="Map{TKey1, TKey2}"/> contains the specified key1.
        /// </summary>
        /// <param name="key">The specified key1.</param>
        /// <returns><see langword="true"/> if it contains; otherwise, <see langword="false"/>.</returns>
        public bool ContainsKey1(TKey1 key) => dic.ContainsKey(key);
        /// <summary>
        /// Determines whether the <see cref="Map{TKey1, TKey2}"/> contains the specified key2.
        /// </summary>
        /// <param name="key">The specified key2.</param>
        /// <returns><see langword="true"/> if it contains; otherwise, <see langword="false"/>.</returns>
        public bool ContainsKey2(TKey2 key) => rev.ContainsKey(key);
        /// <summary>
        /// Determines whether the <see cref="Map{TKey1, TKey2}"/> contains the element with specified key1 and key2.
        /// </summary>
        /// <param name="key1">The specified key1.</param>
        /// <param name="key2">The specified key2.</param>
        /// <returns><see langword="true"/> if it contains; otherwise, <see langword="false"/>.</returns>
        public bool Contains(TKey1 key1, TKey2 key2) => dic.ContainsKey(key1) && rev.Comparer.Equals(dic[key1], key2);
        /// <summary>
        /// Determines whether the <see cref="Map{TKey1, TKey2}"/> contains the element.
        /// </summary>
        /// <param name="item">The specified element.</param>
        /// <returns><see langword="true"/> if it contains; otherwise, <see langword="false"/>.</returns>
        public bool Contains(KeyPair<TKey1, TKey2> item) => Contains(item.Key1, item.Key2);
        /// <summary>
        /// Remove element with specified key1.
        /// </summary>
        /// <param name="key">The specified key1.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        public bool RemoveKey1(TKey1 key)
            => dic.TryGetValue(key, out TKey2 value) && dic.Remove(key) && rev.Remove(value);
        /// <summary>
        /// Remove element with specified key2.
        /// </summary>
        /// <param name="key">The specified key2.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        public bool RemoveKey2(TKey2 key)
            => rev.TryGetValue(key, out TKey1 value) && rev.Remove(key) && dic.Remove(value);
        /// <summary>
        /// Remove element with specified key1 and key2.
        /// </summary>
        /// <param name="key1">The specified key1.</param>
        /// <param name="key2">The specified key2.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        public bool Remove(TKey1 key1, TKey2 key2)
            => dic.TryGetValue(key1, out TKey2 value) && rev.Comparer.Equals(value, key2) && dic.Remove(key1) && rev.Remove(key2);
        /// <summary>
        /// Remove an element.
        /// </summary>
        /// <param name="item">The specified element.</param>
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
            foreach (var pair in dic)
            {
                array[arrayIndex] = new KeyPair<TKey1, TKey2>(pair.Key, pair.Value);
                arrayIndex++;
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Map{TKey1, TKey2}"/>.
        /// </summary>
        /// <returns>An enumerator for the <see cref="Map{TKey1, TKey2}"/>.</returns>
        public IEnumerator<KeyPair<TKey1, TKey2>> GetEnumerator()
        {
            foreach (var item in dic)
            {
                yield return new KeyPair<TKey1, TKey2>(item.Key, item.Value);
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Map{TKey1, TKey2}"/>.
        /// </summary>
        /// <returns>An enumerator for the <see cref="Map{TKey1, TKey2}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        /// <summary>
        /// Get <see cref="Dictionary{TKey, TValue}"/> with key1 as key and key2 as value.
        /// </summary>
        /// <returns>An instance of <see cref="Dictionary{TKey, TValue}"/>.</returns>
        public Dictionary<TKey1, TKey2> ToDictionaryFromKey1() => new Dictionary<TKey1, TKey2>(dic);
        /// <summary>
        /// Get <see cref="Dictionary{TKey, TValue}"/> with key2 as key and key1 as value.
        /// </summary>
        /// <returns>An instance of <see cref="Dictionary{TKey, TValue}"/>.</returns>
        public Dictionary<TKey2, TKey1> ToDictionaryFromKey2() => new Dictionary<TKey2, TKey1>(rev);
    }
}
