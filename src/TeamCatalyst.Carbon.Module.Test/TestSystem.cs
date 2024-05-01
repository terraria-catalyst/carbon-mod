using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace TeamCatalyst.Carbon.Module.Test
{
    internal sealed class TestSystem : ILoadable
    {
        public void Load(Mod mod)
        {
            mod.Logger.Info("Hello from TestSystem!");
        }

        public void Unload() { }
    }
}
