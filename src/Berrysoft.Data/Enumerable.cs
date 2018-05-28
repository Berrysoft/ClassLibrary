﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Berrysoft.Data
{
    /// <summary>
    /// Provides a set of static (Shared in Visual Basic) methods for querying objects.
    /// </summary>
    public static class Enumerable
    {
        #region Tree
        /// <summary>
        /// Get depth of the tree.
        /// </summary>
        /// <typeparam name="TValue">The type of value the node contains.</typeparam>
        /// <typeparam name="TNode">The type of node.</typeparam>
        /// <param name="tree">A tree to calculte depth.</param>
        /// <returns>The depth of the tree.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="tree"/> is null.</exception>
        public static int GetDepth<TValue, TNode>(this ITree<TValue, TNode> tree)
            where TNode : INodeBase<TValue, TNode>
        {
            int GetDepthInternal(TNode node, int depth)
            {
                int result = depth;
                foreach (TNode child in node.AsEnumerable())
                {
                    int tempDepth = GetDepthInternal(child, depth + 1);
                    if (tempDepth > result)
                    {
                        result = tempDepth;
                    }
                }
                return result;
            }
            switch (tree ?? throw ExceptionHelper.ArgumentNull(nameof(tree)))
            {
                case Tree<TValue> simpleTree:
                    return simpleTree.GetDepth();
                case BinaryTree<TValue> binaryTree:
                    return binaryTree.GetDepth();
                default:
                    return GetDepthInternal(tree.Root, 1);
            }
        }
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with order of depth-first-search.
        /// </summary>
        /// <typeparam name="TValue">The type of value the node contains.</typeparam>
        /// <typeparam name="TNode">The type of node.</typeparam>
        /// <param name="tree">A tree to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of depth-first-search.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="tree"/> is null.</exception>
        public static IEnumerable<TNode> AsDFSEnumerable<TValue, TNode>(this ITree<TValue, TNode> tree)
            where TNode : INodeBase<TValue, TNode>
        {
            switch (tree ?? throw ExceptionHelper.ArgumentNull(nameof(tree)))
            {
                case BinaryTree<TValue> binaryTree:
                    return (IEnumerable<TNode>)binaryTree.AsDFSEnumerable();
                default:
                    return AsDFSEnumerableIterator(tree);
            }
        }
        /// <summary>
        /// Get an iterator with order of depth-first-search.
        /// </summary>
        /// <typeparam name="TValue">The type of value the node contains.</typeparam>
        /// <typeparam name="TNode">The type of node.</typeparam>
        /// <param name="tree">A tree to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of depth-first-search.</returns>
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
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with order of breadth-first-search.
        /// </summary>
        /// <typeparam name="TValue">The type of value the node contains.</typeparam>
        /// <typeparam name="TNode">The type of node.</typeparam>
        /// <param name="tree">A tree to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of breadth-first-search.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="tree"/> is null.</exception>
        public static IEnumerable<TNode> AsBFSEnumerable<TValue, TNode>(this ITree<TValue, TNode> tree)
            where TNode : INodeBase<TValue, TNode>
        {
            switch (tree ?? throw ExceptionHelper.ArgumentNull(nameof(tree)))
            {
                case BinaryTree<TValue> binaryTree:
                    return (IEnumerable<TNode>)binaryTree.AsBFSEnumerable();
                default:
                    return AsBFSEnumerableIterator(tree);
            }
        }
        /// <summary>
        /// Get an iterator with order of breadth-first-search.
        /// </summary>
        /// <typeparam name="TValue">The type of value the node contains.</typeparam>
        /// <typeparam name="TNode">The type of node.</typeparam>
        /// <param name="tree">A tree to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of breadth-first-search.</returns>
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
        #region Graph
        public static IEnumerable<T> AsDFSEnumerable<T>(this IGraph<T> graph, T root)
        {
            if (graph == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(graph));
            }
            if (!graph.Contains(root))
            {
                throw ExceptionHelper.KeyNotFound();
            }
            return AsDFSEnumerableIterator(graph, root);
        }
        private static IEnumerable<T> AsDFSEnumerableIterator<T>(IGraph<T> graph, T root)
        {
            Stack<T> nodes = new Stack<T>();
            HashSet<T> visited = new HashSet<T>();
            nodes.Push(root);
            while (nodes.Count != 0)
            {
                T current;
                do
                {
                    if (nodes.Count == 0)
                    {
                        yield break;
                    }
                    current = nodes.Pop();
                }
                while (visited.Contains(current));
                visited.Add(current);
                yield return current;
                if (graph.TryGetHeads(current, out var heads))
                {
                    foreach (var child in heads.Reverse())
                    {
                        if (!visited.Contains(child))
                        {
                            nodes.Push(child);
                        }
                    }
                }
            }
        }
        public static IEnumerable<T> AsBFSEnumerable<T>(this IGraph<T> graph, T root)
        {
            if (graph == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(graph));
            }
            if (!graph.Contains(root))
            {
                throw ExceptionHelper.KeyNotFound();
            }
            return AsBFSEnumerableIterator(graph, root);
        }
        private static IEnumerable<T> AsBFSEnumerableIterator<T>(IGraph<T> graph, T root)
        {
            Queue<T> nodes = new Queue<T>();
            HashSet<T> visited = new HashSet<T>();
            nodes.Enqueue(root);
            while (nodes.Count != 0)
            {
                T current;
                do
                {
                    if (nodes.Count == 0)
                    {
                        yield break;
                    }
                    current = nodes.Dequeue();
                }
                while (visited.Contains(current));
                visited.Add(current);
                yield return current;
                if (graph.TryGetHeads(current, out var heads))
                {
                    foreach (var child in heads)
                    {
                        if (!visited.Contains(child))
                        {
                            nodes.Enqueue(child);
                        }
                    }
                }
            }
        }
        public static Tree<T> ToDFSTree<T>(this Graph<T> graph, T root)
            => ToDFSTree<T, Tree<T>, Node<T>>(graph, root);
        public static TTree ToDFSTree<TValue, TTree, TNode>(this IGraph<TValue> graph, TValue root)
            where TTree : ITree<TValue, TNode>, new()
            where TNode : INode<TValue, TNode>, new()
        {
            if (graph == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(graph));
            }
            if (!graph.Contains(root))
            {
                throw ExceptionHelper.KeyNotFound();
            }
            TTree result = new TTree();
            result.Root.Value = root;
            Stack<TNode> nodes = new Stack<TNode>();
            HashSet<TValue> visited = new HashSet<TValue>();
            nodes.Push(result.Root);
            while (nodes.Count != 0)
            {
                TNode current;
                for (; ; )
                {
                    if (nodes.Count == 0)
                    {
                        goto ret;
                    }
                    current = nodes.Pop();
                    if (visited.Contains(current.Value))
                    {
                        current.Parent.Remove(current);
                        continue;
                    }
                    break;
                }
                visited.Add(current.Value);
                if (graph.TryGetHeads(current.Value, out var heads))
                {
                    foreach (var child in heads.Reverse())
                    {
                        if (!visited.Contains(child))
                        {
                            TNode nc = new TNode();
                            nc.Value = child;
                            nodes.Push(nc);
                            current.Add(nc);
                        }
                    }
                }
            }
            ret:
            return result;
        }
        public static Tree<T> ToBFSTree<T>(this Graph<T> graph, T root)
            => ToBFSTree<T, Tree<T>, Node<T>>(graph, root);
        public static TTree ToBFSTree<TValue, TTree, TNode>(this IGraph<TValue> graph, TValue root)
            where TTree : ITree<TValue, TNode>, new()
            where TNode : INode<TValue, TNode>, new()
        {
            if (graph == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(graph));
            }
            if (!graph.Contains(root))
            {
                throw ExceptionHelper.KeyNotFound();
            }
            TTree result = new TTree();
            result.Root.Value = root;
            Queue<TNode> nodes = new Queue<TNode>();
            HashSet<TValue> visited = new HashSet<TValue>();
            nodes.Enqueue(result.Root);
            while (nodes.Count != 0)
            {
                TNode current;
                for (; ; )
                {
                    if (nodes.Count == 0)
                    {
                        goto ret;
                    }
                    current = nodes.Dequeue();
                    if (visited.Contains(current.Value))
                    {
                        current.Parent.Remove(current);
                        continue;
                    }
                    break;
                }
                visited.Add(current.Value);
                if (graph.TryGetHeads(current.Value, out var heads))
                {
                    foreach (var child in heads)
                    {
                        if (!visited.Contains(child))
                        {
                            TNode nc = new TNode();
                            nc.Value = child;
                            nodes.Enqueue(nc);
                            current.Add(nc);
                        }
                    }
                }
            }
            ret:
            return result;
        }
        #endregion
        #region Key
        public static KeyDictionary<TKey1, TKey2> ToKeyDictionary<TSource, TKey1, TKey2>(this IEnumerable<TSource> source, Func<TSource, TKey1> key1Selector, Func<TSource, TKey2> key2Selector)
            => ToKeyDictionary(source, key1Selector, key2Selector, null, null);
        public static KeyDictionary<TKey1, TKey2> ToKeyDictionary<TSource, TKey1, TKey2>(this IEnumerable<TSource> source, Func<TSource, TKey1> key1Selector, Func<TSource, TKey2> key2Selector, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
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
            => SelectWhenIterator(source ?? throw ExceptionHelper.ArgumentNull(nameof(source)), predicate ?? throw ExceptionHelper.ArgumentNull(nameof(predicate)));
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
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            switch (source ?? throw ExceptionHelper.ArgumentNull(nameof(source)))
            {
                case TSource[] array:
                    return ForEachArrayIterator(array, action);
                case IList<TSource> collection:
                    return ForEachListIterator(collection, action);
                default:
                    return ForEachIterator(source, action);
            }
        }
        private static IEnumerable<TSource> ForEachArrayIterator<TSource>(TSource[] source, Action<TSource> action)
        {
            int n = source.Length;
            for (int i = 0; i < n; i++)
            {
                action(source[i]);
                yield return source[i];
            }
        }
        private static IEnumerable<TSource> ForEachListIterator<TSource>(IList<TSource> source, Action<TSource> action)
        {
            int n = source.Count;
            for (int i = 0; i < n; i++)
            {
                action(source[i]);
                yield return source[i];
            }
        }
        private static IEnumerable<TSource> ForEachIterator<TSource>(IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (TSource item in source)
            {
                action(item);
                yield return item;
            }
        }
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action)
        {
            switch (source ?? throw ExceptionHelper.ArgumentNull(nameof(source)))
            {
                case TSource[] array:
                    return ForEachArrayIterator(array, action);
                case IList<TSource> collection:
                    return ForEachListIterator(collection, action);
                default:
                    return ForEachIterator(source, action);
            }
        }
        private static IEnumerable<TSource> ForEachArrayIterator<TSource>(TSource[] source, Action<TSource, int> action)
        {
            int n = source.Length;
            for (int i = 0; i < n; i++)
            {
                action(source[i], i);
                yield return source[i];
            }
        }
        private static IEnumerable<TSource> ForEachListIterator<TSource>(IList<TSource> source, Action<TSource, int> action)
        {
            int n = source.Count;
            for (int i = 0; i < n; i++)
            {
                action(source[i], i);
                yield return source[i];
            }
        }
        private static IEnumerable<TSource> ForEachIterator<TSource>(IEnumerable<TSource> source, Action<TSource, int> action)
        {
            int i = 0;
            foreach (TSource item in source)
            {
                action(item, i);
                yield return item;
                i++;
            }
        }
        public static IEnumerable<TSource> Random<TSource>(this IEnumerable<TSource> source)
            => RandomIterator(source ?? throw ExceptionHelper.ArgumentNull(nameof(source)));
        private static IEnumerable<TSource> RandomIterator<TSource>(IEnumerable<TSource> source)
        {
            Random random = new Random();
            List<TSource> list = new List<TSource>(source);
            int n = list.Count;
            for (int i = 0; i < n; i++)
            {
                int index = random.Next(list.Count - 1);
                yield return list[index];
                list.RemoveAt(index);
            }
        }
        #endregion
    }
}
