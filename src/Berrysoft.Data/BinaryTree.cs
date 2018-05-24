using System;
using System.Collections.Generic;

namespace Berrysoft.Data
{
    #region Interfaces
    public interface IBinaryNode<TValue, TNode> : INodeBase<TValue, TNode>
        where TNode : IBinaryNode<TValue, TNode>
    {
        TNode LeftChild { get; set; }
        TNode RightChild { get; set; }
    }
    #endregion
    public class BinaryTree<T> : ITree<T, BinaryNode<T>>
    {
        private BinaryNode<T> _root;
        public BinaryTree()
        {
            _root = new BinaryNode<T>();
        }
        public BinaryTree(T value)
        {
            _root = new BinaryNode<T>(value);
        }
        public BinaryTree(BinaryNode<T> root)
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
        public BinaryNode<T> Root => _root;
        public int GetDepth()
        {
            return GetDepthInternal(_root, 1);
        }
        private int GetDepthInternal(BinaryNode<T> root, int depth)
        {
            int result = depth;
            int tempDepth;
            if (root.LeftChild != null)
            {
                tempDepth = GetDepthInternal(root.LeftChild, depth + 1);
                if (tempDepth > result)
                {
                    result = tempDepth;
                }
            }
            if (root.RightChild != null)
            {
                tempDepth = GetDepthInternal(root.RightChild, depth + 1);
                if (tempDepth > result)
                {
                    result = tempDepth;
                }
            }
            return result;
        }
        public IEnumerable<BinaryNode<T>> AsDFSEnumerable()
        {
            return AsPreOrderEnumerableIterator();
        }
        public IEnumerable<BinaryNode<T>> AsPreOrderEnumerable()
        {
            return AsPreOrderEnumerableIterator();
        }
        private IEnumerable<BinaryNode<T>> AsPreOrderEnumerableIterator()
        {
            Stack<BinaryNode<T>> nodes = new Stack<BinaryNode<T>>();
            nodes.Push(_root);
            while (nodes.Count != 0)
            {
                BinaryNode<T> current = nodes.Pop();
                yield return current;
                if (current.RightChild != null)
                {
                    nodes.Push(current.RightChild);
                }
                if (current.LeftChild != null)
                {
                    nodes.Push(current.LeftChild);
                }
            }
        }
        public IEnumerable<BinaryNode<T>> AsInOrderEnumerable()
        {
            return AsInOrderEnumerableIterator();
        }
        private IEnumerable<BinaryNode<T>> AsInOrderEnumerableIterator()
        {
            Stack<BinaryNode<T>> nodes = new Stack<BinaryNode<T>>();
            BinaryNode<T> current = _root;
            while (current != null || nodes.Count != 0)
            {
                if (current != null)
                {
                    nodes.Push(current);
                    current = current.LeftChild;
                }
                else
                {
                    BinaryNode<T> top = nodes.Pop();
                    yield return top;
                    current = top.RightChild;
                }
            }
        }
        public IEnumerable<BinaryNode<T>> AsPostOrderEnumerable()
        {
            return AsPostOrderEnumerableIterator();
        }
        private IEnumerable<BinaryNode<T>> AsPostOrderEnumerableIterator()
        {
            Stack<BinaryNode<T>> nodes = new Stack<BinaryNode<T>>();
            nodes.Push(_root);
            BinaryNode<T> pre = null;
            while (nodes.Count != 0)
            {
                BinaryNode<T> current = nodes.Peek();
                if ((current.LeftChild == null && current.RightChild == null) || (pre != null && (pre == current.LeftChild || pre == current.RightChild)))
                {
                    yield return current;
                    pre = current;
                    nodes.Pop();
                }
                else
                {
                    if (current.RightChild != null)
                    {
                        nodes.Push(current.RightChild);
                    }
                    if (current.LeftChild != null)
                    {
                        nodes.Push(current.LeftChild);
                    }
                }
            }
        }
        public IEnumerable<BinaryNode<T>> AsBFSEnumerable()
        {
            return AsLevelOrderEnumerableIterator();
        }
        public IEnumerable<BinaryNode<T>> AsLevelOrderEnumerable()
        {
            return AsLevelOrderEnumerableIterator();
        }
        private IEnumerable<BinaryNode<T>> AsLevelOrderEnumerableIterator()
        {
            Queue<BinaryNode<T>> nodes = new Queue<BinaryNode<T>>();
            nodes.Enqueue(_root);
            while (nodes.Count != 0)
            {
                BinaryNode<T> current = nodes.Dequeue();
                yield return current;
                if (current.LeftChild != null)
                {
                    nodes.Enqueue(current.LeftChild);
                }
                if (current.RightChild != null)
                {
                    nodes.Enqueue(current.RightChild);
                }
            }
        }
    }
    public class BinaryNode<T> : IBinaryNode<T, BinaryNode<T>>
    {
        private T _value;
        private BinaryNode<T> _parent;
        private BinaryNode<T> _left;
        private BinaryNode<T> _right;
        public BinaryNode()
            : this(default)
        { }
        public BinaryNode(T value)
        {
            _value = value;
        }
        public BinaryNode(T value, BinaryNode<T> parent)
        {
            _value = value;
            _parent = parent;
        }
        public BinaryNode(T value, BinaryNode<T> parent, BinaryNode<T> left, BinaryNode<T> right)
        {
            _value = value;
            _parent = parent;
            _left = left;
            _right = right;
        }
        public T Value
        {
            get => _value;
            set => _value = value;
        }
        public BinaryNode<T> Parent => _parent;
        public BinaryNode<T> LeftChild
        {
            get => _left;
            set
            {
                if (_left != null)
                {
                    _left._parent = null;
                }
                if (value != null)
                {
                    value._parent = this;
                }
                _left = value;
            }
        }
        public BinaryNode<T> RightChild
        {
            get => _right;
            set
            {
                if (_right != null)
                {
                    _right._parent = null;
                }
                if (value != null)
                {
                    value._parent = this;
                }
                _right = value;
            }
        }
        public IEnumerable<BinaryNode<T>> AsEnumerable()
        {
            yield return _left;
            yield return _right;
        }
        public static explicit operator T(BinaryNode<T> node) => node.Value;
        public static implicit operator BinaryNode<T>(T value) => new BinaryNode<T>(value);
        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
