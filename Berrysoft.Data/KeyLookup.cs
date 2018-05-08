using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Berrysoft.Data
{
    #region Interfaces
    public interface IKeyLookup<TKey1, TKey2> : ICollection<KeyPair<TKey1, TKey2>>
    {
        IEnumerable<TKey1> Keys1 { get; }
        IEnumerable<TKey2> Keys2 { get; }
        IEnumerable<TKey2> GetValuesFromKey1(TKey1 key);
        IEnumerable<TKey1> GetValuesFromKey2(TKey2 key);
        bool TryGetValuesFromKey1(TKey1 key, out IEnumerable<TKey2> values);
        bool TryGetValuesFromKey2(TKey2 key, out IEnumerable<TKey1> values);
        void Add(TKey1 key1, TKey2 key2);
        void SetPair(TKey1 key1, TKey2 key2);
        bool ContainsKey1(TKey1 key);
        bool ContainsKey2(TKey2 key);
        bool Contains(TKey1 key1, TKey2 key2);
        bool RemoveKey1(TKey1 key);
        bool RemoveKey2(TKey2 key);
        bool Remove(TKey1 key1, TKey2 key2);
        ILookup<TKey1, TKey2> ToLookupFromKey1();
        ILookup<TKey2, TKey1> ToLookupFromKey2();
    }
    #endregion
    public class KeyLookup<TKey1, TKey2> : IKeyLookup<TKey1, TKey2>
    {
        List<KeyPair<TKey1, TKey2>> list;
        private IEqualityComparer<TKey1> comparer1;
        private IEqualityComparer<TKey2> comparer2;
        public KeyLookup()
            : this(0, null, null)
        { }
        public KeyLookup(int capacity)
            : this(capacity, null, null)
        { }
        public KeyLookup(IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
            : this(0, comparer1, comparer2)
        { }
        public KeyLookup(int capacity, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
        {
            this.list = new List<KeyPair<TKey1, TKey2>>(capacity);
            this.comparer1 = comparer1 ?? EqualityComparer<TKey1>.Default;
            this.comparer2 = comparer2 ?? EqualityComparer<TKey2>.Default;
        }
        public KeyLookup(IKeyLookup<TKey1, TKey2> dictionary)
            : this(dictionary, null, null)
        { }
        public KeyLookup(IKeyLookup<TKey1, TKey2> dictionary, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
            : this(dictionary != null ? dictionary.Count : 0, comparer1, comparer2)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            foreach (var item in dictionary)
            {
                Add(item.Key1, item.Key2);
            }
        }
        public int Count => list.Count;
        public bool IsReadOnly => false;
        public IEnumerable<TKey1> Keys1 => list.Select(pair => pair.Key1).Distinct(comparer1);
        public IEnumerable<TKey2> Keys2 => list.Select(pair => pair.Key2).Distinct(comparer2);
        private void Insert(TKey1 key1, TKey2 key2, bool add)
        {
            if (Contains(key1, key2))
            {
                if (add)
                {
                    throw new ArgumentException("The key pair is existed.");
                }
            }
            else
            {
                list.Add(new KeyPair<TKey1, TKey2>(key1, key2));
            }
        }
        public void Add(TKey1 key1, TKey2 key2) => Insert(key1, key2, true);
        public void Add(KeyPair<TKey1, TKey2> item) => Add(item.Key1, item.Key2);
        public void SetPair(TKey1 key1, TKey2 key2) => Insert(key1, key2, false);
        public IEnumerable<TKey2> GetValuesFromKey1(TKey1 key)
        {
            if (TryGetValuesFromKey1(key, out var result))
            {
                return result;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
        public IEnumerable<TKey1> GetValuesFromKey2(TKey2 key)
        {
            if (TryGetValuesFromKey2(key, out var result))
            {
                return result;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
        public bool TryGetValuesFromKey1(TKey1 key, out IEnumerable<TKey2> values)
        {
            if (ContainsKey1(key))
            {
                values = list.Where(pair => comparer1.Equals(pair.Key1, key)).Select(pair => pair.Key2);
                return true;
            }
            else
            {
                values = null;
                return false;
            }
        }
        public bool TryGetValuesFromKey2(TKey2 key, out IEnumerable<TKey1> values)
        {
            if (ContainsKey2(key))
            {
                values = list.Where(pair => comparer2.Equals(pair.Key2, key)).Select(pair => pair.Key1);
                return true;
            }
            else
            {
                values = null;
                return false;
            }
        }
        public bool ContainsKey1(TKey1 key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return list.FindIndex(pair => comparer1.Equals(pair.Key1, key)) >= 0;
        }
        public bool ContainsKey2(TKey2 key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return list.FindIndex(pair => comparer2.Equals(pair.Key2, key)) >= 0;
        }
        public bool Contains(TKey1 key1, TKey2 key2)
        {
            if (key1 == null)
            {
                throw new ArgumentNullException(nameof(key1));
            }
            if (key2 == null)
            {
                throw new ArgumentNullException(nameof(key2));
            }
            return list.FindIndex(pair => comparer1.Equals(pair.Key1, key1) && comparer2.Equals(pair.Key2, key2)) >= 0;
        }
        public bool Contains(KeyPair<TKey1, TKey2> item) => Contains(item.Key1, item.Key2);
        public void CopyTo(KeyPair<TKey1, TKey2>[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        public bool RemoveKey1(TKey1 key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (list.RemoveAll(pair => comparer1.Equals(pair.Key1, key)) > 0)
            {
                return true;
            }
            return false;
        }
        public bool RemoveKey2(TKey2 key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (list.RemoveAll(pair => comparer2.Equals(pair.Key2, key)) > 0)
            {
                return true;
            }
            return false;
        }
        public bool Remove(TKey1 key1, TKey2 key2)
        {
            if (key1 == null)
            {
                throw new ArgumentNullException(nameof(key1));
            }
            if (key2 == null)
            {
                throw new ArgumentNullException(nameof(key2));
            }
            int index = list.FindIndex(pair => comparer1.Equals(pair.Key1, key1) && comparer2.Equals(pair.Key2, key2));
            if (index >= 0)
            {
                list.RemoveAt(index);
                return true;
            }
            return false;
        }
        public bool Remove(KeyPair<TKey1, TKey2> item) => Remove(item.Key1, item.Key2);
        public void Clear() => list.Clear();
        public IEnumerator<KeyPair<TKey1, TKey2>> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public ILookup<TKey1, TKey2> ToLookupFromKey1()
        {
            return list.ToLookup(pair => pair.Key1, pair => pair.Key2, comparer1);
        }
        public ILookup<TKey2, TKey1> ToLookupFromKey2()
        {
            return list.ToLookup(pair => pair.Key2, pair => pair.Key1, comparer2);
        }
    }
}
