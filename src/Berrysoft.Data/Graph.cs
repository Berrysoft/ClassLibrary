﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Berrysoft.Data
{
    #region Interfaces
    public interface IGraph<T>
    {
        int Count { get; }
        void Add(T vertex);
        void AddAsHead(T vertex, params T[] tails);
        void AddAsTail(T vertex, params T[] heads);
        bool Contains(T vertex);
        bool Remove(T vertex);
        void Clear();
        void AddArc(T tail, T head);
        void AddEdge(T tail, T head);
        bool ContainsArc(T tail, T head);
        bool ContainsEdge(T tail, T head);
        bool RemoveArc(T tail, T head);
        bool RemoveEdge(T tail, T head);
        void ClearArc();
        void ClearArc(T vertex);
        void ClearHeads(T tail);
        void ClearTails(T head);
        ILookup<T, T> GetHeads();
        IEnumerable<T> GetHeads(T tail);
        ILookup<T, T> GetTails();
        IEnumerable<T> GetTails(T head);
        bool TryGetHeads(T tail, out IEnumerable<T> heads);
        bool TryGetTails(T head, out IEnumerable<T> tails);
    }
    #endregion
    public class Graph<T> : IGraph<T>
    {
        private HashSet<T> _vertexes;
        private KeyLookup<T, T> _arcs;
#if NETCOREAPP2_0 || NET472
        public Graph()
            : this(0, 0, null)
        { }
        public Graph(IEqualityComparer<T> comparer)
            : this(0, 0, comparer)
        { }
        public Graph(int vertexCapacity, int arcCapacity, IEqualityComparer<T> comparer)
        {
            _vertexes = new HashSet<T>(vertexCapacity, comparer);
            _arcs = new KeyLookup<T, T>(arcCapacity, comparer, comparer);
        }
#else
        public Graph()
            : this((IEqualityComparer<T>)null)
        { }
        public Graph(IEqualityComparer<T> comparer)
        {
            _vertexes = new HashSet<T>(comparer);
            _arcs = new KeyLookup<T, T>(comparer, comparer);
        }
#endif
        public Graph(IEnumerable<T> vertexes)
            : this(vertexes, null)
        { }
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
        public void AddEdge(T tail, T head)
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
                _arcs.Add(head, tail);
            }
            else
            {
                throw ExceptionHelper.KeyNotFound();
            }
        }
        public bool ContainsArc(T tail, T head) => _arcs.Contains(tail, head);
        public bool ContainsEdge(T tail, T head) => _arcs.Contains(tail, head) && _arcs.Contains(head, tail);
        public bool RemoveArc(T tail, T head) => _arcs.Remove(tail, head);
        public bool RemoveEdge(T tail, T head) => _arcs.Remove(tail, head) || _arcs.Remove(head, tail);
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