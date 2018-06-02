using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Berrysoft.Data
{
    #region Interfaces
    /// <summary>
    /// Exposes members of a tree data structure.
    /// </summary>
    /// <typeparam name="TValue">The type of value the node contains.</typeparam>
    /// <typeparam name="TNode">The type of node.</typeparam>
    public interface ITree<TValue, TNode>
        where TNode : INodeBase<TValue, TNode>
    {
        /// <summary>
        /// The root node of a tree.
        /// </summary>
        TNode Root { get; }
    }
    /// <summary>
    /// Exposes members of a node.
    /// </summary>
    /// <typeparam name="TValue">The type of value the node contains.</typeparam>
    /// <typeparam name="TNode">The type of node.</typeparam>
    public interface INodeBase<TValue, TNode> : IEnumerable<TNode>
        where TNode : INodeBase<TValue, TNode>
    {
        /// <summary>
        /// Value the node contains.
        /// </summary>
        TValue Value { get; set; }
        /// <summary>
        /// Parent of the node.
        /// </summary>
        TNode Parent { get; }
        /// <summary>
        /// The count of children.
        /// </summary>
        int Count { get; }
    }
    /// <summary>
    /// Exposes members of a node with many children.
    /// </summary>
    /// <typeparam name="TValue">The type of value the node contains.</typeparam>
    /// <typeparam name="TNode">The type of node.</typeparam>
    public interface INode<TValue, TNode> : INodeBase<TValue, TNode>
        where TNode : INode<TValue, TNode>
    {
        /// <summary>
        /// Add a child.
        /// </summary>
        /// <param name="child">The child node to be added.</param>
        void Add(TNode child);
        /// <summary>
        /// Insert a child.
        /// </summary>
        /// <param name="index">The zero-based index at which child should be inserted.</param>
        /// <param name="child">The child to insert.</param>
        void Insert(int index, TNode child);
        /// <summary>
        /// Determines whether a child is in the <see cref="INode{TValue, TNode}"/>.
        /// </summary>
        /// <param name="child">The child to locate in the <see cref="INode{TValue, TNode}"/>.</param>
        /// <returns><see langword="true"/> if the child is found; otherwise, <see langword="false"/>.</returns>
        bool Contains(TNode child);
        /// <summary>
        /// Remove a child.
        /// </summary>
        /// <param name="child">The child node to be removed.</param>
        /// <returns><see langword="true"/> if the child is successfully removed; otherwise, <see langword="false"/>.</returns>
        bool Remove(TNode child);
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
        /// <typeparam name="TValue">The type of value the node contains.</typeparam>
        /// <typeparam name="TNode">The type of node.</typeparam>
        /// <param name="tree">A tree to calculte depth.</param>
        /// <returns>The depth of the tree.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="tree"/> is <see langword="null"/>.</exception>
        public static int GetDepth<TValue, TNode>(this ITree<TValue, TNode> tree)
            where TNode : INodeBase<TValue, TNode>
        {
            int GetDepthInternal(TNode node, int depth)
            {
                int result = depth;
                foreach (TNode child in node)
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
        /// <exception cref="ArgumentNullException">When <paramref name="tree"/> is <see langword="null"/>.</exception>
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
                foreach (var child in current.Reverse())
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
        /// <exception cref="ArgumentNullException">When <paramref name="tree"/> is <see langword="null"/>.</exception>
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
                foreach (var child in current)
                {
                    nodes.Enqueue(child);
                }
            }
        }
        /// <summary>
        /// Get current node and current path when searching.
        /// </summary>
        /// <typeparam name="TValue">The type of value the node contains.</typeparam>
        /// <typeparam name="TNode">The type of node.</typeparam>
        /// <param name="nodes">Nodes of searching.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with node and path.</returns>
        public static IEnumerable<(TNode Node, IReadOnlyCollection<TNode> Path)> WithPath<TValue, TNode>(this IEnumerable<INodeBase<TValue, TNode>> nodes)
            where TNode : INodeBase<TValue, TNode>
        {
            return WithPathIterator(nodes ?? throw ExceptionHelper.ArgumentNull(nameof(nodes)));
        }
        /// <summary>
        /// Get current node and current path when searching.
        /// </summary>
        /// <typeparam name="TValue">The type of value the node contains.</typeparam>
        /// <typeparam name="TNode">The type of node.</typeparam>
        /// <param name="nodes">Nodes of searching.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with node and path.</returns>
        private static IEnumerable<(TNode Node, IReadOnlyCollection<TNode> Path)> WithPathIterator<TValue, TNode>(IEnumerable<INodeBase<TValue, TNode>> nodes)
            where TNode : INodeBase<TValue, TNode>
        {
            Stack<TNode> stack = new Stack<TNode>();
            bool check = false;
            foreach (TNode node in nodes)
            {
                if(check)
                {
                    while (!EqualityComparer<TNode>.Default.Equals(stack.Peek(), node.Parent))
                    {
                        stack.Pop();
                    }
                    check = false;
                }
                stack.Push(node);
                yield return (node, stack);
                if (node.Count==0)
                {
                    check = true;
                }
            }
        }
    }
    /// <summary>
    /// Represents a tree with a root <see cref="Node{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of value the node contains.</typeparam>
    public class Tree<T> : ITree<T, Node<T>>
    {
        private Node<T> _root;
        /// <summary>
        /// Initialize an instance of <see cref="Tree{T}"/>.
        /// </summary>
        public Tree()
        {
            _root = new Node<T>();
        }
        /// <summary>
        /// Initialize an instance of <see cref="Tree{T}"/> with root value.
        /// </summary>
        /// <param name="value">The value of root node.</param>
        public Tree(T value)
        {
            _root = new Node<T>(value);
        }
        /// <summary>
        /// Initialize an instance of <see cref="Tree{T}"/> with root node.
        /// </summary>
        /// <param name="root">Root node.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="root"/> is null.</exception>
        /// <exception cref="ArgumentException">When <paramref name="root"/> has a parent.</exception>
        public Tree(Node<T> root)
        {
            if (root == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(root));
            }
            if (root.Parent != null)
            {
                throw ExceptionHelper.RootHasParent();
            }
            _root = root;
        }
        /// <summary>
        /// The root node of the tree.
        /// </summary>
        public Node<T> Root => _root;
        /// <summary>
        /// Get depth of the tree.
        /// </summary>
        /// <returns>The depth of the tree.</returns>
        public int GetDepth()
        {
            int GetDepthInternal(Node<T> node, int depth)
            {
                int result = depth;
                foreach (Node<T> child in node)
                {
                    int tempDepth = GetDepthInternal(child, depth + 1);
                    if (tempDepth > result)
                    {
                        result = tempDepth;
                    }
                }
                return result;
            }
            return GetDepthInternal(_root, 1);
        }
    }
    /// <summary>
    /// Represents a node of a <see cref="Tree{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of value the node contains.</typeparam>
    public class Node<T> : INode<T, Node<T>>
    {
        private T _value;
        private Node<T> _parent;
        private List<Node<T>> _children;
        /// <summary>
        /// Initialize an instance of <see cref="Node{T}"/>.
        /// </summary>
        public Node()
            : this(default)
        { }
        /// <summary>
        /// Initialize an instance of <see cref="Node{T}"/> with value.
        /// </summary>
        /// <param name="value">Value of the node.</param>
        public Node(T value)
        {
            _value = value;
            _children = new List<Node<T>>();
        }
        /// <summary>
        /// Initialize an instance of <see cref="Node{T}"/> with value and children.
        /// </summary>
        /// <param name="value">Value of the node.</param>
        /// <param name="children">Children of the node.</param>
        public Node(T value, IEnumerable<Node<T>> children)
        {
            _value = value;
            _children = new List<Node<T>>(children.ForEach(SetParent));
        }
        /// <summary>
        /// Set the child's parent as this node.
        /// </summary>
        /// <param name="child">Child to be set.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="child"/> is null.</exception>
        private void SetParent(Node<T> child)
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
        public Node<T> Parent => _parent;
        /// <summary>
        /// The count of children.
        /// </summary>
        public int Count => _children.Count;
        /// <summary>
        /// Add a child.
        /// </summary>
        /// <param name="child">The child node to be added.</param>
        public void Add(Node<T> child)
        {
            SetParent(child);
            _children.Add(child);
        }
        /// <summary>
        /// Add children.
        /// </summary>
        /// <param name="children">The child nodes to be added.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="children"/> is null.</exception>
        public void AddRange(IEnumerable<Node<T>> children)
        {
            _children.AddRange((children ?? throw ExceptionHelper.ArgumentNull(nameof(children))).ForEach(SetParent));
        }
        /// <summary>
        /// Insert a child.
        /// </summary>
        /// <param name="index">The zero-based index at which child should be inserted.</param>
        /// <param name="child">The child to insert.</param>
        public void Insert(int index, Node<T> child)
        {
            SetParent(child);
            _children.Insert(index, child);
        }
        /// <summary>
        /// Determines whether a child is in the <see cref="INode{TValue, TNode}"/>.
        /// </summary>
        /// <param name="child">The child to locate in the <see cref="INode{TValue, TNode}"/>.</param>
        /// <returns><see langword="true"/> if the child is found; otherwise, <see langword="false"/>.</returns>
        public bool Contains(Node<T> child)
        {
            return _children.Contains(child);
        }
        /// <summary>
        /// Remove a child.
        /// </summary>
        /// <param name="child">The child node to be removed.</param>
        /// <returns><see langword="true"/> if the child is successfully removed; otherwise, <see langword="false"/>.</returns>
        public bool Remove(Node<T> child)
        {
            if (_children.Remove(child))
            {
                child._parent = null;
                return true;
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
        /// Returns an enumerator that iterates through the <see cref="Node{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{TNode}"/> for the <see cref="Node{T}"/>.</returns>
        public IEnumerator<Node<T>> GetEnumerator() => _children.GetEnumerator();
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Node{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable"/> for the <see cref="Node{T}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_children).GetEnumerator();
        /// <summary>
        /// Convert <see cref="Node{T}"/> to <typeparamref name="T"/> explicitly.
        /// </summary>
        /// <param name="node">Node to be converted.</param>
        /// <returns>Value of the node.</returns>
        public static explicit operator T(Node<T> node) => node.Value;
        /// <summary>
        /// Convert <typeparamref name="T"/> to <see cref="Node{T}"/> implicitly.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns>A new <see cref="Node{T}"/>.</returns>
        public static implicit operator Node<T>(T value) => new Node<T>(value);
        /// <summary>
        /// Returns a string that represents the value.
        /// </summary>
        /// <returns>A string that represents the value.</returns>
        public override string ToString() => _value.ToString();
    }
}
