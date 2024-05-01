using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using JetBrains.Annotations;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using ReLogic.Content.Sources;
using TeamCatalyst.Carbon.Core;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Carbon
{
    [UsedImplicitly]
    internal static class Unused;
}

namespace TeamCatalyst.Carbon
{
    /// <summary>
    ///     Carbon <see cref="Mod"/> entrypoint.
    /// </summary>
    public sealed class CarbonMod : Mod {
        public static readonly string CarbonFolder = Path.Join(Main.SavePath, "CarbonMod");
        public static readonly string ModuleConfigFile = Path.Join(Main.SavePath, "CarbonMod", "enabled-modules.json");

        public static Mod? ModReference { get; private set; }

        private static ILHook? getLoadableTypesHookAutoload;
        private static ILHook? getLoadableTypesHookAutoloadConfig;

        public override void Load()
            => ModReference = this;

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
            if (!System.IO.File.Exists(CarbonFolder))
            {
                Directory.CreateDirectory(CarbonFolder);
            }

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



            List<Type> types = new List<Type>();

            foreach (KeyValuePair<Assembly, Type[]> pair in mlc.loadableTypes)
            {
                Console.WriteLine(pair.Key.FullName);

                types.AddRange(pair.Value);
            }
            return types.ToArray();
        }

        public override IContentSource CreateDefaultContentSource()
        {
            SmartContentSource source = new SmartContentSource(base.CreateDefaultContentSource());
            source.AddDirectoryRedirect("Content", "Assets/Textures");
            source.AddDirectoryRedirect("Common", "Assets/Textures");
            return source;
        }
    }
}
