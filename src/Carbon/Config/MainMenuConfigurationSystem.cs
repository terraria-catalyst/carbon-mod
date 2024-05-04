using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace TeamCatalyst.Carbon.Config;

internal class MainMenuConfigurationSystem : ModSystem
{
    private Hook? modItemDrawHook;

    private ILHook? configButtonHook;

    private UIImage? configButton;

    private delegate void ModItemDrawDelegate(UIModItem self, SpriteBatch spriteBatch);

    public override void Load()
    {
        MethodInfo drawMethod = typeof(ModLoader).Assembly
            .GetType("Terraria.ModLoader.UI.UIModItem")!
            .GetMethod("Draw", BindingFlags.Public | BindingFlags.Public | BindingFlags.Instance)!;

        modItemDrawHook = new Hook(drawMethod, Draw);

        MethodInfo initialiseMethod = typeof(ModLoader).Assembly
            .GetType("Terraria.ModLoader.UI.UIModItem")!
            .GetMethod("OnInitialize", BindingFlags.Public | BindingFlags.Instance)!;

        configButtonHook = new ILHook(initialiseMethod, AddNewButton);
    }

    public override void Unload()
    {
        modItemDrawHook?.Dispose();
        configButtonHook?.Dispose();
    }

    private void Draw(ModItemDrawDelegate orig, UIModItem self, SpriteBatch spriteBatch)
    {
        orig(self, spriteBatch);
    }

    private void AddNewButton(ILContext il)
    {
        ILCursor c = new(il);

        if (!c.TryGotoNext(x => x.MatchStfld(typeof(ModLoader).Assembly
            .GetType("Terraria.ModLoader.UI.UIModItem")!
            .GetField("_moreInfoButton", BindingFlags.Instance | BindingFlags.NonPublic)!))
            )
        {
            throw new Exception($"IL edit failed: {GetType().FullName}");
        }

        for (int i = 0; i < 2; i++)
        {
            if (!c.TryGotoNext(x => x.MatchLdfld(
                typeof(ModLoader).Assembly
                .GetType("Terraria.ModLoader.UI.UIModItem")!
                .GetField("_moreInfoButton", BindingFlags.Instance | BindingFlags.NonPublic)!))
                )
            {
                throw new Exception($"IL edit failed: {GetType().FullName}");
            }
        }

        c.Index += 2;

        c.Emit(OpCodes.Ldarg_0);
        c.Emit(OpCodes.Ldloc_0);
        c.EmitDelegate<Action<UIPanel, string>>((@this, name) =>
        {
            // TODO: better way of verifying mod identity?
            if (!name.Equals($"{Mod.DisplayName} v{Mod.Version}"))
                return;

            configButton = new UIImage(ModContent.Request<Texture2D>("Carbon/Assets/UI/ConfigButton"))
            {
                Width = { Pixels = 36f },
                Height = { Pixels = 36f },
                Left = { Pixels = -108f - 10f, Precent = 1f },
                Top = { Pixels = 40f }
            };
            configButton.OnLeftClick += ConfigButtonClick;

            @this.Append(configButton);
        });
    }

    private void ConfigButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        // Change menu states
    }
}
