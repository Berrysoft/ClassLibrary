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
        /// <returns></returns>
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
    public class Tree<T> : ITree<T, Node<T>>
    {
        private Node<T> _root;
        public Tree()
        {
            _root = new Node<T>();
        }
        public Tree(T value)
        {
            _root = new Node<T>(value);
        }
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
        public Node<T> Root => _root;
        public int GetDepth()
        {
            return GetDepthInternal(_root, 1);
        }
        private int GetDepthInternal(Node<T> node, int depth)
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
    }
    public class Node<T> : INode<T, Node<T>>
    {
        private T _value;
        private Node<T> _parent;
        private List<Node<T>> _children;
        public Node()
            : this(default)
        { }
        public Node(T value)
        {
            _value = value;
            _children = new List<Node<T>>();
        }
        public Node(T value, Node<T> parent)
        {
            _value = value;
            _parent = parent;
            _children = new List<Node<T>>();
        }
        public Node(T value, IEnumerable<Node<T>> children)
        {
            _value = value;
            _children = new List<Node<T>>(children);
        }
        public Node(T value, Node<T> parent, IEnumerable<Node<T>> children)
        {
            _value = value;
            _parent = parent;
            _children = new List<Node<T>>(children);
        }
        public T Value
        {
            get => _value;
            set => _value = value;
        }
        public Node<T> Parent => _parent;
        public int Count => _children.Count;
        public void Add(Node<T> child)
        {
            if (child == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(child));
            }
            child._parent = this;
            _children.Add(child);
        }
        public void Insert(int index, Node<T> child)
        {
            if (child == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(child));
            }
            child._parent = this;
            _children.Insert(index, child);
        }
        public bool Contains(Node<T> child)
        {
            return _children.Contains(child);
        }
        public bool Remove(Node<T> child)
        {
            if (_children.Remove(child))
            {
                child._parent = null;
                return true;
            }
            return false;
        }
        public void Clear()
        {
            _children.ForEach(child => child._parent = null);
            _children.Clear();
        }
        public IEnumerable<Node<T>> AsEnumerable() => _children;
        public static explicit operator T(Node<T> node) => node.Value;
        public static implicit operator Node<T>(T value) => new Node<T>(value);
        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
