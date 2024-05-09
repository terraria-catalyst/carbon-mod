using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

using JetBrains.Annotations;

using MonoMod.Cil;
using MonoMod.RuntimeDetour;

using Newtonsoft.Json;

using ReLogic.Content.Sources;

using TeamCatalyst.Carbon.API;
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
	public sealed class CarbonMod : Mod
	{
		public static string CarbonFolder => Path.Join(Main.SavePath, "CarbonMod");

		public static string ModuleConfigFile => Path.Join(Main.SavePath, "CarbonMod", "enabled-modules.json");

		public static Dictionary<string, bool>? EnabledModules { get; set; }

		public static List<Assembly>? LoadedAssemblies { get; set; }

		private static ILHook? getLoadableTypesHookAutoload;
		private static ILHook? getLoadableTypesHookAutoloadConfig;
		private static Hook? getCarbonAssetsSplitName;

		public override void Load()
		{
			LogLoadedModules();
		}

		public override void Unload()
		{
			base.Unload();

			getLoadableTypesHookAutoload?.Dispose();
			getLoadableTypesHookAutoload = null;
			getLoadableTypesHookAutoloadConfig?.Dispose();
			getLoadableTypesHookAutoloadConfig = null;
			getCarbonAssetsSplitName?.Dispose();
			getCarbonAssetsSplitName = null;
		}

#pragma warning disable CA2255
		[ModuleInitializer]
		internal static void Initialize()
		{
			if (!System.IO.File.Exists(CarbonFolder))
			{
				Directory.CreateDirectory(CarbonFolder);
			}

			InitializeModuleToggles();

			getLoadableTypesHookAutoload = new ILHook(typeof(Mod).GetMethod("Autoload", BindingFlags.NonPublic | BindingFlags.Instance)!, LoadableTypesHook);
			getLoadableTypesHookAutoloadConfig = new ILHook(typeof(Mod).GetMethod("AutoloadConfig", BindingFlags.NonPublic | BindingFlags.Instance)!, LoadableTypesHook);
			getCarbonAssetsSplitName = new Hook(typeof(ModContent).GetMethod("SplitName", BindingFlags.Public | BindingFlags.Static)!, GetCarbonAssets);
		}

#pragma warning restore CA2255

		private static void GetCarbonAssets(orig_SplitName orig, string name, out string domain, out string subName)
		{
			if (name.StartsWith("TeamCatalyst/Carbon/Module"))
			{
				var splitPath = name.Split("/");
				if (splitPath.Length >= 5)
				{
					name = $"Carbon/{string.Join("/", splitPath[4..])}";
				}
			}
			orig(name, out domain, out subName);
		}

		private delegate void orig_SplitName(string name, out string domain, out string subName);

		private static void LoadableTypesHook(ILContext il)
		{
			var c = new ILCursor(il);

			c.GotoNext(MoveType.Before, x => x.MatchCall(typeof(AssemblyManager), "GetLoadableTypes"));
			c.Remove();
			c.EmitDelegate(
				(Assembly code) => code != typeof(CarbonMod).Assembly ? AssemblyManager.GetLoadableTypes(code) : GetLoadableTypesWithModules()
			);
		}

		private static Type[] GetLoadableTypesWithModules()
		{
			if (AssemblyLoadContext.GetLoadContext(typeof(CarbonMod).Assembly) is not AssemblyManager.ModLoadContext mlc)
			{
				throw new InvalidOperationException("CarbonMod is not loaded in a ModLoadContext.");
			}

			List<Type> types = [];

			foreach (KeyValuePair<Assembly, Type[]> pair in mlc.loadableTypes)
			{
				if (LoadedAssemblies.Contains(pair.Key))
				{
					types.AddRange(pair.Value);
				}
			}

			return types.ToArray();
		}

		internal static void InitializeModuleToggles()
		{
			if (AssemblyLoadContext.GetLoadContext(typeof(CarbonMod).Assembly) is not AssemblyManager.ModLoadContext mlc)
			{
				throw new InvalidOperationException("CarbonMod is not loaded in a ModLoadContext.");
			}

			EnabledModules = [];
			LoadedAssemblies = [];

			if (!System.IO.File.Exists(ModuleConfigFile))
			{
				System.IO.File.WriteAllText(ModuleConfigFile, "{}");
			}

			string json = System.IO.File.ReadAllText(ModuleConfigFile);
			Dictionary<string, bool> toggledModules = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json)!;

			foreach (KeyValuePair<string, Assembly> pair in mlc.assemblies)
			{
				if (pair.Value.GetCustomAttribute<ModuleAttribute>() is null)
				{
					LoadedAssemblies.Add(pair.Value);
				}
				else
				{
					if (toggledModules!.ContainsKey(pair.Key))
					{
						bool isModuleOn = toggledModules[pair.Key];
						EnabledModules.Add(pair.Value.GetName().Name!, isModuleOn);
						if (isModuleOn)
						{
							LoadedAssemblies.Add(pair.Value);
						}
					}
					else
					{
						EnabledModules.Add(pair.Value.GetName().Name!, true);
						LoadedAssemblies.Add(pair.Value);
					}
				}
			}

			WriteModuleToggles();
		}

		internal static void ReadModuleToggles()
		{
			string json = System.IO.File.ReadAllText(ModuleConfigFile);

			EnabledModules = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json)!;
		}

		internal static void WriteModuleToggles()
		{
			string json = JsonConvert.SerializeObject(EnabledModules);

			System.IO.File.WriteAllText(ModuleConfigFile, json);
		}

		public override IContentSource CreateDefaultContentSource()
		{
			SmartContentSource source = new(base.CreateDefaultContentSource());

			source.AddDirectoryRedirect("Content", "Assets/Textures");
			source.AddDirectoryRedirect("Common", "Assets/Textures");

			return source;
		}

		public void LogLoadedModules()
		{
			Logger.Info("Loaded Modules:");

			foreach (Assembly assembly in LoadedAssemblies)
			{
				Logger.Info($"Module {assembly.GetName().Name}");
			}
		}
	}
}