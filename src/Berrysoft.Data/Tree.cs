using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Berrysoft.Data
{
    #region Interfaces
    /// <summary>
    /// Exposes members of a node.
    /// </summary>
    /// <typeparam name="T">The type of value the node contains.</typeparam>
    public interface ITreeBase<T> : IEnumerable<ITreeBase<T>>
    {
        /// <summary>
        /// Value the node contains.
        /// </summary>
        T Value { get; set; }
        /// <summary>
        /// Parent of the node.
        /// </summary>
        ITreeBase<T> Parent { get; }
        /// <summary>
        /// The count of children.
        /// </summary>
        int Count { get; }
    }
    /// <summary>
    /// Exposes members of a node with many children.
    /// </summary>
    /// <typeparam name="T">The type of value the node contains.</typeparam>
    public interface ITree<T> : ITreeBase<T>
    {
        /// <summary>
        /// Add a child.
        /// </summary>
        /// <param name="child">The child node to be added.</param>
        void Add(ITree<T> child);
        /// <summary>
        /// Insert a child.
        /// </summary>
        /// <param name="index">The zero-based index at which child should be inserted.</param>
        /// <param name="child">The child to insert.</param>
        void Insert(int index, ITree<T> child);
        /// <summary>
        /// Determines whether a child is in the <see cref="ITree{TValue}"/>.
        /// </summary>
        /// <param name="child">The child to locate in the <see cref="ITree{TValue}"/>.</param>
        /// <returns><see langword="true"/> if the child is found; otherwise, <see langword="false"/>.</returns>
        bool Contains(ITree<T> child);
        /// <summary>
        /// Remove a child.
        /// </summary>
        /// <param name="child">The child node to be removed.</param>
        /// <returns><see langword="true"/> if the child is successfully removed; otherwise, <see langword="false"/>.</returns>
        bool Remove(ITree<T> child);
        /// <summary>
        /// Clear all children.
        /// </summary>
        void Clear();
    }
    #endregion
    public static partial class Enumerable
    {
        /// <summary>
        /// Get depth of the tree.
        /// </summary>
        /// <typeparam name="T">The type of value the node contains.</typeparam>
        /// <param name="tree">A tree to calculte depth.</param>
        /// <returns>The depth of the tree.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="tree"/> is <see langword="null"/>.</exception>
        public static int GetDepth<T>(this ITreeBase<T> tree)
        {
            int GetDepthInternal(ITreeBase<T> node, int depth)
            {
                int result = depth;
                foreach (ITreeBase<T> child in node)
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
                case IBinaryTree<T> binaryTree:
                    return GetDepthBinary(binaryTree);
                default:
                    return GetDepthInternal(tree, 1);
            }
        }
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with order of depth-first-search.
        /// </summary>
        /// <typeparam name="T">The type of value the node contains.</typeparam>
        /// <param name="tree">A tree to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of depth-first-search.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="tree"/> is <see langword="null"/>.</exception>
        public static IEnumerable<ITreeBase<T>> AsDFSEnumerable<T>(this ITreeBase<T> tree)
        {
            switch (tree ?? throw ExceptionHelper.ArgumentNull(nameof(tree)))
            {
                case IBinaryTree<T> binaryTree:
                    return AsPreOrderEnumerableIterator(binaryTree);
                default:
                    return AsDFSEnumerableIterator(tree);
            }
        }
        /// <summary>
        /// Get an iterator with order of depth-first-search.
        /// </summary>
        /// <typeparam name="T">The type of value the node contains.</typeparam>
        /// <param name="tree">A tree to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of depth-first-search.</returns>
        private static IEnumerable<ITreeBase<T>> AsDFSEnumerableIterator<T>(ITreeBase<T> tree)
        {
            Stack<ITreeBase<T>> nodes = new Stack<ITreeBase<T>>();
            nodes.Push(tree);
            while (nodes.Count != 0)
            {
                ITreeBase<T> current = nodes.Pop();
                yield return current;
                foreach (var child in current.Reverse())
                {
                    nodes.Push(child);
                }
            }
        }
        public static IEnumerable<(ITreeBase<T> Node, IReadOnlyCollection<ITreeBase<T>> Path)> AsDFSWithPath<T>(this ITreeBase<T> tree)
            => AsDFSWithPathIterator(tree ?? throw ExceptionHelper.ArgumentNull(nameof(tree)));
        private static IEnumerable<(ITreeBase<T> Node, IReadOnlyCollection<ITreeBase<T>> Path)> AsDFSWithPathIterator<T>(ITreeBase<T> tree)
        {
            Stack<(int Index, ITreeBase<T> Node)> nodes = new Stack<(int Index, ITreeBase<T> Node)>();
            List<ITreeBase<T>> list = new List<ITreeBase<T>>();
            nodes.Push((0, tree));
            while (nodes.Count != 0)
            {
                var (index, current) = nodes.Pop();
                if (index < list.Count)
                {
                    list.RemoveRange(index, list.Count - index);
                }
                list.Add(current);
                yield return (current, list);
                index++;
                foreach (var child in current.Reverse())
                {
                    nodes.Push((index, child));
                }
            }
        }
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with order of breadth-first-search.
        /// </summary>
        /// <typeparam name="T">The type of value the node contains.</typeparam>
        /// <param name="tree">A tree to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of breadth-first-search.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="tree"/> is <see langword="null"/>.</exception>
        public static IEnumerable<ITreeBase<T>> AsBFSEnumerable<T>(this ITreeBase<T> tree)
        {
            switch (tree ?? throw ExceptionHelper.ArgumentNull(nameof(tree)))
            {
                case IBinaryTree<T> binaryTree:
                    return AsLevelOrderEnumerableIterator(binaryTree);
                default:
                    return AsBFSEnumerableIterator(tree);
            }
        }
        /// <summary>
        /// Get an iterator with order of breadth-first-search.
        /// </summary>
        /// <typeparam name="T">The type of value the node contains.</typeparam>
        /// <param name="tree">A tree to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of breadth-first-search.</returns>
        private static IEnumerable<ITreeBase<T>> AsBFSEnumerableIterator<T>(ITreeBase<T> tree)
        {
            Queue<ITreeBase<T>> nodes = new Queue<ITreeBase<T>>();
            nodes.Enqueue(tree);
            while (nodes.Count != 0)
            {
                ITreeBase<T> current = nodes.Dequeue();
                yield return current;
                foreach (var child in current)
                {
                    nodes.Enqueue(child);
                }
            }
        }
        public static IEnumerable<(ITreeBase<T> Node, IReadOnlyCollection<ITreeBase<T>> Path)> AsBFSWithPath<T>(this ITreeBase<T> tree)
            => AsBFSWithPathIterator(tree ?? throw ExceptionHelper.ArgumentNull(nameof(tree)));
        private static IEnumerable<(ITreeBase<T> Node, IReadOnlyCollection<ITreeBase<T>> Path)> AsBFSWithPathIterator<T>(ITreeBase<T> tree)
        {
            Queue<(int Index, ITreeBase<T> Node)> nodes = new Queue<(int Index, ITreeBase<T> Node)>();
            List<ITreeBase<T>> list = new List<ITreeBase<T>>();
            nodes.Enqueue((0, tree));
            while (nodes.Count != 0)
            {
                var (index, current) = nodes.Dequeue();
                if (index < list.Count)
                {
                    list.RemoveRange(index, list.Count - index);
                }
                list.Add(current);
                yield return (current, list);
                index++;
                foreach (var child in current)
                {
                    nodes.Enqueue((index, child));
                }
            }
        }
        /// <summary>
        /// Get current node and current path when searching.
        /// </summary>
        /// <typeparam name="T">The type of value the node contains.</typeparam>
        /// <param name="nodes">Nodes of searching.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with node and path. The path is in reverse order.</returns>
        [Obsolete("Use specified WithPath function instead.")]
        public static IEnumerable<(ITreeBase<T> Node, IReadOnlyCollection<ITreeBase<T>> Path)> WithPath<T>(this IEnumerable<ITreeBase<T>> nodes)
        {
            return WithPathIterator(nodes ?? throw ExceptionHelper.ArgumentNull(nameof(nodes)));
        }
        /// <summary>
        /// Get current node and current path when searching.
        /// </summary>
        /// <typeparam name="T">The type of value the node contains.</typeparam>
        /// <param name="nodes">Nodes of searching.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with node and path. The path is in reverse order.</returns>
        private static IEnumerable<(ITreeBase<T> Node, IReadOnlyCollection<ITreeBase<T>> Path)> WithPathIterator<T>(IEnumerable<ITreeBase<T>> nodes)
        {
            Stack<ITreeBase<T>> stack = new Stack<ITreeBase<T>>();
            bool check = false;
            foreach (ITreeBase<T> node in nodes)
            {
                if (check)
                {
                    while (!EqualityComparer<ITreeBase<T>>.Default.Equals(stack.Peek(), node.Parent))
                    {
                        stack.Pop();
                    }
                    check = false;
                }
                stack.Push(node);
                yield return (node, stack);
                if (node.Count == 0)
                {
                    check = true;
                }
            }
        }
        /// <summary>
        /// Get an instance of <see cref="Graph{T}"/> class which is equivalent to the tree.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="tree">The specified tree.</param>
        /// <returns>An instance of <see cref="Graph{T}"/>.</returns>
        public static IGraph<T> ToGraph<T>(this ITreeBase<T> tree)
        {
            if (tree == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(tree));
            }
            Graph<T> graph = new Graph<T>();
            Queue<ITreeBase<T>> nodes = new Queue<ITreeBase<T>>();
            nodes.Enqueue(tree);
            graph.Add(tree.Value);
            while (nodes.Count != 0)
            {
                ITreeBase<T> current = nodes.Dequeue();
                foreach (var child in current)
                {
                    nodes.Enqueue(child);
                    graph.Add(child.Value);
                    graph.AddEdge(current.Value, child.Value);
                }
            }
            return graph;
        }
        /// <summary>
        /// Get an instance of <see cref="Graph{T}"/> which is equivalentto the tree. Each arc starts from the parent to the child.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="tree">The specified tree.</param>
        /// <returns>An instance of <see cref="Graph{T}"/>.</returns>
        public static IGraph<T> ToDirectedGraph<T>(this ITreeBase<T> tree)
        {
            if (tree == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(tree));
            }
            Graph<T> graph = new Graph<T>();
            Queue<ITreeBase<T>> nodes = new Queue<ITreeBase<T>>();
            nodes.Enqueue(tree);
            graph.Add(tree.Value);
            while (nodes.Count != 0)
            {
                ITreeBase<T> current = nodes.Dequeue();
                foreach (var child in current)
                {
                    nodes.Enqueue(child);
                    graph.AddAsHead(child.Value, current.Value);
                }
            }
            return graph;
        }
    }
    /// <summary>
    /// Represents a tree.
    /// </summary>
    /// <typeparam name="T">The type of value the node contains.</typeparam>
    public class Tree<T> : ITree<T>
    {
        private T _value;
        private Tree<T> _parent;
        private List<Tree<T>> _children;
        /// <summary>
        /// Initialize an instance of <see cref="Tree{T}"/>.
        /// </summary>
        public Tree()
            : this(default)
        { }
        /// <summary>
        /// Initialize an instance of <see cref="Tree{T}"/> with value.
        /// </summary>
        /// <param name="value">Value of the node.</param>
        public Tree(T value)
        {
            _value = value;
            _children = new List<Tree<T>>();
        }
        /// <summary>
        /// Initialize an instance of <see cref="Tree{T}"/> with value and children.
        /// </summary>
        /// <param name="value">Value of the node.</param>
        /// <param name="children">Children of the node.</param>
        public Tree(T value, IEnumerable<Tree<T>> children)
        {
            _value = value;
            _children = new List<Tree<T>>(children.ForEach(SetParent));
        }
        /// <summary>
        /// Set the child's parent as this node.
        /// </summary>
        /// <param name="child">Child to be set.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="child"/> is null.</exception>
        private void SetParent(Tree<T> child)
        {
            (child ?? throw ExceptionHelper.ArgumentNull(nameof(child)))._parent = this;
        }
        /// <summary>
        /// Value of the node.
        /// </summary>
        public T Value
        {
            get => _value;
            set => _value = value;
        }
        /// <summary>
        /// Parent of the node, null when the node is root node of a tree.
        /// </summary>
        public Tree<T> Parent => _parent;
        /// <summary>
        /// The count of children.
        /// </summary>
        public int Count => _children.Count;
        /// <summary>
        /// Parent of the node, null when the node is root node of a tree.
        /// </summary>
        ITreeBase<T> ITreeBase<T>.Parent => Parent;
        /// <summary>
        /// Add a child.
        /// </summary>
        /// <param name="child">The child node to be added.</param>
        public void Add(Tree<T> child)
        {
            SetParent(child);
            _children.Add(child);
        }
        /// <summary>
        /// Add a child.
        /// </summary>
        /// <param name="child">The child node to be added.</param>
        void ITree<T>.Add(ITree<T> child) => Add((Tree<T>)child);
        /// <summary>
        /// Add children.
        /// </summary>
        /// <param name="children">The child nodes to be added.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="children"/> is null.</exception>
        public void AddRange(IEnumerable<Tree<T>> children)
        {
            _children.AddRange((children ?? throw ExceptionHelper.ArgumentNull(nameof(children))).ForEach(SetParent));
        }
        /// <summary>
        /// Insert a child.
        /// </summary>
        /// <param name="index">The zero-based index at which child should be inserted.</param>
        /// <param name="child">The child to insert.</param>
        public void Insert(int index, Tree<T> child)
        {
            SetParent(child);
            _children.Insert(index, child);
        }
        /// <summary>
        /// Insert a child.
        /// </summary>
        /// <param name="index">The zero-based index at which child should be inserted.</param>
        /// <param name="child">The child to insert.</param>
        void ITree<T>.Insert(int index, ITree<T> child) => Insert(index, (Tree<T>)child);
        /// <summary>
        /// Determines whether a child is in the <see cref="ITree{T}"/>.
        /// </summary>
        /// <param name="child">The child to locate in the <see cref="ITree{T}"/>.</param>
        /// <returns><see langword="true"/> if the child is found; otherwise, <see langword="false"/>.</returns>
        public bool Contains(Tree<T> child)
        {
            return _children.Contains(child);
        }
        /// <summary>
        /// Determines whether a child is in the <see cref="ITree{T}"/>.
        /// </summary>
        /// <param name="child">The child to locate in the <see cref="ITree{T}"/>.</param>
        /// <returns><see langword="true"/> if the child is found; otherwise, <see langword="false"/>.</returns>
        bool ITree<T>.Contains(ITree<T> child)
        {
            if (child is Tree<T> c)
            {
                return Contains(c);
            }
            return false;
        }
        /// <summary>
        /// Remove a child.
        /// </summary>
        /// <param name="child">The child node to be removed.</param>
        /// <returns><see langword="true"/> if the child is successfully removed; otherwise, <see langword="false"/>.</returns>
        public bool Remove(Tree<T> child)
        {
            if (_children.Remove(child))
            {
                child._parent = null;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Remove a child.
        /// </summary>
        /// <param name="child">The child node to be removed.</param>
        /// <returns><see langword="true"/> if the child is successfully removed; otherwise, <see langword="false"/>.</returns>
        bool ITree<T>.Remove(ITree<T> child)
        {
            if (child is Tree<T> c)
            {
                return Remove(c);
            }
            return false;
        }
        /// <summary>
        /// Clear all children.
        /// </summary>
        public void Clear()
        {
            _children.ForEach(child => child._parent = null);
            _children.Clear();
        }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Tree{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{TNode}"/> for the <see cref="Tree{T}"/>.</returns>
        public IEnumerator<Tree<T>> GetEnumerator() => _children.GetEnumerator();
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Tree{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{TNode}"/> for the <see cref="Tree{T}"/>.</returns>
        IEnumerator<ITreeBase<T>> IEnumerable<ITreeBase<T>>.GetEnumerator() => GetEnumerator();
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Tree{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable"/> for the <see cref="Tree{T}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_children).GetEnumerator();
        /// <summary>
        /// Convert <see cref="Tree{T}"/> to <typeparamref name="T"/> explicitly.
        /// </summary>
        /// <param name="node">Node to be converted.</param>
        /// <returns>Value of the node.</returns>
        public static explicit operator T(Tree<T> node) => node.Value;
        /// <summary>
        /// Convert <typeparamref name="T"/> to <see cref="Tree{T}"/> implicitly.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns>A new <see cref="Tree{T}"/>.</returns>
        public static implicit operator Tree<T>(T value) => new Tree<T>(value);
        /// <summary>
        /// Returns a string that represents the value.
        /// </summary>
        /// <returns>A string that represents the value.</returns>
        public override string ToString() => _value.ToString();
    }
}
