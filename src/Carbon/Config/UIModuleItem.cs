using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
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

    private UIText description;

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

        icon = ModContent.Request<Texture2D>($"Carbon/Assets/Textures/Modules/{module}");

        AddChildren();
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        Rectangle frame = new(
            (int)description.GetOuterDimensions().X,
            (int)description.GetOuterDimensions().Y - 2,
            (int)description.GetOuterDimensions().Width,
            (int)GetInnerDimensions().Height - (int)description.Top.Pixels
        );

        base.DrawSelf(spriteBatch);

        spriteBatch.Draw(
            TextureAssets.MagicPixel.Value,
            frame,
            Color.Lerp(BackgroundColor, Color.Black, 0.1f)
        );

        DrawIcon(spriteBatch);
    }

    private void DrawIcon(SpriteBatch spriteBatch)
    {
        CalculatedStyle innerDimensions = GetInnerDimensions();

        int width = (int)innerDimensions.Height - 4;
        int height = (int)innerDimensions.Height - 4;

        spriteBatch.Draw(icon.Value, new Rectangle((int)innerDimensions.X + 2, (int)innerDimensions.Y + 2, width, height), Color.White);
    }

    private void AddChildren()
    {
        StyleDimension left = StyleDimension.FromPixels(94);
        StyleDimension top = StyleDimension.FromPixels(2);

        UIText title = new(Language.GetTextValue($"Mods.Carbon.{Module}"))
        {
            Left = left,
            Top = top
        };

        Append(title);

        top.Pixels += title.GetOuterDimensions().Height + 6f;

        description = new(Language.GetTextValue($"Mods.Carbon.{Module}Description"), 0.6f)
        {
            Left = left,
            Top = top,
            IsWrapped = true,
            Width = StyleDimension.FromPixelsAndPercent(-129, 1)
        };

        Append(description);
    }
}
