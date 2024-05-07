using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using Terraria.GameContent;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria.Audio;
using Terraria.ID;

namespace TeamCatalyst.Carbon.Config;

internal sealed class MainMenuConfigurationSystem : ModSystem
{
    private const int FancyUIMenuMode = 888;

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

        if (configButton == null)
        {
            return;
        }

        if (configButton.IsMouseHovering && self._mod.DisplayName == Mod.DisplayName)
        {
            Rectangle bounds = self.GetOuterDimensions().ToRectangle();
            
            bounds.Height += 16;

            string text = Language.GetTextValue("Mods.Carbon.UI.Modules.Config");

            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(text);

            Vector2 position = Main.MouseScreen + new Vector2(16);

            position.X = Math.Min(position.X, bounds.Right - textSize.X - 16);
            position.Y = bounds.Bottom - textSize.Y - 16;

            Utils.DrawBorderStringFourWay(
                Main.spriteBatch,
                FontAssets.MouseText.Value,
                text,
                position.X,
                position.Y,
                Main.OurFavoriteColor,
                Color.Black,
                Vector2.Zero
            );
        }
    }

    private void AddNewButton(ILContext il)
    {
        ILCursor c = new(il);

        // Needs to add the button just to the left of the mod info button.
        if (!c.TryGotoNext(x => x.MatchCall(typeof(ModLoader)
            .GetMethod("TryGetMod", BindingFlags.Static | BindingFlags.Public)!)))
        {
            throw new Exception($"IL edit failed: {GetType().FullName}");
        }

        if (!c.TryGotoPrev(x => x.MatchLdarg0()))
        {
            throw new Exception($"IL edit failed: {GetType().FullName}");
        }

        ILLabel label = c.DefineLabel();

        // Skip the offset decrement and addition of the new element if this isn't the Carbon widget.
        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Func<UIModItem, bool>>(uiModItem => uiModItem._mod.DisplayName == Mod.DisplayName);
        c.Emit(OpCodes.Brfalse, label);

        // Decrement the bottomRightRowOffset by 36 for this button.
        c.Emit(OpCodes.Ldloc, 6);
        c.Emit(OpCodes.Ldc_I4, 36);
        c.Emit(OpCodes.Sub);
        c.Emit(OpCodes.Stloc, 6);

        c.Emit(OpCodes.Ldarg_0);
        c.Emit(OpCodes.Ldloc, 6);

        c.EmitDelegate<Action<UIModItem, int>>((uiModItem, bottomRightRowOffset) =>
        {
            configButton = new UIImage(ModContent.Request<Texture2D>("Carbon/Assets/Textures/UI/ConfigButton"))
            {
                Width = { Pixels = 36f },
                Height = { Pixels = 36f },
                Left = { Pixels = bottomRightRowOffset - 5, Precent = 1f },
                Top = { Pixels = 40f }
            };
            configButton.OnLeftClick += ConfigButtonClick;

            uiModItem.Append(configButton);
        });

        c.MarkLabel(label);
    }

    private void ConfigButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        SoundEngine.PlaySound(SoundID.MenuOpen);

        Main.menuMode = FancyUIMenuMode;
        Main.MenuUI.SetState(new ModuleConfigState(Main.MenuUI.CurrentState));
    }
}
