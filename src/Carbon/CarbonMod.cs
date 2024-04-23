using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using JetBrains.Annotations;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Carbon {
    [UsedImplicitly]
    internal static class Unused;
}

namespace TeamCatalyst.Carbon {
    /// <summary>
    ///     Carbon <see cref="Mod"/> entrypoint.
    /// </summary>
    public sealed class CarbonMod : Mod {
        private static ILHook? getLoadableTypesHookAutoload;
        private static ILHook? getLoadableTypesHookAutoloadConfig;

        public override void Unload() {
            base.Unload();

            getLoadableTypesHookAutoload?.Dispose();
            getLoadableTypesHookAutoload = null;
            getLoadableTypesHookAutoloadConfig?.Dispose();
            getLoadableTypesHookAutoloadConfig = null;
        }

#pragma warning disable CA2255
        [ModuleInitializer]
        internal static void Initialize() {
            getLoadableTypesHookAutoload = new ILHook(typeof(Mod).GetMethod("Autoload", BindingFlags.NonPublic | BindingFlags.Instance)!, LoadableTypesHook);
            getLoadableTypesHookAutoloadConfig = new ILHook(typeof(Mod).GetMethod("AutoloadConfig", BindingFlags.NonPublic | BindingFlags.Instance)!, LoadableTypesHook);
        }
#pragma warning restore CA2255

        private static void LoadableTypesHook(ILContext il) {
            var c = new ILCursor(il);

            c.GotoNext(MoveType.Before, x => x.MatchCall(typeof(AssemblyManager), "GetLoadableTypes"));
            c.Remove();
            c.EmitDelegate(
                (Assembly code) => code != typeof(CarbonMod).Assembly ? AssemblyManager.GetLoadableTypes(code) : GetLoadableTypesWithModules()
            );
        }

        private static Type[] GetLoadableTypesWithModules() {
            // TODO: Add config support.
            if (AssemblyLoadContext.GetLoadContext(typeof(CarbonMod).Assembly) is not AssemblyManager.ModLoadContext mlc)
                throw new InvalidOperationException("CarbonMod is not loaded in a ModLoadContext.");

            return mlc.loadableTypes.SelectMany(x => x.Value).ToArray();
        }
    }
}
