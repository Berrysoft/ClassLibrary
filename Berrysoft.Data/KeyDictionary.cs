using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berrysoft.Data
{
    #region Interfaces
    public interface IKeyDictionary<TKey1, TKey2> : ICollection<KeyPair<TKey1, TKey2>>
    {
        ICollection<TKey1> Keys1 { get; }
        ICollection<TKey2> Keys2 { get; }
        TKey2 GetValueFromKey1(TKey1 key);
        TKey1 GetValueFromKey2(TKey2 key);
        bool TryGetValueFromKey1(TKey1 key, out TKey2 value);
        bool TryGetValueFromKey2(TKey2 key, out TKey1 value);
        void Add(TKey1 key1, TKey2 key2);
        bool SetPair(TKey1 key1, TKey2 key2);
        bool ContainsKey1(TKey1 key);
        bool ContainsKey2(TKey2 key);
        bool Contains(TKey1 key1, TKey2 key2);
        bool RemoveKey1(TKey1 key);
        bool RemoveKey2(TKey2 key);
        bool Remove(TKey1 key1, TKey2 key2);
    }
    #endregion
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
    public class KeyDictionary<TKey1, TKey2> : IKeyDictionary<TKey1, TKey2>
    {
        private List<KeyPair<TKey1, TKey2>> list;
        private IEqualityComparer<TKey1> comparer1;
        private IEqualityComparer<TKey2> comparer2;
        private Key1Collection keys1;
        private Key2Collection keys2;
        public KeyDictionary()
            : this(0, null, null)
        { }
        public KeyDictionary(int capacity)
            : this(capacity, null, null)
        { }
        public KeyDictionary(IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
            : this(0, comparer1, comparer2)
        { }
        public KeyDictionary(int capacity, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
        {
            this.list = new List<KeyPair<TKey1, TKey2>>(capacity);
            this.comparer1 = comparer1 ?? EqualityComparer<TKey1>.Default;
            this.comparer2 = comparer2 ?? EqualityComparer<TKey2>.Default;
        }
        public KeyDictionary(IKeyDictionary<TKey1, TKey2> dictionary)
            : this(dictionary, null, null)
        { }
        public KeyDictionary(IKeyDictionary<TKey1, TKey2> dictionary, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
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
        public Key1Collection Keys1
        {
            get
            {
                if (keys1 == null)
                {
                    keys1 = new Key1Collection(this);
                }
                return keys1;
            }
        }
        public Key2Collection Keys2
        {
            get
            {
                if (keys2 == null)
                {
                    keys2 = new Key2Collection(this);
                }
                return keys2;
            }
        }
        ICollection<TKey1> IKeyDictionary<TKey1, TKey2>.Keys1 => Keys1;
        ICollection<TKey2> IKeyDictionary<TKey1, TKey2>.Keys2 => Keys2;
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
                        throw new ArgumentException("Key1 is existed.");
                    }
                    index1 = i;
                }
                if (comparer2.Equals(list[i].Key2, key2))
                {
                    if (add)
                    {
                        throw new ArgumentException("Key2 is existed.");
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
                    throw new ArgumentException("The key pair is existed.");
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
                    throw new ArgumentException("The keys are existed.");
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
                throw new KeyNotFoundException();
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
                throw new KeyNotFoundException();
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
        public bool RemoveKey1(TKey1 key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
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
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
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
            if (key1 == null)
            {
                throw new ArgumentNullException(nameof(key1));
            }
            if (key2 == null)
            {
                throw new ArgumentNullException(nameof(key2));
            }
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
        #region Collections
        public sealed class Key1Collection : ICollection<TKey1>
        {
            private KeyDictionary<TKey1, TKey2> dictionary;
            public Key1Collection(KeyDictionary<TKey1,TKey2> dictionary)
            {
                this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }
            public int Count => dictionary.Count;
            public bool IsReadOnly => true;
            public void Add(TKey1 item) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();
            public bool Contains(TKey1 item) => dictionary.ContainsKey1(item);
            public bool Remove(TKey1 item) => throw new NotSupportedException();
            public void CopyTo(TKey1[] array, int arrayIndex)
            {
                for (int i = 0; i < Count; i++)
                {
                    array[arrayIndex++] = dictionary.list[i].Key1;
                }
            }
            public IEnumerator<TKey1> GetEnumerator() => dictionary.list.Select(pair => pair.Key1).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        public sealed class Key2Collection : ICollection<TKey2>
        {
            private KeyDictionary<TKey1, TKey2> dictionary;
            public Key2Collection(KeyDictionary<TKey1, TKey2> dictionary)
            {
                this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }
            public int Count => dictionary.Count;
            public bool IsReadOnly => true;
            public void Add(TKey2 item) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();
            public bool Contains(TKey2 item) => dictionary.ContainsKey2(item);
            public bool Remove(TKey2 item) => throw new NotSupportedException();
            public void CopyTo(TKey2[] array, int arrayIndex)
            {
                for (int i = 0; i < Count; i++)
                {
                    array[arrayIndex++] = dictionary.list[i].Key2;
                }
            }
            public IEnumerator<TKey2> GetEnumerator() => dictionary.list.Select(pair => pair.Key2).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        #endregion
    }
}
