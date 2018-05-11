using System;
using System.Collections.Generic;
using System.Linq;

namespace Berrysoft.Data
{
    public static class Enumerable
    {
        #region Tree
        public static IEnumerable<TNode> AsDFSEnumerable<TValue, TNode>(this ITree<TValue, TNode> tree)
            where TNode : INodeBase<TValue, TNode>
        {
            if (tree == null)
            {
                throw new ArgumentNullException(nameof(tree));
            }
            return AsDFSEnumerableIterator(tree);
        }
        private static IEnumerable<TNode> AsDFSEnumerableIterator<TValue, TNode>(ITree<TValue, TNode> tree)
            where TNode : INodeBase<TValue, TNode>
        {
            Stack<TNode> nodes = new Stack<TNode>();
            nodes.Push(tree.Root);
            while (nodes.Count != 0)
            {
                TNode current = nodes.Pop();
                yield return current;
                foreach (var child in current.AsEnumerable().Reverse())
                {
                    nodes.Push(child);
                }
            }
        }
        public static IEnumerable<TNode> AsBFSEnumerable<TValue, TNode>(this ITree<TValue, TNode> tree)
            where TNode : INodeBase<TValue, TNode>
        {
            if (tree == null)
            {
                throw new ArgumentNullException(nameof(tree));
            }
            return AsBFSEnumerableIterator(tree);
        }
        private static IEnumerable<TNode> AsBFSEnumerableIterator<TValue, TNode>(ITree<TValue, TNode> tree)
            where TNode : INodeBase<TValue, TNode>
        {
            Queue<TNode> nodes = new Queue<TNode>();
            nodes.Enqueue(tree.Root);
            while (nodes.Count != 0)
            {
                TNode current = nodes.Dequeue();
                yield return current;
                foreach (var child in current.AsEnumerable())
                {
                    nodes.Enqueue(child);
                }
            }
        }
        #endregion
        #region Key
        public static KeyDictionary<TKey1, TKey2> ToKeyDictionary<TSource, TKey1, TKey2>(this IEnumerable<TSource> source, Func<TSource, TKey1> key1Selector, Func<TSource, TKey2> key2Selector)
            => ToKeyDictionary(source, key1Selector, key2Selector, null, null);
        public static KeyDictionary<TKey1, TKey2> ToKeyDictionary<TSource, TKey1, TKey2>(this IEnumerable<TSource> source, Func<TSource, TKey1> key1Selector, Func<TSource, TKey2> key2Selector, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (key1Selector == null)
            {
                throw new ArgumentNullException(nameof(key1Selector));
            }
            if (key2Selector == null)
            {
                throw new ArgumentNullException(nameof(key2Selector));
            }
            KeyDictionary<TKey1, TKey2> dictionary = new KeyDictionary<TKey1, TKey2>(comparer1, comparer2);
            foreach (TSource item in source)
            {
                dictionary.Add(key1Selector(item), key2Selector(item));
            }
            return dictionary;
        }
        public static KeyLookup<TKey1, TKey2> ToKeyLookup<TSource, TKey1, TKey2>(this IEnumerable<TSource> source, Func<TSource, TKey1> key1Selector, Func<TSource, TKey2> key2Selector)
            => ToKeyLookup(source, key1Selector, key2Selector, null, null);
        public static KeyLookup<TKey1, TKey2> ToKeyLookup<TSource, TKey1, TKey2>(this IEnumerable<TSource> source, Func<TSource, TKey1> key1Selector, Func<TSource, TKey2> key2Selector, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (key1Selector == null)
            {
                throw new ArgumentNullException(nameof(key1Selector));
            }
            if (key2Selector == null)
            {
                throw new ArgumentNullException(nameof(key2Selector));
            }
            KeyLookup<TKey1, TKey2> lookup = new KeyLookup<TKey1, TKey2>(comparer1, comparer2);
            foreach (TSource item in source)
            {
                lookup.Add(key1Selector(item), key2Selector(item));
            }
            return lookup;
        }
        #endregion
        #region Linq
        public static IEnumerable<TResult> SelectWhen<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, (bool Select, TResult Result)> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return SelectWhenIterator(source, predicate);
        }
        private static IEnumerable<TResult> SelectWhenIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, (bool Select, TResult Result)> predicate)
        {
            foreach (TSource item in source)
            {
                var (Select, Result) = predicate(item);
                if (Select)
                {
                    yield return Result;
                }
            }
        }
        #endregion
    }
}
