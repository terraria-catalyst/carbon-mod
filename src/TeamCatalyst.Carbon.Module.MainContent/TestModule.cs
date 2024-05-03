using TeamCatalyst.Carbon.API;
using Terraria.ModLoader;

namespace TeamCatalyst.Carbon.Module.MainContent {
    public sealed class TestModule : ModSystem {
        public override void Load() {
            Mod.Logger.Info("Hello from TestModule!");
        }
    }
}
