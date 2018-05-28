using System;
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
    public interface INodeBase<TValue, TNode>
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
        /// Get an <see cref="IEnumerable{TNode}"/> of its children.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{TNode}"/> of its children.</returns>
        IEnumerable<TNode> AsEnumerable();
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
        /// The count of children.
        /// </summary>
        int Count { get; }
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
        /// <returns>true if the child is found; otherwise, false.</returns>
        bool Contains(TNode child);
        /// <summary>
        /// Remove a child.
        /// </summary>
        /// <param name="child">The child node to be removed.</param>
        /// <returns>true if the child is successfully removed; otherwise, false.</returns>
        bool Remove(TNode child);
        /// <summary>
        /// Clear all children.
        /// </summary>
        void Clear();
    }
    #endregion
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
                foreach (Node<T> child in node.AsEnumerable())
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
        /// <returns>true if the child is found; otherwise, false.</returns>
        public bool Contains(Node<T> child)
        {
            return _children.Contains(child);
        }
        /// <summary>
        /// Remove a child.
        /// </summary>
        /// <param name="child">The child node to be removed.</param>
        /// <returns>true if the child is successfully removed; otherwise, false.</returns>
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
        /// Get an <see cref="IEnumerable{TNode}"/> of its children.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{TNode}"/> of its children.</returns>
        public IEnumerable<Node<T>> AsEnumerable() => _children;
        /// <summary>
        /// Convert <see cref="Node{T}"/> to <typeparamref name="T"/> explicitly.
        /// </summary>
        /// <param name="node">Node to be converted.</param>
        public static explicit operator T(Node<T> node) => node.Value;
        /// <summary>
        /// Convert <typeparamref name="T"/> to <see cref="Node{T}"/> implicitly.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        public static implicit operator Node<T>(T value) => new Node<T>(value);
        /// <summary>
        /// Returns a string that represents the value.
        /// </summary>
        /// <returns>A string that represents the value.</returns>
        public override string ToString() => _value.ToString();
    }
}
