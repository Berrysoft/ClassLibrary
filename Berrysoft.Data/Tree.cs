using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berrysoft.Data
{
    public interface ITree<TValue,TNode>
        where TNode : INode<TValue, TNode>
    {
        TNode Root { get; }
        int GetDepth();
        void Traverse(Func<TValue, TValue> func);
    }
    public interface INode<TValue, TNode>
        where TNode : INode<TValue, TNode>
    {
        TValue Value { get; set; }
        TNode Parent { get; }
        int Count { get; }
        void Add(TNode child);
        void Insert(int index, TNode child);
        bool Contains(TNode child);
        void Remove(TNode child);
        void Clear();
        IEnumerable<TNode> AsEnumerable();
    }
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
        public Node<T> Root => _root;
        public int GetDepth()
        {
            return GetDepthInternal(_root, 1);
        }
        private int GetDepthInternal(Node<T> node,int depth)
        {
            int result = depth;
            foreach(Node<T> child in node.AsEnumerable())
            {
                int tempDepth = GetDepthInternal(child, depth + 1);
                if (tempDepth > result)
                {
                    result = tempDepth;
                }
            }
            return result;
        }
        public void Traverse(Func<T, T> func)
        {
            _root.Traverse(func);
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
        public T Value
        {
            get => _value;
            set => _value = value;
        }
        public Node<T> Parent => _parent;
        public int Count => _children.Count;
        public void Add(Node<T> child)
        {
            child._parent = this;
            _children.Add(child);
        }
        public void Insert(int index, Node<T> child)
        {
            child._parent = this;
            _children.Insert(index, child);
        }
        public bool Contains(Node<T> child)
        {
            return _children.Contains(child);
        }
        public void Remove(Node<T> child)
        {
            child._parent = null;
            _children.Remove(child);
        }
        public void Clear()
        {
            _children.ForEach(child => child._parent = null);
            _children.Clear();
        }
        public IEnumerable<Node<T>> AsEnumerable() => _children;
        internal void Traverse(Func<T, T> func)
        {
            _value = func(_value);
            foreach(Node<T> child in _children)
            {
                child.Traverse(func);
            }
        }
    }
}
