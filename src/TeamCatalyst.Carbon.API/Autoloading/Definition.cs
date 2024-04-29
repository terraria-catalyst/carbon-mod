using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace TeamCatalyst.Carbon.API.Core.Autoloading {
    /// <summary>
    /// Simple autoloaded definition class, stores singletons of every derived type to be accessed easily from any instance.
    /// </summary>
    internal abstract class Definition : ModType {
        readonly static Dictionary<Type, Definition> _definitions = new Dictionary<Type, Definition>();
        static uint _nextID = 0;
        
        public uint ID { get; protected set; }

        protected sealed override void Register() {
            Type type = GetType();
            _definitions.Add(type, this);
            ID = ++_nextID;
        }

        public static uint GetIDFromType<T>() where T : Definition {
            _definitions.TryGetValue(typeof(T), out var ret);

            return ret != null ? ret.ID : 0;
        }

        public static Definition? GetDefinitionFromType<T>() where T : Definition {
            return _definitions.FirstOrDefault(d => d.Key == typeof(T)).Value;
        }

        public static Definition? GetDefinitionFromID(int i) {
            Definition? d = null;
            foreach (var item in _definitions.Values)
            {
                if (item.ID == i) {
                    d = item; break;
                }
            }
            return d;
        }
    }
}
