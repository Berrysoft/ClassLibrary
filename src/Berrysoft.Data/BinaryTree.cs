using System;
using System.Collections.Generic;

namespace Berrysoft.Data
{
    #region Interfaces
    /// <summary>
    /// Exposes members of a binary tree data structure.
    /// </summary>
    /// <typeparam name="TValue">The type of value the node contains.</typeparam>
    /// <typeparam name="TNode">The type of node.</typeparam>
    public interface IBinaryNode<TValue, TNode> : INodeBase<TValue, TNode>
        where TNode : IBinaryNode<TValue, TNode>
    {
        /// <summary>
        /// Left child of the tree.
        /// </summary>
        TNode LeftChild { get; set; }
        /// <summary>
        /// Right child of the tree.
        /// </summary>
        TNode RightChild { get; set; }
    }
    #endregion
    /// <summary>
    /// Represents a binary tree with a root <see cref="BinaryNode{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of value the node contains.</typeparam>
    /// <example>
    /// This is an example to instantiatea binary tree with its pre and in order.
    /// <code language="C#"><![CDATA[
    /// static BinaryTree<int> tree;
    /// static BinaryNode<int> current;
    /// static void Main(string[] args)
    /// {
    ///     tree = new BinaryTree<int>();
    ///     Console.WriteLine("Please enter the values by pre order, splited by space:");
    ///     int[] front = Console.ReadLine().Split(' ').Select(str => int.Parse(str)).ToArray();
    ///     Console.WriteLine("Please enter the values by in order, splited by space:");
    ///     int[] mid = Console.ReadLine().Split(' ').Select(str => int.Parse(str)).ToArray();
    ///     if (front.Length != mid.Length)
    ///     {
    ///         Console.WriteLine("The length of the two arrays should be equal.");
    ///         return;
    ///     }
    ///     tree.Root.LeftChild = null;
    ///     tree.Root.RightChild = null;
    ///     current = tree.Root;
    ///     Create(front, mid);
    ///     Console.WriteLine("The post order of this tree is:");
    ///     Console.WriteLine(String.Join(" ", tree.AsPostOrderEnumerable().Select(node => node.ToString()).ToArray()));
    /// }
    /// static void Create(in Span<int> front, in Span<int> mid)
    /// {
    ///     int n = front.Length;
    ///     BinaryNode<int> tr = current;
    ///     tr.Value = front[0];
    ///     int i;
    ///     for (i = 0; i < n; i++)
    ///     {
    ///         if (mid[i] == front[0])
    ///         {
    ///             break;
    ///         }
    ///     }
    ///     if (i > 0)
    ///     {
    ///         current = (tr.LeftChild = new BinaryNode<int>());
    ///         Create(front.Slice(1, i), mid);
    ///     }
    ///     if (n - 1 - i > 0)
    ///     {
    ///         current = (tr.RightChild = new BinaryNode<int>());
    ///         Create(front.Slice(i + 1, n - 1 - i), mid.Slice(i + 1, n - 1 - i));
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    public class BinaryTree<T> : ITree<T, BinaryNode<T>>
    {
        private BinaryNode<T> _root;
        /// <summary>
        /// Initialize an instance of <see cref="BinaryTree{T}"/>.
        /// </summary>
        public BinaryTree()
        {
            _root = new BinaryNode<T>();
        }
        /// <summary>
        /// Initialize an instance of <see cref="BinaryTree{T}"/>.
        /// </summary>
        /// <param name="value">The value of root node.</param>
        public BinaryTree(T value)
        {
            _root = new BinaryNode<T>(value);
        }
        /// <summary>
        /// Initialize an instance of <see cref="BinaryTree{T}"/>.
        /// </summary>
        /// <param name="root">Root node.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="root"/> is null.</exception>
        /// <exception cref="ArgumentException">When <paramref name="root"/> has a parent.</exception>
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
        /// <summary>
        /// The root node of the tree.
        /// </summary>
        public BinaryNode<T> Root => _root;
        /// <summary>
        /// Get depth of the tree.
        /// </summary>
        /// <returns>The depth of the tree.</returns>
        public int GetDepth()
        {
            int GetDepthInternal(BinaryNode<T> root, int depth)
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
            return GetDepthInternal(_root, 1);
        }
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with order of depth-first-search.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of depth-first-search.</returns>
        public IEnumerable<BinaryNode<T>> AsDFSEnumerable() => AsPreOrderEnumerableIterator();
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with pre order.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> with pre order.</returns>
        public IEnumerable<BinaryNode<T>> AsPreOrderEnumerable() => AsPreOrderEnumerableIterator();
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with pre order.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> with pre order.</returns>
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
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with in order.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> with in order.</returns>
        public IEnumerable<BinaryNode<T>> AsInOrderEnumerable() => AsInOrderEnumerableIterator();
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with in order.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> with in order.</returns>
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
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with post order.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> with post order.</returns>
        public IEnumerable<BinaryNode<T>> AsPostOrderEnumerable() => AsPostOrderEnumerableIterator();
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with post order.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> with post order.</returns>
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
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with order of breadth-first-search.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of breadth-first-search.</returns>
        public IEnumerable<BinaryNode<T>> AsBFSEnumerable() => AsLevelOrderEnumerableIterator();
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with level order.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> with level order.</returns>
        public IEnumerable<BinaryNode<T>> AsLevelOrderEnumerable() => AsLevelOrderEnumerableIterator();
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with level order.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> with level order.</returns>
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
    /// <summary>
    /// Represents a binary node of a <see cref="BinaryTree{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of value the node contains.</typeparam>
    public class BinaryNode<T> : IBinaryNode<T, BinaryNode<T>>
    {
        private T _value;
        private BinaryNode<T> _parent;
        private BinaryNode<T> _left;
        private BinaryNode<T> _right;
        /// <summary>
        /// Initialize an instance of <see cref="BinaryNode{T}"/>.
        /// </summary>
        public BinaryNode()
            : this(default)
        { }
        /// <summary>
        /// Initialize an instance of <see cref="BinaryNode{T}"/>.
        /// </summary>
        /// <param name="value">Value of the node.</param>
        public BinaryNode(T value)
        {
            _value = value;
        }
        /// <summary>
        /// Initialize an instance of <see cref="BinaryNode{T}"/>.
        /// </summary>
        /// <param name="value">Value of the node.</param>
        /// <param name="left">Left child of the node.</param>
        /// <param name="right">Right child of the node.</param>
        public BinaryNode(T value, BinaryNode<T> left, BinaryNode<T> right)
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
        public BinaryNode<T> Parent => _parent;
        /// <summary>
        /// Left child of the node.
        /// </summary>
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
        /// <summary>
        /// Right child of the node.
        /// </summary>
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
        /// <summary>
        /// Get an <see cref="IEnumerable{TNode}"/> of its children.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{TNode}"/> of its children.</returns>
        public IEnumerable<BinaryNode<T>> AsEnumerable() => new BinaryNode<T>[] { _left, _right };
        /// <summary>
        /// Convert <see cref="BinaryNode{T}"/> to <typeparamref name="T"/> explicitly.
        /// </summary>
        /// <param name="node">Node to be converted.</param>
        /// <returns>Value of the node.</returns>
        public static explicit operator T(BinaryNode<T> node) => node.Value;
        /// <summary>
        /// Convert <typeparamref name="T"/> to <see cref="BinaryNode{T}"/> implicitly.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns>A new <see cref="BinaryNode{T}"/>.</returns>
        public static implicit operator BinaryNode<T>(T value) => new BinaryNode<T>(value);
        /// <summary>
        /// Returns a string that represents the value.
        /// </summary>
        /// <returns>A string that represents the value.</returns>
        public override string ToString() => _value.ToString();
    }
}
