using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Berrysoft.Data
{
    #region Interfaces
    public interface IMultiMap<TKey1, TKey2> : ICollection<KeyPair<TKey1, TKey2>>
    {
        ICollection<TKey1> Keys1 { get; }
        ICollection<TKey2> Keys2 { get; }
        ICollection<TKey2> GetValuesFromKey1(TKey1 key);
        ICollection<TKey1> GetValuesFromKey2(TKey2 key);
        bool TryGetValuesFromKey1(TKey1 key, out ICollection<TKey2> values);
        bool TryGetValuesFromKey2(TKey2 key, out ICollection<TKey1> values);
        void Add(TKey1 key1, TKey2 key2);
        bool TryAdd(TKey1 key1, TKey2 key2);
        bool ContainsKey1(TKey1 key);
        bool ContainsKey2(TKey2 key);
        bool Contains(TKey1 key1, TKey2 key2);
        bool RemoveKey1(TKey1 key);
        bool RemoveKey2(TKey2 key);
        bool Remove(TKey1 key1, TKey2 key2);
    }
    #endregion
    public static partial class Enumerable
    {
        public static MultiMap<TKey1, TKey2> ToMultiMap<TSource, TKey1, TKey2>(this IEnumerable<TSource> source, Func<TSource, TKey1> key1Selector, Func<TSource, TKey2> key2Selector)
            => ToMultiMap(source, key1Selector, key2Selector, null, null);
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
    public class MultiMap<TKey1, TKey2> : IMultiMap<TKey1, TKey2>
    {
        private Dictionary<TKey1, HashSet<TKey2>> lkp;
        private Dictionary<TKey2, HashSet<TKey1>> rev;
        public MultiMap()
            : this(0, null, null)
        { }
        public MultiMap(int capacity)
            : this(capacity, null, null)
        { }
        public MultiMap(IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
            : this(0, comparer1, comparer2)
        { }
        public MultiMap(int capacity, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
        {
            this.lkp = new Dictionary<TKey1, HashSet<TKey2>>(capacity, comparer1);
            this.rev = new Dictionary<TKey2, HashSet<TKey1>>(capacity, comparer2);
        }
        public MultiMap(IMultiMap<TKey1, TKey2> dictionary)
            : this(dictionary, null, null)
        { }
        public MultiMap(IMultiMap<TKey1, TKey2> dictionary, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
            : this(dictionary != null ? dictionary.Count : 0, comparer1, comparer2)
        {
            foreach (var item in dictionary ?? throw ExceptionHelper.ArgumentNull(nameof(dictionary)))
            {
                Add(item.Key1, item.Key2);
            }
        }
        public int Count => lkp.Count;
        public bool IsReadOnly => false;
        public Dictionary<TKey1, HashSet<TKey2>>.KeyCollection Keys1 => lkp.Keys;
        public Dictionary<TKey2, HashSet<TKey1>>.KeyCollection Keys2 => rev.Keys;
        ICollection<TKey1> IMultiMap<TKey1, TKey2>.Keys1 => Keys1;
        ICollection<TKey2> IMultiMap<TKey1, TKey2>.Keys2 => Keys2;
        private bool Insert(TKey1 key1, TKey2 key2, bool add)
        {
            if (key1 == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(key1));
            }
            if (key2 == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(key2));
            }
            if (lkp.ContainsKey(key1))
            {
                if (!lkp[key1].Add(key2))
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
            }
            else
            {
                lkp[key1] = new HashSet<TKey2>();
                lkp[key1].Add(key2);
            }
            if (rev.ContainsKey(key2))
            {
                rev[key2].Add(key1);
            }
            else
            {
                rev[key2] = new HashSet<TKey1>();
                rev[key2].Add(key1);
            }
            return true;
        }
        public void Add(TKey1 key1, TKey2 key2) => Insert(key1, key2, true);
        public void Add(KeyPair<TKey1, TKey2> item) => Add(item.Key1, item.Key2);
        public bool TryAdd(TKey1 key1, TKey2 key2) => Insert(key1, key2, false);
        public ICollection<TKey2> GetValuesFromKey1(TKey1 key) => lkp[key];
        public ICollection<TKey1> GetValuesFromKey2(TKey2 key) => rev[key];
        public bool TryGetValuesFromKey1(TKey1 key, out ICollection<TKey2> values)
        {
            bool result = lkp.TryGetValue(key, out var value);
            values = value;
            return result;
        }
        public bool TryGetValuesFromKey2(TKey2 key, out ICollection<TKey1> values)
        {
            bool result = rev.TryGetValue(key, out var value);
            values = value;
            return result;
        }
        public bool ContainsKey1(TKey1 key) => lkp.ContainsKey(key);
        public bool ContainsKey2(TKey2 key) => rev.ContainsKey(key);
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
            return lkp.ContainsKey(key1) && lkp[key1].Contains(key2);
        }
        public bool Contains(KeyPair<TKey1, TKey2> item) => Contains(item.Key1, item.Key2);
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
            if (array.Length - arrayIndex < lkp.Count)
            {
                throw ExceptionHelper.ArrayTooSmall();
            }
            foreach(var item in this)
            {
                array[arrayIndex++] = item;
            }
        }
        public bool RemoveKey1(TKey1 key)
        {
            HashSet<TKey2> values = lkp[key];
            if (lkp.Remove(key))
            {
                foreach (TKey2 value in values)
                {
                    rev[value].Remove(key);
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
        public bool RemoveKey2(TKey2 key)
        {
            HashSet<TKey1> values = rev[key];
            if (rev.Remove(key))
            {
                foreach (TKey1 value in values)
                {
                    lkp[value].Remove(key);
                    if (lkp[value].Count == 0)
                    {
                        lkp.Remove(value);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Remove(TKey1 key1, TKey2 key2)
        {
            if (lkp[key1].Remove(key2) && rev[key2].Remove(key1))
            {
                if (lkp[key1].Count == 0)
                {
                    if (!lkp.Remove(key1))
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
        public bool Remove(KeyPair<TKey1, TKey2> item) => Remove(item.Key1, item.Key2);
        public void Clear()
        {
            lkp.Clear();
            rev.Clear();
        }
        public IEnumerator<KeyPair<TKey1, TKey2>> GetEnumerator()
        {
            foreach (var item in lkp)
            {
                foreach (var value in item.Value)
                {
                    yield return new KeyPair<TKey1, TKey2>(item.Key, value);
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public ILookup<TKey1, TKey2> ToLookupFromKey1() => new Lookup<TKey1, TKey2>(lkp);
        public ILookup<TKey2, TKey1> ToLookupFromKey2() => new Lookup<TKey2, TKey1>(rev);
    }
}
