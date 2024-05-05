using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace TeamCatalyst.Carbon.Config;

// This class is a reimplementation of vanilla's UIResourcePackSelectionMenu for module widgets.
internal sealed class ModuleConfigState : UIState
{
    private readonly UIState previousState;

    public ModuleConfigState(UIState previousState)
    {
        this.previousState = previousState;
    }

    public override void OnInitialize()
    {
        UIElement mainElement = new();

        mainElement.Width.Set(0f, 0.8f);
        mainElement.MaxWidth.Set(800f, 0f);
        mainElement.MinWidth.Set(600f, 0f);
        mainElement.Top.Set(240f, 0f);
        mainElement.Height.Set(-240f, 1f);
        mainElement.HAlign = 0.5f;

        Append(mainElement);

        UIPanel mainPanel = new()
        {
            Width = StyleDimension.Fill,
            Height = new StyleDimension(-110f, 1f),
            BackgroundColor = new Color(33, 43, 79) * 0.8f,
            PaddingRight = 0f,
            PaddingLeft = 0f
        };

        mainElement.Append(mainPanel);

        AddBackAndReloadButtons(mainElement);
    }

    private void AddBackAndReloadButtons(UIElement outerContainer)
    {
        UITextPanel<LocalizedText> backButton = new(Language.GetText("UI.Back"), 0.7f, large: true)
        {
            Width = new StyleDimension(-8f, 0.5f),
            Height = new StyleDimension(50f, 0f),
            VAlign = 1f,
            HAlign = 0f,
            Top = new StyleDimension(-45f, 0f)
        };

        backButton.OnMouseOver += FadedMouseOver;
        backButton.OnMouseOut += FadedMouseOut;
        backButton.OnLeftClick += GoBack;

        backButton.SetSnapPoint("GoBack", 0);

        outerContainer.Append(backButton);

        UITextPanel<LocalizedText> reloadButton = new(Language.GetText("Mods.Carbon.UI.ModuleConfigReload"), 0.7f, large: true)
        {
            Width = new StyleDimension(-8f, 0.5f),
            Height = new StyleDimension(50f, 0f),
            VAlign = 1f,
            HAlign = 1f,
            Top = new StyleDimension(-45f, 0f)
        };

        reloadButton.OnMouseOver += FadedMouseOver;
        reloadButton.OnMouseOut += FadedMouseOut;
        reloadButton.OnLeftClick += CommitModuleChanges;

        reloadButton.SetSnapPoint("CommitChanges", 0);

        outerContainer.Append(reloadButton);
    }

    private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
    {
        SoundEngine.PlaySound(SoundID.MenuTick);

        ((UIPanel)evt.Target).BackgroundColor = new Color(73, 94, 171);
        ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
    }

    private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
    {
        ((UIPanel)evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.8f;
        ((UIPanel)evt.Target).BorderColor = Color.Black;
    }

    private void GoBack(UIMouseEvent evt, UIElement listeningElement)
    {
        SoundEngine.PlaySound(SoundID.MenuClose);

        if (previousState == null)
        {
            Main.menuMode = 0;

            return;
        }

        Main.menuMode = 888;
        Main.MenuUI.SetState(previousState);
    }

    private void CommitModuleChanges(UIMouseEvent evt, UIElement listeningElement)
    {
        SoundEngine.PlaySound(SoundID.MenuOpen);

        // TODO: write the module toggles to file.

        CarbonMod carbon = ModContent.GetInstance<CarbonMod>();

        carbon.Logger.Info("Reloaded module selection!");
        carbon.LogLoadedModules();

        ModLoader.Reload();
    }
}
