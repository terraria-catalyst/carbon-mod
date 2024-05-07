using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.UI;

namespace TeamCatalyst.Carbon.Config;

internal class ModuleSwitchButton : UIElement
{
    private readonly Asset<Texture2D> arrowTexture;
    private readonly Asset<Texture2D> basePanelTexture;
    private readonly Asset<Texture2D> hoveredTexture;

    private readonly Color color;

    private readonly bool enabled;

    public ModuleSwitchButton(bool enabled)
    {
        this.enabled = enabled;

        arrowTexture = Main.Assets.Request<Texture2D>("Images/UI/TexturePackButtons");
        basePanelTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/PanelGrayscale");
        hoveredTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder");

        color = enabled ? Color.PaleVioletRed : Color.LightGreen;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        CalculatedStyle dimensions = GetDimensions();

        Utils.DrawSplicedPanel(
            spriteBatch,
            basePanelTexture.Value,
            (int)dimensions.X,
            (int)dimensions.Y,
            (int)dimensions.Width,
            (int)dimensions.Height,
            10, 10, 10, 10,
            Color.Lerp(Color.Black, color, IsMouseHovering ? 1 : 0.9f) * 0.7f
        );

        if (IsMouseHovering)
        {
            Utils.DrawSplicedPanel(
                spriteBatch,
                hoveredTexture.Value,
                (int)dimensions.X,
                (int)dimensions.Y,
                (int)dimensions.Width,
                (int)dimensions.Height,
                10, 10, 10, 10,
                Main.OurFavoriteColor
            );
        }

        if (arrowTexture.Value == null) 
        {
            return;
        }

        int halfWidth = arrowTexture.Value.Width / 2;
        int halfHeight = arrowTexture.Value.Height / 2;

        Rectangle frame = new(enabled ? 0 : halfWidth, halfHeight, halfWidth, halfHeight);

        Vector2 offset = new(enabled ? 4 : -4, 0);

        spriteBatch.Draw(arrowTexture.Value, dimensions.Center() + offset, frame, Color.White, 0, new Vector2(halfWidth, halfHeight) / 2, 1, SpriteEffects.None, 0);
    }
}
