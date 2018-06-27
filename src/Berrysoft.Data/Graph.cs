using System;
using System.Collections.Generic;
using System.Linq;

namespace Berrysoft.Data
{
    #region Interfaces
    /// <summary>
    /// Exposes members of a graph data structure.
    /// </summary>
    /// <typeparam name="T">The type of vertex.</typeparam>
    public interface IGraph<T>
    {
        /// <summary>
        /// Count of vertexes.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Add a vertex.
        /// </summary>
        /// <param name="vertex">Value of the new vertex.</param>
        void Add(T vertex);
        /// <summary>
        /// Determines whether an <see cref="IGraph{T}"/> object contains specified vertex.
        /// </summary>
        /// <param name="vertex">The specified vertex.</param>
        /// <returns><see langword="true"/> if the <see cref="IGraph{T}"/> contains the vertex; otherwise, <see langword="false"/>.</returns>
        bool Contains(T vertex);
        /// <summary>
        /// Remove a vertex. This will remove all the arcs related to the vertex.
        /// </summary>
        /// <param name="vertex">The vertex to be removed.</param>
        /// <returns><see langword="true"/> if the vertex is removed successfully; otherwise, <see langword="false"/>.</returns>
        bool Remove(T vertex);
        /// <summary>
        /// Clear all vertexes. This will remove all the arcs.
        /// </summary>
        void Clear();
        /// <summary>
        /// Add an arc.
        /// </summary>
        /// <param name="tail">The tail of the arc.</param>
        /// <param name="head">The head of the arc.</param>
        void AddArc(T tail, T head);
        /// <summary>
        /// Determines whether an <see cref="IGraph{T}"/> object contains an arc of specified tail and head.
        /// </summary>
        /// <param name="tail">The tail of the arc.</param>
        /// <param name="head">The head of the arc.</param>
        /// <returns><see langword="true"/> if the <see cref="IGraph{T}"/> contains the arc; otherwise, <see langword="false"/>.</returns>
        bool ContainsArc(T tail, T head);
        /// <summary>
        /// Remove an arc of specified tail and head.
        /// </summary>
        /// <param name="tail">The tail of the arc.</param>
        /// <param name="head">The head of the arc.</param>
        /// <returns><see langword="true"/> if the arc is removed successfully; otherwise, <see langword="false"/>.</returns>
        bool RemoveArc(T tail, T head);
        /// <summary>
        /// Clear all arcs.
        /// </summary>
        void ClearArc();
        /// <summary>
        /// Clear all arcs whose tail is the specified vertex.
        /// </summary>
        /// <param name="tail">The specified vertex.</param>
        void ClearHeads(T tail);
        /// <summary>
        /// Clear all arcs whose head is the specified vertex.
        /// </summary>
        /// <param name="head">The specified vertex.</param>
        void ClearTails(T head);
        /// <summary>
        /// Get a <see cref="ILookup{TKey, TElement}"/> contains all heads and their arcs.
        /// </summary>
        /// <returns>An instance of <see cref="ILookup{TKey, TElement}"/>.</returns>
        ILookup<T, T> GetHeads();
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> of all heads of a specified tail.
        /// </summary>
        /// <param name="tail">The specified tail.</param>
        /// <returns>An instance of <see cref="IEnumerable{T}"/>.</returns>
        ICollection<T> GetHeads(T tail);
        /// <summary>
        /// Get a <see cref="ILookup{TKey, TElement}"/> contains all tails and their arcs.
        /// </summary>
        /// <returns>An instance of <see cref="ILookup{TKey, TElement}"/>.</returns>
        ILookup<T, T> GetTails();
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> of all tails of a specified head.
        /// </summary>
        /// <param name="head">The specified head.</param>
        /// <returns>An instance of <see cref="IEnumerable{T}"/>.</returns>
        ICollection<T> GetTails(T head);
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> of all heads of a specified tail.
        /// A return value indicates whether succeeded or failed.
        /// </summary>
        /// <param name="tail">The specified tail.</param>
        /// <param name="heads">An instance of <see cref="IEnumerable{T}"/>, when succeed; otherwise, null.</param>
        /// <returns>true if no exceptions; otherwise, false.</returns>
        bool TryGetHeads(T tail, out ICollection<T> heads);
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> of all tails of a specified head.
        /// A return value indicates whether succeeded or failed.
        /// </summary>
        /// <param name="head">The specified head.</param>
        /// <param name="tails">An instance of <see cref="IEnumerable{T}"/>, when succeed; otherwise, null.</param>
        /// <returns>true if no exceptions; otherwise, false.</returns>
        bool TryGetTails(T head, out ICollection<T> tails);
    }
    #endregion
    public static partial class Enumerable
    {
        /// <summary>
        /// Determines whether a vertex is in a loop of an instance of <see cref="IGraph{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of vertex.</typeparam>
        /// <param name="graph">The graph to enumerate.</param>
        /// <param name="vertex">The specified vertex.</param>
        /// <returns><see langword="true"/> if the <paramref name="vertex"/> is in a loop; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">When <paramref name="vertex"/> is not contained in the graph.</exception>
        public static bool IsInLoop<T>(this IGraph<T> graph, T vertex)
        {
            if (graph == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(graph));
            }
            if (!graph.Contains(vertex))
            {
                throw ExceptionHelper.KeyNotFound();
            }
            Stack<T> nodes = new Stack<T>();
            HashSet<T> visited = new HashSet<T>();
            nodes.Push(vertex);
            T last = default;
            while (nodes.Count != 0)
            {
                T current;
                do
                {
                    if (nodes.Count == 0)
                    {
                        goto ret;
                    }
                    current = nodes.Pop();
                }
                while (visited.Contains(current));
                visited.Add(current);
                if (graph.TryGetHeads(current, out var heads))
                {
                    foreach (var child in heads)
                    {
                        if (!visited.Contains(child))
                        {
                            nodes.Push(child);
                        }
                        else if (EqualityComparer<T>.Default.Equals(vertex, child) && !EqualityComparer<T>.Default.Equals(last, child))
                        {
                            return true;
                        }
                    }
                }
                last = current;
            }
            ret:
            return false;
        }
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with order of depth-first-search.
        /// </summary>
        /// <typeparam name="T">The type of vertex.</typeparam>
        /// <param name="graph">The graph to enumerate.</param>
        /// <param name="root">The first vertex to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of depth-first-search.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">When <paramref name="root"/> is not contained in the graph.</exception>
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
        /// <summary>
        /// Get an iterator with order of depth-first-search.
        /// </summary>
        /// <typeparam name="T">The type of vertex.</typeparam>
        /// <param name="graph">The graph to enumerate.</param>
        /// <param name="root">The first vertex to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of depth-first-search.</returns>
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
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with order of depth-first-search with current path.
        /// </summary>
        /// <typeparam name="T">The type of vertex.</typeparam>
        /// <param name="graph">The graph to enumerate.</param>
        /// <param name="root">The first vertex to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of depth-first-search with current path.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">When <paramref name="root"/> is not contained in the graph.</exception>
        public static IEnumerable<(T Vertex, IReadOnlyCollection<T> Path)> AsDFSWithPath<T>(this IGraph<T> graph, T root)
        {
            if (graph == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(graph));
            }
            if (!graph.Contains(root))
            {
                throw ExceptionHelper.KeyNotFound();
            }
            return AsDFSWithPathIterator(graph, root);
        }
        /// <summary>
        /// Get an iterator with order of depth-first-search with current path.
        /// </summary>
        /// <typeparam name="T">The type of vertex.</typeparam>
        /// <param name="graph">The graph to enumerate.</param>
        /// <param name="root">The first vertex to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of depth-first-search with current path.</returns>
        private static IEnumerable<(T Vertex, IReadOnlyCollection<T> Path)> AsDFSWithPathIterator<T>(this IGraph<T> graph, T root)
        {
            Stack<(int Index, T Value)> nodes = new Stack<(int Index, T Value)>();
            HashSet<T> visited = new HashSet<T>();
            List<T> list = new List<T>();
            nodes.Push((0, root));
            while (nodes.Count != 0)
            {
                T current;
                int index;
                do
                {
                    if (nodes.Count == 0)
                    {
                        yield break;
                    }
                    (index, current) = nodes.Pop();
                }
                while (visited.Contains(current));
                if (index < list.Count)
                {
                    list.RemoveRange(index, list.Count - index);
                }
                list.Add(current);
                visited.Add(current);
                yield return (current, list);
                index++;
                if (graph.TryGetHeads(current, out var heads))
                {
                    foreach (var child in heads.Reverse())
                    {
                        if (!visited.Contains(child))
                        {
                            nodes.Push((index, child));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with order of breadth-first-search.
        /// </summary>
        /// <typeparam name="T">The type of vertex.</typeparam>
        /// <param name="graph">The graph to enumerate.</param>
        /// <param name="root">The first vertex to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of breadth-first-search.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">When <paramref name="root"/> is not contained in the graph.</exception>
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
        /// <summary>
        /// Get an iterator with order of breadth-first-search.
        /// </summary>
        /// <typeparam name="T">The type of vertex.</typeparam>
        /// <param name="graph">The graph to enumerate.</param>
        /// <param name="root">The first vertex to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of breadth-first-search.</returns>
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
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> with order of breadth-first-search with current path.
        /// </summary>
        /// <typeparam name="T">The type of vertex.</typeparam>
        /// <param name="graph">The graph to enumerate.</param>
        /// <param name="root">The first vertex to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of breadth-first-search with current path.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">When <paramref name="root"/> is not contained in the graph.</exception>
        public static IEnumerable<(T Vertex, IReadOnlyCollection<T> Path)> AsBFSWithPath<T>(this IGraph<T> graph, T root)
        {
            if (graph == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(graph));
            }
            if (!graph.Contains(root))
            {
                throw ExceptionHelper.KeyNotFound();
            }
            return AsBFSWithPathIterator(graph, root);
        }
        /// <summary>
        /// Get an iterator with order of breadth-first-search with current path.
        /// </summary>
        /// <typeparam name="T">The type of vertex.</typeparam>
        /// <param name="graph">The graph to enumerate.</param>
        /// <param name="root">The first vertex to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with order of breadth-first-search with current path.</returns>
        private static IEnumerable<(T Vertex, IReadOnlyCollection<T> Path)> AsBFSWithPathIterator<T>(IGraph<T> graph, T root)
        {
            Queue<(int Index, T Value)> nodes = new Queue<(int Index, T Value)>();
            HashSet<T> visited = new HashSet<T>();
            List<T> list = new List<T>();
            nodes.Enqueue((0, root));
            while (nodes.Count != 0)
            {
                T current;
                int index;
                do
                {
                    if (nodes.Count == 0)
                    {
                        yield break;
                    }
                    (index, current) = nodes.Dequeue();
                }
                while (visited.Contains(current));
                if (index < list.Count)
                {
                    list.RemoveRange(index, list.Count - index);
                }
                list.Add(current);
                visited.Add(current);
                yield return (current, list);
                index++;
                if (graph.TryGetHeads(current, out var heads))
                {
                    foreach (var child in heads)
                    {
                        if (!visited.Contains(child))
                        {
                            nodes.Enqueue((index, child));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Get a tree with order of depth-first-search.
        /// </summary>
        /// <typeparam name="T">The type of vertex.</typeparam>
        /// <param name="graph">The graph to convert.</param>
        /// <param name="root">The value of root node.</param>
        /// <returns>A tree with order of depth-first-search.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">When <paramref name="root"/> is not contained in the graph.</exception>
        public static ITree<T> ToDFSTree<T>(this IGraph<T> graph, T root)
        {
            if (graph == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(graph));
            }
            if (!graph.Contains(root))
            {
                throw ExceptionHelper.KeyNotFound();
            }
            Tree<T> result = new Tree<T>(root);
            Stack<Tree<T>> nodes = new Stack<Tree<T>>();
            HashSet<T> visited = new HashSet<T>();
            nodes.Push(result);
            while (nodes.Count != 0)
            {
                Tree<T> current;
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
                            Tree<T> nc = new Tree<T>(child);
                            nodes.Push(nc);
                            current.Add(nc);
                        }
                    }
                }
            }
            ret:
            return result;
        }
        /// <summary>
        /// Get a tree with order of breadth-first-search.
        /// </summary>
        /// <typeparam name="T">The type of vertex.</typeparam>
        /// <param name="graph">The graph to convert.</param>
        /// <param name="root">The value of root node.</param>
        /// <returns>A tree with order of breadth-first-search.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">When <paramref name="root"/> is not contained in the graph.</exception>
        public static ITree<T> ToBFSTree<T>(this IGraph<T> graph, T root)
        {
            if (graph == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(graph));
            }
            if (!graph.Contains(root))
            {
                throw ExceptionHelper.KeyNotFound();
            }
            Tree<T> result = new Tree<T>(root);
            Queue<Tree<T>> nodes = new Queue<Tree<T>>();
            HashSet<T> visited = new HashSet<T>();
            nodes.Enqueue(result);
            while (nodes.Count != 0)
            {
                Tree<T> current;
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
                            Tree<T> nc = new Tree<T>(child);
                            nodes.Enqueue(nc);
                            current.Add(nc);
                        }
                    }
                }
            }
            ret:
            return result;
        }
    }
    /// <summary>
    /// Represents a graph data structure.
    /// </summary>
    /// <typeparam name="T">The type of vertex.</typeparam>
    [Serializable]
    public class Graph<T> : IGraph<T>
    {
        private HashSet<T> _vertexes;
        private MultiMap<T, T> _arcs;
        /// <summary>
        /// Initialize an instance of <see cref="Graph{T}"/>.
        /// </summary>
        public Graph()
#if NETCOREAPP2_1 || NET472
            : this(0, 0, null)
#else
            : this((IEqualityComparer<T>)null)
#endif
        { }
        /// <summary>
        /// Initialize an instance of <see cref="Graph{T}"/>.
        /// </summary>
        /// <param name="comparer">An instance of <see cref="IEqualityComparer{T}"/>; default when <see langword="null"/>.</param>
        public Graph(IEqualityComparer<T> comparer)
#if NETCOREAPP2_1 || NET472
            : this(0, 0, comparer)
        { }
#else
        {
            _vertexes = new HashSet<T>(comparer);
            _arcs = new MultiMap<T, T>(comparer, comparer);
        }
#endif
#if NETCOREAPP2_1 || NET472
        /// <summary>
        /// Initialize an instance of <see cref="Graph{T}"/>.
        /// </summary>
        /// <param name="vertexCapacity">The capacity of vertexes.</param>
        /// <param name="arcCapacity">The capacity of arcs.</param>
        /// <param name="comparer">An instance of <see cref="IEqualityComparer{T}"/>; default when <see langword="null"/>.</param>
        public Graph(int vertexCapacity, int arcCapacity, IEqualityComparer<T> comparer)
        {
            _vertexes = new HashSet<T>(vertexCapacity, comparer);
            _arcs = new MultiMap<T, T>(arcCapacity, comparer, comparer);
        }
#endif
        /// <summary>
        /// Initialize an instance of <see cref="Graph{T}"/>.
        /// </summary>
        /// <param name="vertexes">The specified collection of vertexes.</param>
        public Graph(IEnumerable<T> vertexes)
            : this(vertexes, null)
        { }
        /// <summary>
        /// Initialize an instance of <see cref="Graph{T}"/>.
        /// </summary>
        /// <param name="vertexes">The specified collection of vertexes.</param>
        /// <param name="comparer">An instance of <see cref="IEqualityComparer{T}"/>; default when <see langword="null"/>.</param>
        public Graph(IEnumerable<T> vertexes, IEqualityComparer<T> comparer)
        {
            _vertexes = new HashSet<T>(vertexes);
            _arcs = new MultiMap<T, T>(comparer, comparer);
        }
        /// <summary>
        /// Count of vertexes.
        /// </summary>
        public int Count => _vertexes.Count;
        /// <summary>
        /// Add a vertex.
        /// </summary>
        /// <param name="vertex">Value of the new vertex.</param>
        public void Add(T vertex)
        {
            if (vertex == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(vertex));
            }
            _vertexes.Add(vertex);
        }
        /// <summary>
        /// Add a vertex as head of specified vertexes.
        /// </summary>
        /// <param name="vertex">Value of the new vertex.</param>
        /// <param name="tails">The specified vertexes.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="vertex"/> is <see langword="null"/>.</exception>
        public void AddAsHead(T vertex, params T[] tails)
        {
            if (vertex == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(vertex));
            }
            _vertexes.Add(vertex);
            foreach (T tail in tails)
            {
                if (_vertexes.Contains(tail))
                {
                    _arcs.Add(tail, vertex);
                }
            }
        }
        /// <summary>
        /// Add a vertex as tail of specified vertexes.
        /// </summary>
        /// <param name="vertex">Value of the new vertex.</param>
        /// <param name="heads">The specified vertexes.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="vertex"/> is <see langword="null"/>.</exception>
        public void AddAsTail(T vertex, params T[] heads)
        {
            if (vertex == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(vertex));
            }
            _vertexes.Add(vertex);
            foreach (T head in heads)
            {
                if (_vertexes.Contains(head))
                {
                    _arcs.Add(vertex, head);
                }
            }
        }
        /// <summary>
        /// Determines whether a <see cref="Graph{T}"/> object contains specified vertex.
        /// </summary>
        /// <param name="vertex">The specified vertex.</param>
        /// <returns><see langword="true"/> if the <see cref="Graph{T}"/> contains the vertex; otherwise, <see langword="false"/>.</returns>
        public bool Contains(T vertex) => _vertexes.Contains(vertex);
        /// <summary>
        /// Remove a vertex. This will remove all the arcs related to the vertex.
        /// </summary>
        /// <param name="vertex">The vertex to be removed.</param>
        /// <returns><see langword="true"/> if the vertex is removed successfully; otherwise, <see langword="false"/>.</returns>
        public bool Remove(T vertex)
        {
            if (_vertexes.Remove(vertex))
            {
                _arcs.RemoveKey1(vertex);
                _arcs.RemoveKey2(vertex);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Clear all vertexes. This will remove all the arcs.
        /// </summary>
        public void Clear()
        {
            _vertexes.Clear();
            _arcs.Clear();
        }
        /// <summary>
        /// Add an arc.
        /// </summary>
        /// <param name="tail">The tail of the arc.</param>
        /// <param name="head">The head of the arc.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="tail"/> or <paramref name="head"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">When <paramref name="tail"/> or <paramref name="head"/> isn't found.</exception>
        public void AddArc(T tail, T head)
        {
            if (tail == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(tail));
            }
            if (head == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(head));
            }
            if (_vertexes.Contains(tail) && _vertexes.Contains(head))
            {
                _arcs.Add(tail, head);
            }
            else
            {
                throw ExceptionHelper.KeyNotFound();
            }
        }
        /// <summary>
        /// Add an edge.
        /// </summary>
        /// <param name="head1">One head of the edge.</param>
        /// <param name="head2">Another head of the edge.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="head1"/> or <paramref name="head2"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">When <paramref name="head1"/> or <paramref name="head2"/> isn't found.</exception>
        public void AddEdge(T head1, T head2)
        {
            if (head1 == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(head1));
            }
            if (head2 == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(head2));
            }
            if (_vertexes.Contains(head1) && _vertexes.Contains(head2))
            {
                _arcs.Add(head1, head2);
                _arcs.Add(head2, head1);
            }
            else
            {
                throw ExceptionHelper.KeyNotFound();
            }
        }
        /// <summary>
        /// Determines whether a <see cref="Graph{T}"/> object contains an arc of specified tail and head.
        /// </summary>
        /// <param name="tail">The tail of the arc.</param>
        /// <param name="head">The head of the arc.</param>
        /// <returns><see langword="true"/> if the <see cref="Graph{T}"/> contains the arc; otherwise, <see langword="false"/>.</returns>
        public bool ContainsArc(T tail, T head) => _arcs.Contains(tail, head);
        /// <summary>
        /// Determines whether a <see cref="Graph{T}"/> object contains an edge of specified heads.
        /// </summary>
        /// <param name="head1">One head of the edge.</param>
        /// <param name="head2">Another head of the edge.</param>
        /// <returns><see langword="true"/> if the <see cref="Graph{T}"/> contains the edge; otherwise, <see langword="false"/>.</returns>
        public bool ContainsEdge(T head1, T head2) => _arcs.Contains(head1, head2) && _arcs.Contains(head2, head1);
        /// <summary>
        /// Remove an arc of specified tail and head.
        /// </summary>
        /// <param name="tail">The tail of the arc.</param>
        /// <param name="head">The head of the arc.</param>
        /// <returns><see langword="true"/> if the arc is removed successfully; otherwise, <see langword="false"/>.</returns>
        public bool RemoveArc(T tail, T head) => _arcs.Remove(tail, head);
        /// <summary>
        /// Remove an edge of specified heads.
        /// </summary>
        /// <param name="head1">One head of the edge.</param>
        /// <param name="head2">Another head of the edge.</param>
        /// <returns><see langword="true"/> if the edge is removed successfully; otherwise, <see langword="false"/>.</returns>
        public bool RemoveEdge(T head1, T head2) => _arcs.Remove(head1, head2) || _arcs.Remove(head2, head1);
        /// <summary>
        /// Clear all arcs.
        /// </summary>
        public void ClearArc() => _arcs.Clear();
        /// <summary>
        /// Clear all arcs of specified vertex.
        /// </summary>
        /// <param name="vertex">The specified vertex.</param>
        public void ClearArc(T vertex)
        {
            ClearHeads(vertex);
            ClearTails(vertex);
        }
        /// <summary>
        /// Clear all arcs whose tail is the specified vertex.
        /// </summary>
        /// <param name="tail">The specified vertex.</param>
        public void ClearHeads(T tail) => _arcs.RemoveKey1(tail);
        /// <summary>
        /// Clear all arcs whose head is the specified vertex.
        /// </summary>
        /// <param name="head">The specified vertex.</param>
        public void ClearTails(T head) => _arcs.RemoveKey2(head);
        /// <summary>
        /// Get an <see cref="ILookup{TKey, TElement}"/> contains all heads and their arcs.
        /// </summary>
        /// <returns>An instance of <see cref="ILookup{TKey, TElement}"/>.</returns>
        public ILookup<T, T> GetHeads() => _arcs.ToLookupFromKey1();
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> of all heads of a specified tail.
        /// </summary>
        /// <param name="tail">The specified tail.</param>
        /// <returns>An instance of <see cref="IEnumerable{T}"/>.</returns>
        public ICollection<T> GetHeads(T tail) => _arcs.GetValuesFromKey1(tail);
        /// <summary>
        /// Get an <see cref="ILookup{TKey, TElement}"/> contains all tails and their arcs.
        /// </summary>
        /// <returns>An instance of <see cref="ILookup{TKey, TElement}"/>.</returns>
        public ILookup<T, T> GetTails() => _arcs.ToLookupFromKey2();
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> of all tails of a specified head.
        /// </summary>
        /// <param name="head">The specified head.</param>
        /// <returns>An instance of <see cref="IEnumerable{T}"/>.</returns>
        public ICollection<T> GetTails(T head) => _arcs.GetValuesFromKey2(head);
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> of all heads of a specified tail.
        /// A return value indicates whether succeeded or failed.
        /// </summary>
        /// <param name="tail">The specified tail.</param>
        /// <param name="heads">An instance of <see cref="IEnumerable{T}"/>, when succeed; otherwise, null.</param>
        /// <returns>true if no exceptions; otherwise, false.</returns>
        public bool TryGetHeads(T tail, out ICollection<T> heads) => _arcs.TryGetValuesFromKey1(tail, out heads);
        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> of all tails of a specified head.
        /// A return value indicates whether succeeded or failed.
        /// </summary>
        /// <param name="head">The specified head.</param>
        /// <param name="tails">An instance of <see cref="IEnumerable{T}"/>, when succeed; otherwise, null.</param>
        /// <returns>true if no exceptions; otherwise, false.</returns>
        public bool TryGetTails(T head, out ICollection<T> tails) => _arcs.TryGetValuesFromKey2(head, out tails);
    }
}
