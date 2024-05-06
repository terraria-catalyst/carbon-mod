using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace TeamCatalyst.Carbon.Config;

internal class UIModuleItem : UIPanel
{
    private static readonly Color DefaultBackgroundColor = new Color(26, 40, 89) * 0.8f;
    private static readonly Color DefaultBorderColor = new Color(13, 20, 44) * 0.8f;

    public string Module { get; private set; }

    private readonly Asset<Texture2D> icon;

    public UIModuleItem(string module)
    {
        Module = module;

        BackgroundColor = DefaultBackgroundColor;
        BorderColor = DefaultBorderColor;

        Height = StyleDimension.FromPixels(102f);
        MinHeight = Height;
        MaxHeight = Height;
        MinWidth = StyleDimension.FromPixels(102f);
        Width = StyleDimension.FromPercent(1f);

        SetPadding(5f);

        OverflowHidden = true;

        // TODO: Icon metadata from module key
        icon = ModContent.Request<Texture2D>($"Carbon/Assets/Modules/{module}");

        AddChildren();
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        DrawIcon(spriteBatch);
    }

    private void DrawIcon(SpriteBatch spriteBatch)
    {
        CalculatedStyle innerDimensions = GetInnerDimensions();

        int width = (int)innerDimensions.Height - 12;
        int height = (int)innerDimensions.Height - 12;

        spriteBatch.Draw(icon.Value, new Rectangle((int)innerDimensions.X + 6, (int)innerDimensions.Y + 6, width, height), Color.White);
    }

    private void AddChildren()
    {
        StyleDimension left = StyleDimension.FromPixels(93);
        StyleDimension top = StyleDimension.FromPixels(2);

        UIText title = new(Language.GetTextValue($"Mods.Carbon.{Module}"))
        {
            Left = left,
            Top = top
        };

        Append(title);

        top.Pixels += title.GetOuterDimensions().Height + 6f;

        UIText description = new(Language.GetTextValue($"Mods.Carbon.{Module}Description"), 0.6f)
        {
            Left = left,
            Top = top,
            IsWrapped = true,
            Width = StyleDimension.FromPixelsAndPercent(-184, 1)
        };

        Append(description);
    }
}
