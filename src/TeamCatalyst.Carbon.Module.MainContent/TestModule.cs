using TeamCatalyst.Carbon.API;
using Terraria.ModLoader;

namespace TeamCatalyst.Carbon.Module.MainContent {
    public sealed class TestModule : ICarbonModule {
        void ILoadable.Load(Mod mod) {
            mod.Logger.Info("Hello from TestModule!");
        }

        void ILoadable.Unload() { }
    }
}
