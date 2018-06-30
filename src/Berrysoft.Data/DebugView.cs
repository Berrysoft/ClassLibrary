using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Berrysoft.Data
{
    internal sealed class MapDebugView<K, V>
    {
        private readonly Map<K, V> map;
        public MapDebugView(Map<K, V> map)
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
    internal sealed class LookupDebugView<K, V>
    {
        private readonly Lookup<K, V> lookup;
        public LookupDebugView(Lookup<K, V> lookup)
        {
            this.lookup = lookup ?? throw ExceptionHelper.ArgumentNull(nameof(lookup));
        }
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public Lookup<K, V>.Grouping[] Items => ((IEnumerable<Lookup<K, V>.Grouping>)lookup).ToArray();
    }
    internal sealed class MultiMapDebugView<K, V>
    {
        private readonly MultiMap<K, V> multiMap;
        public MultiMapDebugView(MultiMap<K, V> multiMap)
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
