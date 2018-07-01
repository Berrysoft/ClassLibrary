using System;
using System.Diagnostics;
using System.Linq;

namespace Berrysoft.Data
{
    internal sealed class IMapDebugView<K, V>
    {
        private readonly IMap<K, V> map;
        public IMapDebugView(IMap<K, V> map)
        {
            this.map = map ?? throw ExceptionHelper.ArgumentNull(nameof(map));
        }
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyPair<K, V>[] Items
        {
            get
            {
                KeyPair<K, V>[] array = new KeyPair<K, V>[map.Count];
                map.CopyTo(array, 0);
                return array;
            }
        }
    }
    internal sealed class IMutableLookupDebugView<K, V>
    {
        private readonly IMutableLookup<K, V> lookup;
        public IMutableLookupDebugView(IMutableLookup<K, V> lookup)
        {
            this.lookup = lookup ?? throw ExceptionHelper.ArgumentNull(nameof(lookup));
        }
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IGrouping<K, V>[] Items => lookup.ToArray();
    }
    internal sealed class ICountableGroupingDebugView<K,V>
    {
        private readonly ICountableGrouping<K, V> grouping;
        public ICountableGroupingDebugView(ICountableGrouping<K,V> grouping)
        {
            this.grouping = grouping ?? throw ExceptionHelper.ArgumentNull(nameof(grouping));
        }
        public K Key => grouping.Key;
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public V[] Items => grouping.ToArray();
    }
    internal sealed class IMultiMapDebugView<K, V>
    {
        private readonly IMultiMap<K, V> multiMap;
        public IMultiMapDebugView(IMultiMap<K, V> multiMap)
        {
            this.multiMap = multiMap ?? throw ExceptionHelper.ArgumentNull(nameof(multiMap));
        }
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyPair<K, V>[] Items
        {
            get
            {
                KeyPair<K, V>[] array = new KeyPair<K, V>[multiMap.Count];
                multiMap.CopyTo(array, 0);
                return array;
            }
        }
    }
}
