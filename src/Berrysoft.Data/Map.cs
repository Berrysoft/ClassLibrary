using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        /// Gets an <see cref="IEnumerable{TKey1}"/> containing the keys in the <see cref="IMap{TKey1, TKey2}"/>.
        /// </summary>
        IEnumerable<TKey1> Keys1 { get; }
        /// <summary>
        /// Gets an <see cref="IEnumerable{TKey2}"/> containing the keys in the <see cref="IMap{TKey1, TKey2}"/>.
        /// </summary>
        IEnumerable<TKey2> Keys2 { get; }
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
        /// <returns><see langword="true"/> if key1 or key2 is/are not contained in the <see cref="IMap{TKey1, TKey2}"/> and set successfully; otherwise, <see langword="false"/>.</returns>
        bool SetPair(TKey1 key1, TKey2 key2);
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
        /// Remove element with specified key1.
        /// </summary>
        /// <param name="key">The specified key1.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        bool RemoveKey1(TKey1 key);
        /// <summary>
        /// Remove element with specified key2.
        /// </summary>
        /// <param name="key">The specified key2.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        bool RemoveKey2(TKey2 key);
        /// <summary>
        /// Remove element with specified key1 and key2.
        /// </summary>
        /// <param name="key1">The specified key1.</param>
        /// <param name="key2">The specified key2.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        bool Remove(TKey1 key1, TKey2 key2);
    }
    #endregion
    /// <summary>
    /// Represents pair with two keys.
    /// </summary>
    /// <typeparam name="TKey1">Type of key1.</typeparam>
    /// <typeparam name="TKey2">Type of key2.</typeparam>
    public struct KeyPair<TKey1, TKey2>
    {
        private TKey1 key1;
        private TKey2 key2;
        public TKey1 Key1 => key1;
        public TKey2 Key2 => key2;
        public KeyPair(TKey1 key1, TKey2 key2)
        {
            this.key1 = key1;
            this.key2 = key2;
        }
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
    public class Map<TKey1, TKey2> : IMap<TKey1, TKey2>
    {
        private List<KeyPair<TKey1, TKey2>> list;
        private IEqualityComparer<TKey1> comparer1;
        private IEqualityComparer<TKey2> comparer2;
        public Map()
            : this(0, null, null)
        { }
        public Map(int capacity)
            : this(capacity, null, null)
        { }
        public Map(IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
            : this(0, comparer1, comparer2)
        { }
        public Map(int capacity, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
        {
            this.list = new List<KeyPair<TKey1, TKey2>>(capacity);
            this.comparer1 = comparer1 ?? EqualityComparer<TKey1>.Default;
            this.comparer2 = comparer2 ?? EqualityComparer<TKey2>.Default;
        }
        public Map(IMap<TKey1, TKey2> dictionary)
            : this(dictionary, null, null)
        { }
        public Map(IMap<TKey1, TKey2> dictionary, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
            : this(dictionary != null ? dictionary.Count : 0, comparer1, comparer2)
        {
            if (dictionary == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(dictionary));
            }
            foreach (var item in dictionary)
            {
                Add(item.Key1, item.Key2);
            }
        }
        public int Count => list.Count;
        public bool IsReadOnly => false;
        public IEnumerable<TKey1> Keys1 => list.Select(pair => pair.Key1);
        public IEnumerable<TKey2> Keys2 => list.Select(pair => pair.Key2);
        private bool Insert(TKey1 key1, TKey2 key2, bool add)
        {
            int index1 = -1;
            int index2 = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (comparer1.Equals(list[i].Key1, key1))
                {
                    if (add)
                    {
                        throw ExceptionHelper.KeyExisted($"Key1 {key1}");
                    }
                    index1 = i;
                }
                if (comparer2.Equals(list[i].Key2, key2))
                {
                    if (add)
                    {
                        throw ExceptionHelper.KeyExisted($"Key2 {key2}");
                    }
                    index2 = i;
                }
            }
            if (index1 == index2)
            {
                if (index1 == -1)
                {
                    list.Add(new KeyPair<TKey1, TKey2>(key1, key2));
                }
                else if (add)
                {
                    throw ExceptionHelper.PairExisted();
                }
            }
            else
            {
                if (index1 == -1)
                {
                    list[index2] = new KeyPair<TKey1, TKey2>(key1, key2);
                }
                else if (index2 == -1)
                {
                    list[index1] = new KeyPair<TKey1, TKey2>(key1, key2);
                }
                else if (add)
                {
                    throw ExceptionHelper.KeysExisted();
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        public void Add(TKey1 key1, TKey2 key2) => Insert(key1, key2, true);
        public void Add(KeyPair<TKey1, TKey2> item) => Insert(item.Key1, item.Key2, true);
        public bool SetPair(TKey1 key1, TKey2 key2) => Insert(key1, key2, false);
        public TKey2 GetValueFromKey1(TKey1 key)
        {
            if (TryGetValueFromKey1(key, out var value))
            {
                return value;
            }
            else
            {
                throw ExceptionHelper.KeyNotFound();
            }
        }
        public TKey1 GetValueFromKey2(TKey2 key)
        {
            if (TryGetValueFromKey2(key, out var value))
            {
                return value;
            }
            else
            {
                throw ExceptionHelper.KeyNotFound();
            }
        }
        public bool TryGetValueFromKey1(TKey1 key, out TKey2 value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (comparer1.Equals(list[i].Key1, key))
                {
                    value = list[i].Key2;
                    return true;
                }
            }
            value = default;
            return false;
        }
        public bool TryGetValueFromKey2(TKey2 key, out TKey1 value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (comparer2.Equals(list[i].Key2, key))
                {
                    value = list[i].Key1;
                    return true;
                }
            }
            value = default;
            return false;
        }
        public bool ContainsKey1(TKey1 key)
        {
            if (key == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(key));
            }
            return list.FindIndex(pair => comparer1.Equals(pair.Key1, key)) >= 0;
        }
        public bool ContainsKey2(TKey2 key)
        {
            if (key == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(key));
            }
            return list.FindIndex(pair => comparer2.Equals(pair.Key2, key)) >= 0;
        }
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
            return list.FindIndex(pair => comparer1.Equals(pair.Key1, key1) && comparer2.Equals(pair.Key2, key2)) >= 0;
        }
        public bool Contains(KeyPair<TKey1, TKey2> item) => Contains(item.Key1, item.Key2);
        public bool RemoveKey1(TKey1 key)
        {
            int index = list.FindIndex(pair => comparer1.Equals(pair.Key1, key));
            if (index < 0)
            {
                return false;
            }
            else
            {
                list.RemoveAt(index);
                return true;
            }
        }
        public bool RemoveKey2(TKey2 key)
        {
            int index = list.FindIndex(pair => comparer2.Equals(pair.Key2, key));
            if (index < 0)
            {
                return false;
            }
            else
            {
                list.RemoveAt(index);
                return true;
            }
        }
        public bool Remove(TKey1 key1, TKey2 key2)
        {
            int index = list.FindIndex(pair => comparer1.Equals(pair.Key1, key1) && comparer2.Equals(pair.Key2, key2));
            if (index < 0)
            {
                return false;
            }
            else
            {
                list.RemoveAt(index);
                return true;
            }
        }
        public bool Remove(KeyPair<TKey1, TKey2> item) => Remove(item.Key1, item.Key2);
        public void Clear() => list.Clear();
        public void CopyTo(KeyPair<TKey1, TKey2>[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        public IEnumerator<KeyPair<TKey1, TKey2>> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)list).GetEnumerator();
        public IDictionary<TKey1, TKey2> ToDictionaryFromKey1() => list.ToDictionary(pair => pair.Key1, pair => pair.Key2, comparer1);
        public IDictionary<TKey2, TKey1> ToDictionaryFromKey2() => list.ToDictionary(pair => pair.Key2, pair => pair.Key1, comparer2);
    }
}
