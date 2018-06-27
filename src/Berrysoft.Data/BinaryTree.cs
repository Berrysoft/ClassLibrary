using System;
using System.Collections;
using System.Collections.Generic;

namespace Berrysoft.Data
{
    #region Interfaces
    /// <summary>
    /// Exposes members of a binary tree data structure.
    /// </summary>
    /// <typeparam name="T">The type of value the node contains.</typeparam>
    public interface IBinaryTree<T> : ITreeBase<T>
    {
        /// <summary>
        /// Left child of the tree.
        /// </summary>
        IBinaryTree<T> LeftChild { get; set; }
        /// <summary>
        /// Right child of the tree.
        /// </summary>
        IBinaryTree<T> RightChild { get; set; }
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
        private static int GetDepthBinary<T>(IBinaryTree<T> tree)
        {
            int GetDepthInternal(IBinaryTree<T> root, int depth)
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
            return GetDepthInternal(tree, 1);
        }
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with pre order.
        /// </summary>
        /// <typeparam name="T">The type of value the node contains.</typeparam>
        /// <param name="tree">The tree.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with pre order.</returns>
        public static IEnumerable<IBinaryTree<T>> AsPreOrderEnumerable<T>(this IBinaryTree<T> tree)
            => AsPreOrderEnumerableIterator(tree ?? throw ExceptionHelper.ArgumentNull(nameof(tree)));
        /// <summary>
        /// Get an iterator with pre order.
        /// </summary>
        /// <typeparam name="T">The type of value the node contains.</typeparam>
        /// <param name="tree">The tree.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with pre order.</returns>
        private static IEnumerable<IBinaryTree<T>> AsPreOrderEnumerableIterator<T>(IBinaryTree<T> tree)
        {
            Stack<IBinaryTree<T>> nodes = new Stack<IBinaryTree<T>>();
            nodes.Push(tree);
            while (nodes.Count != 0)
            {
                IBinaryTree<T> current = nodes.Pop();
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
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with in order.
        /// </summary>
        /// <typeparam name="T">The type of value the node contains.</typeparam>
        /// <param name="tree">The tree.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with in order.</returns>
        public static IEnumerable<IBinaryTree<T>> AsInOrderEnumerable<T>(this IBinaryTree<T> tree)
            => AsInOrderEnumerableIterator(tree ?? throw ExceptionHelper.ArgumentNull(nameof(tree)));
        /// <summary>
        /// Get an iterator with in order.
        /// </summary>
        /// <typeparam name="T">The type of value the node contains.</typeparam>
        /// <param name="tree">The tree.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with in order.</returns>
        private static IEnumerable<IBinaryTree<T>> AsInOrderEnumerableIterator<T>(IBinaryTree<T> tree)
        {
            Stack<IBinaryTree<T>> nodes = new Stack<IBinaryTree<T>>();
            IBinaryTree<T> current = tree;
            while (current != null || nodes.Count != 0)
            {
                if (current != null)
                {
                    nodes.Push(current);
                    current = current.LeftChild;
                }
                else
                {
                    IBinaryTree<T> top = nodes.Pop();
                    yield return top;
                    current = top.RightChild;
                }
            }
        }
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with post order.
        /// </summary>
        /// <typeparam name="T">The type of value the node contains.</typeparam>
        /// <param name="tree">The tree.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with post order.</returns>
        public static IEnumerable<IBinaryTree<T>> AsPostOrderEnumerable<T>(this IBinaryTree<T> tree)
            => AsPostOrderEnumerableIterator(tree ?? throw ExceptionHelper.ArgumentNull(nameof(tree)));
        /// <summary>
        /// Get an iterator with post order.
        /// </summary>
        /// <typeparam name="T">The type of value the node contains.</typeparam>
        /// <param name="tree">The tree.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with post order.</returns>
        private static IEnumerable<IBinaryTree<T>> AsPostOrderEnumerableIterator<T>(IBinaryTree<T> tree)
        {
            Stack<IBinaryTree<T>> nodes = new Stack<IBinaryTree<T>>();
            nodes.Push(tree);
            IBinaryTree<T> pre = null;
            while (nodes.Count != 0)
            {
                IBinaryTree<T> current = nodes.Peek();
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
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with level order.
        /// </summary>
        /// <typeparam name="T">The type of value the node contains.</typeparam>
        /// <param name="tree">The tree.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with level order.</returns>
        public static IEnumerable<IBinaryTree<T>> AsLevelOrderEnumerable<T>(this IBinaryTree<T> tree)
            => AsLevelOrderEnumerableIterator(tree ?? throw ExceptionHelper.ArgumentNull(nameof(tree)));
        /// <summary>
        /// Get an iterator with level order.
        /// </summary>
        /// <typeparam name="T">The type of value the node contains.</typeparam>
        /// <param name="tree">The tree.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with level order.</returns>
        private static IEnumerable<IBinaryTree<T>> AsLevelOrderEnumerableIterator<T>(IBinaryTree<T> tree)
        {
            Queue<IBinaryTree<T>> nodes = new Queue<IBinaryTree<T>>();
            nodes.Enqueue(tree);
            while (nodes.Count != 0)
            {
                IBinaryTree<T> current = nodes.Dequeue();
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
        /// <summary>
        /// Get an instance of <see cref="Graph{T}"/> class which is equivalent to the tree.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="tree">The specified tree.</param>
        /// <returns>An instance of <see cref="Graph{T}"/>.</returns>
        public static IGraph<T> ToGraph<T>(this IBinaryTree<T> tree)
        {
            if (tree == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(tree));
            }
            Graph<T> graph = new Graph<T>();
            Queue<IBinaryTree<T>> nodes = new Queue<IBinaryTree<T>>();
            nodes.Enqueue(tree);
            graph.Add(tree.Value);
            while (nodes.Count != 0)
            {
                IBinaryTree<T> current = nodes.Dequeue();
                IBinaryTree<T> child = current.LeftChild;
                if (child != null)
                {
                    nodes.Enqueue(child);
                    graph.Add(child.Value);
                    graph.AddEdge(current.Value, child.Value);
                }
                child = current.RightChild;
                if (child != null)
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
        public static IGraph<T> ToDirectedGraph<T>(this IBinaryTree<T> tree)
        {
            if (tree == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(tree));
            }
            Graph<T> graph = new Graph<T>();
            Queue<IBinaryTree<T>> nodes = new Queue<IBinaryTree<T>>();
            nodes.Enqueue(tree);
            graph.Add(tree.Value);
            while (nodes.Count != 0)
            {
                IBinaryTree<T> current = nodes.Dequeue();
                IBinaryTree<T> child = current.LeftChild;
                if (child != null)
                {
                    nodes.Enqueue(child);
                    graph.AddAsHead(child.Value, current.Value);
                }
                child = current.RightChild;
                if (child != null)
                {
                    nodes.Enqueue(child);
                    graph.AddAsHead(child.Value, current.Value);
                }
            }
            return graph;
        }
    }
    /// <summary>
    /// Represents a binary tree.
    /// </summary>
    /// <typeparam name="T">The type of value the node contains.</typeparam>
    [Serializable]
    public class BinaryTree<T> : IBinaryTree<T>
    {
        private T _value;
        private BinaryTree<T> _parent;
        private BinaryTree<T> _left;
        private BinaryTree<T> _right;
        /// <summary>
        /// Initialize an instance of <see cref="BinaryTree{T}"/>.
        /// </summary>
        public BinaryTree()
            : this(default)
        { }
        /// <summary>
        /// Initialize an instance of <see cref="BinaryTree{T}"/>.
        /// </summary>
        /// <param name="value">Value of the node.</param>
        public BinaryTree(T value)
        {
            _value = value;
        }
        /// <summary>
        /// Initialize an instance of <see cref="BinaryTree{T}"/>.
        /// </summary>
        /// <param name="value">Value of the node.</param>
        /// <param name="left">Left child of the node.</param>
        /// <param name="right">Right child of the node.</param>
        public BinaryTree(T value, BinaryTree<T> left, BinaryTree<T> right)
        {
            _value = value;
            _left = left;
            _right = right;
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
        public BinaryTree<T> Parent => _parent;
        /// <summary>
        /// The count of children.
        /// </summary>
        public int Count
        {
            get
            {
                int count = 0;
                if (_left != null)
                {
                    count++;
                }
                if (_right != null)
                {
                    count++;
                }
                return count;
            }
        }
        /// <summary>
        /// Left child of the node.
        /// </summary>
        public BinaryTree<T> LeftChild
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
        /// <summary>
        /// Right child of the node.
        /// </summary>
        public BinaryTree<T> RightChild
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
        /// <summary>
        /// Left child of the node.
        /// </summary>
        IBinaryTree<T> IBinaryTree<T>.LeftChild
        {
            get => LeftChild;
            set => LeftChild = (BinaryTree<T>)value;
        }
        /// <summary>
        /// Right child of the node.
        /// </summary>
        IBinaryTree<T> IBinaryTree<T>.RightChild
        {
            get => RightChild;
            set => RightChild = (BinaryTree<T>)value;
        }
        /// <summary>
        /// Parent of the node, null when the node is root node of a tree.
        /// </summary>
        ITreeBase<T> ITreeBase<T>.Parent => Parent;
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="BinaryTree{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{TNode}"/> for the <see cref="BinaryTree{T}"/>.</returns>
        public IEnumerator<BinaryTree<T>> GetEnumerator()
        {
            if (_left != null)
            {
                yield return _left;
            }
            if (_right != null)
            {
                yield return _right;
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="BinaryTree{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable"/> for the <see cref="BinaryTree{T}"/>.</returns>
        IEnumerator<ITreeBase<T>> IEnumerable<ITreeBase<T>>.GetEnumerator() => GetEnumerator();
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="BinaryTree{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable"/> for the <see cref="BinaryTree{T}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        /// <summary>
        /// Convert <see cref="BinaryTree{T}"/> to <typeparamref name="T"/> explicitly.
        /// </summary>
        /// <param name="node">Node to be converted.</param>
        /// <returns>Value of the node.</returns>
        public static explicit operator T(BinaryTree<T> node) => node.Value;
        /// <summary>
        /// Convert <typeparamref name="T"/> to <see cref="BinaryTree{T}"/> implicitly.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns>A new <see cref="BinaryTree{T}"/>.</returns>
        public static implicit operator BinaryTree<T>(T value) => new BinaryTree<T>(value);
        /// <summary>
        /// Returns a string that represents the value.
        /// </summary>
        /// <returns>A string that represents the value.</returns>
        public override string ToString() => _value.ToString();
    }
}
