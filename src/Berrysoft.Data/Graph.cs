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
        /// Add a vertex as head of specified vertexes.
        /// </summary>
        /// <param name="vertex">Value of the new vertex.</param>
        /// <param name="tails">The specified vertexes.</param>
        void AddAsHead(T vertex, params T[] tails);
        /// <summary>
        /// Add a vertex as tail of specified vertexes.
        /// </summary>
        /// <param name="vertex">Value of the new vertex.</param>
        /// <param name="heads">The specified vertexes.</param>
        void AddAsTail(T vertex, params T[] heads);
        /// <summary>
        /// Determines whether a <see cref="IGraph{T}"/> object contains specified vertex.
        /// </summary>
        /// <param name="vertex">The specified vertex.</param>
        /// <returns>true if the <see cref="IGraph{T}"/> contains the vertex; otherwise, false.</returns>
        bool Contains(T vertex);
        /// <summary>
        /// Remove a vertex. This will remove all the arcs related to the vertex.
        /// </summary>
        /// <param name="vertex">The vertex to be removed.</param>
        /// <returns>true if the vertex is removed successfully; otherwise, false.</returns>
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
        /// Add an edge.
        /// </summary>
        /// <param name="head1">One head of the edge.</param>
        /// <param name="head2">Another head of the edge.</param>
        void AddEdge(T head1, T head2);
        /// <summary>
        /// Determines whether a <see cref="IGraph{T}"/> object contains an arc of specified tail and head.
        /// </summary>
        /// <param name="tail">The tail of the arc.</param>
        /// <param name="head">The head of the arc.</param>
        /// <returns>true if the <see cref="IGraph{T}"/> contains the arc; otherwise, false.</returns>
        bool ContainsArc(T tail, T head);
        /// <summary>
        /// Determines whether a <see cref="IGraph{T}"/> object contains an edge of specified heads.
        /// </summary>
        /// <param name="head1">One head of the edge.</param>
        /// <param name="head2">Another head of the edge.</param>
        /// <returns>true if the <see cref="IGraph{T}"/> contains the edge; otherwise, false.</returns>
        bool ContainsEdge(T head1, T head2);
        /// <summary>
        /// Remove an arc of specified tail and head.
        /// </summary>
        /// <param name="tail">The tail of the arc.</param>
        /// <param name="head">The head of the arc.</param>
        /// <returns>true if the arc is removed successfully; otherwise, false.</returns>
        bool RemoveArc(T tail, T head);
        /// <summary>
        /// Remove an edge of specified heads.
        /// </summary>
        /// <param name="head1">One head of the edge.</param>
        /// <param name="head2">Another head of the edge.</param>
        /// <returns>true if the edge is removed successfully; otherwise, false.</returns>
        bool RemoveEdge(T head1, T head2);
        /// <summary>
        /// Clear all arcs.
        /// </summary>
        void ClearArc();
        /// <summary>
        /// Clear all arcs of specified vertex.
        /// </summary>
        /// <param name="vertex">The specified vertex.</param>
        void ClearArc(T vertex);
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
        /// Get a <see cref="Lookup{TKey, TElement}"/> contains all heads and their arcs.
        /// </summary>
        /// <returns>An instance of <see cref="Lookup{TKey, TElement}"/>.</returns>
        ILookup<T, T> GetHeads();
        /// <summary>
        /// Get a <see cref="IEnumerable{T}"/> of all heads of a specified tail.
        /// </summary>
        /// <param name="tail">The specified tail.</param>
        /// <returns>An instance of <see cref="IEnumerable{T}"/>.</returns>
        IEnumerable<T> GetHeads(T tail);
        /// <summary>
        /// Get a <see cref="Lookup{TKey, TElement}"/> contains all tails and their arcs.
        /// </summary>
        /// <returns>An instance of <see cref="Lookup{TKey, TElement}"/>.</returns>
        ILookup<T, T> GetTails();
        /// <summary>
        /// Get a <see cref="IEnumerable{T}"/> of all tails of a specified head.
        /// </summary>
        /// <param name="head">The specified head.</param>
        /// <returns>An instance of <see cref="IEnumerable{T}"/>.</returns>
        IEnumerable<T> GetTails(T head);
        /// <summary>
        /// Get a <see cref="IEnumerable{T}"/> of all heads of a specified tail.
        /// A return value indicates whether succeeded or failed.
        /// </summary>
        /// <param name="tail">The specified tail.</param>
        /// <param name="heads">An instance of <see cref="IEnumerable{T}"/>, when succeed; otherwise, null.</param>
        /// <returns>true if no exceptions; otherwise, false.</returns>
        bool TryGetHeads(T tail, out IEnumerable<T> heads);
        /// <summary>
        /// Get a <see cref="IEnumerable{T}"/> of all tails of a specified head.
        /// A return value indicates whether succeeded or failed.
        /// </summary>
        /// <param name="head">The specified head.</param>
        /// <param name="tails">An instance of <see cref="IEnumerable{T}"/>, when succeed; otherwise, null.</param>
        /// <returns>true if no exceptions; otherwise, false.</returns>
        bool TryGetTails(T head, out IEnumerable<T> tails);
    }
    #endregion
    /// <summary>
    /// Represents a graph data structure.
    /// </summary>
    /// <typeparam name="T">The type of vertex.</typeparam>
    public class Graph<T> : IGraph<T>
    {
        private HashSet<T> _vertexes;
        private KeyLookup<T, T> _arcs;
        /// <summary>
        /// Initialize an instance of <see cref="Graph{T}"/>.
        /// </summary>
        public Graph()
#if NETCOREAPP2_0 || NET472
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
#if NETCOREAPP2_0 || NET472
            : this(0, 0, comparer)
        { }
#else
        {
            _vertexes = new HashSet<T>(comparer);
            _arcs = new KeyLookup<T, T>(comparer, comparer);
        }
#endif
#if NETCOREAPP2_0 || NET472
        /// <summary>
        /// Initialize an instance of <see cref="Graph{T}"/>.
        /// </summary>
        /// <param name="vertexCapacity">The capacity of vertexes.</param>
        /// <param name="arcCapacity">The capacity of arcs.</param>
        /// <param name="comparer">An instance of <see cref="IEqualityComparer{T}"/>; default when <see langword="null"/>.</param>
        public Graph(int vertexCapacity, int arcCapacity, IEqualityComparer<T> comparer)
        {
            _vertexes = new HashSet<T>(vertexCapacity, comparer);
            _arcs = new KeyLookup<T, T>(arcCapacity, comparer, comparer);
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
            _arcs = new KeyLookup<T, T>(comparer, comparer);
        }
        public int Count => _vertexes.Count;
        public void Add(T vertex)
        {
            if (vertex == null)
            {
                throw ExceptionHelper.ArgumentNull(nameof(vertex));
            }
            _vertexes.Add(vertex);
        }
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
        public bool Contains(T vertex) => _vertexes.Contains(vertex);
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
        public void Clear()
        {
            _vertexes.Clear();
            _arcs.Clear();
        }
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
        public bool ContainsArc(T tail, T head) => _arcs.Contains(tail, head);
        public bool ContainsEdge(T head1, T head2) => _arcs.Contains(head1, head2) && _arcs.Contains(head2, head1);
        public bool RemoveArc(T tail, T head) => _arcs.Remove(tail, head);
        public bool RemoveEdge(T head1, T head2) => _arcs.Remove(head1, head2) || _arcs.Remove(head2, head1);
        public void ClearArc() => _arcs.Clear();
        public void ClearArc(T vertex)
        {
            ClearHeads(vertex);
            ClearTails(vertex);
        }
        public void ClearHeads(T tail) => _arcs.RemoveKey1(tail);
        public void ClearTails(T head) => _arcs.RemoveKey2(head);
        public ILookup<T, T> GetHeads() => _arcs.ToLookupFromKey1();
        public IEnumerable<T> GetHeads(T tail) => _arcs.GetValuesFromKey1(tail);
        public ILookup<T, T> GetTails() => _arcs.ToLookupFromKey2();
        public IEnumerable<T> GetTails(T head) => _arcs.GetValuesFromKey2(head);
        public bool TryGetHeads(T tail, out IEnumerable<T> heads) => _arcs.TryGetValuesFromKey1(tail, out heads);
        public bool TryGetTails(T head, out IEnumerable<T> tails) => _arcs.TryGetValuesFromKey2(head, out tails);
    }
}
