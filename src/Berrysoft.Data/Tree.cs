using System;
using System.Collections.Generic;
using System.Linq;

namespace Berrysoft.Data
{
    #region Interfaces
    public interface ITree<TValue, TNode>
        where TNode : INodeBase<TValue, TNode>
    {
        TNode Root { get; }
    }
    /// <summary>
    /// Exposes method and properties of a node.
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
    public interface INode<TValue, TNode> : INodeBase<TValue, TNode>
        where TNode : INode<TValue, TNode>
    {
        int Count { get; }
        void Add(TNode child);
        void Insert(int index, TNode child);
        bool Contains(TNode child);
        bool Remove(TNode child);
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
