using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace TeamCatalyst.Carbon.Config;

// This class is a reimplementation of vanilla's UIResourcePackSelectionMenu for module widgets.
internal sealed class ModuleConfigState : UIState
{
    private readonly UIState previousState;

    private UIText modulesAvailable;
    private UIText modulesEnabled;

    private UIList availableModulesList;
    private UIList enabledModulesList;

    public ModuleConfigState(UIState previousState)
    {
        this.previousState = previousState;
    }

    public override void OnInitialize()
    {
        UIElement outerContainer = new();

        outerContainer.Width.Set(0f, 0.8f);
        outerContainer.MaxWidth.Set(800f, 0f);
        outerContainer.MinWidth.Set(600f, 0f);
        outerContainer.Top.Set(240f, 0f);
        outerContainer.Height.Set(-240f, 1f);
        outerContainer.HAlign = 0.5f;

        Append(outerContainer);

        UIPanel mainPanel = new()
        {
            Width = StyleDimension.Fill,
            Height = new StyleDimension(-110f, 1f),
            BackgroundColor = new Color(33, 43, 79) * 0.8f,
            PaddingRight = 0f,
            PaddingLeft = 0f
        };

        outerContainer.Append(mainPanel);

        AddBackAndReloadButtons(outerContainer);
        AddTitle(outerContainer);
        AddSeparator(mainPanel);
        AddCategoryTitles(mainPanel);
        AddUILists(mainPanel);

        PopulateModuleList();
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

        outerContainer.Append(backButton);

        UITextPanel<LocalizedText> reloadButton = new(Language.GetText("Mods.Carbon.UI.Modules.ConfigReload"), 0.7f, large: true)
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

        outerContainer.Append(reloadButton);
    }

    private void AddTitle(UIElement outerContainer)
    {
        UITextPanel<LocalizedText> title = new(Language.GetText("Mods.Carbon.UI.Modules.ConfigTitle"), 1f, large: true)
        {
            HAlign = 0.5f,
            VAlign = 0f,
            Top = new StyleDimension(-44f, 0f),
            BackgroundColor = new Color(73, 94, 171)
        };

        title.SetPadding(13f);

        outerContainer.Append(title);
    }

    private void AddSeparator(UIPanel mainPanel)
    {
        UIVerticalSeparator element = new()
        {
            Height = new StyleDimension(-12f, 1f),
            HAlign = 0.5f,
            VAlign = 1f,
            Color = new Color(89, 116, 213, 255) * 0.9f
        };

        mainPanel.Append(element);
    }

    private void AddCategoryTitles(UIPanel mainPanel)
    {
        mainPanel.Append(modulesAvailable = new UIText(Language.GetText("Mods.Carbon.UI.Modules.Available"))
        {
            HAlign = 0f,
            Left = new StyleDimension(25f, 0f),
            Width = new StyleDimension(-25f, 0.5f),
            VAlign = 0f,
            Top = new StyleDimension(10f, 0f)
        });

        mainPanel.Append(modulesEnabled = new UIText(Language.GetText("Mods.Carbon.UI.Modules.Enabled"))
        {
            HAlign = 1f,
            Left = new StyleDimension(-25f, 0f),
            Width = new StyleDimension(-25f, 0.5f),
            VAlign = 0f,
            Top = new StyleDimension(10f, 0f)
        });
    }

    private void AddUILists(UIPanel mainPanel)
    {
        int num3 = 30;

        // These elements are used for padding.
        UIElement mainPadding = new()
        {
            Width = StyleDimension.Fill,
            Height = StyleDimension.FromPixelsAndPercent(-(num3 + 4 + 5), 1f),
            VAlign = 1f
        };

        mainPadding.SetPadding(0f);

        mainPanel.Append(mainPadding);

        UIElement leftPadding = new()
        {
            Width = new StyleDimension(-20f, 0.5f),
            Height = new StyleDimension(0f, 1f),
            Left = new StyleDimension(10f, 0f)
        };

        leftPadding.SetPadding(0f);

        mainPadding.Append(leftPadding);

        UIElement rightPadding = new()
        {
            Width = new StyleDimension(-20f, 0.5f),
            Height = new StyleDimension(0f, 1f),
            Left = new StyleDimension(-10f, 0f),
            HAlign = 1f
        };

        rightPadding.SetPadding(0f);

        mainPadding.Append(rightPadding);

        // The actual lists.
        UIList available = new()
        {
            Width = new StyleDimension(-25f, 1f),
            Height = new StyleDimension(0f, 1f),
            ListPadding = 5f,
            HAlign = 1f
        };

        leftPadding.Append(available);

        availableModulesList = available;

        UIList enabled = new()
        {
            Width = new StyleDimension(-25f, 1f),
            Height = new StyleDimension(0f, 1f),
            ListPadding = 5f,
            HAlign = 0f,
            Left = new StyleDimension(0f, 0f)
        };

        rightPadding.Append(enabled);

        enabledModulesList = enabled;

        AddScrollBars(leftPadding, rightPadding);
    }

    private void AddScrollBars(UIElement leftPadding, UIElement rightPadding)
    {
        UIScrollbar leftScrollbar = new()
        {
            Height = new StyleDimension(0f, 1f),
            HAlign = 0f,
            Left = new StyleDimension(0f, 0f)
        };

        leftPadding.Append(leftScrollbar);

        availableModulesList.SetScrollbar(leftScrollbar);

        UIScrollbar rightScrollbar = new()
        {
            Height = new StyleDimension(0f, 1f),
            HAlign = 1f
        };

        rightPadding.Append(rightScrollbar);

        enabledModulesList.SetScrollbar(rightScrollbar);
    }

    private void UpdateTitles()
    {
        modulesAvailable.SetText(Language.GetTextValue("Mods.Carbon.UI.Modules.Available", availableModulesList.Count));
        modulesEnabled.SetText(Language.GetTextValue("Mods.Carbon.UI.Modules.Enabled", enabledModulesList.Count));
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

        CarbonMod.WriteModuleToggles();

        CarbonMod carbon = ModContent.GetInstance<CarbonMod>();

        carbon.Logger.Info("Player reconfigured module selection!");
        carbon.LogLoadedModules();

        ModLoader.Reload();
    }

    private void PopulateModuleList()
    {
        availableModulesList.Clear();
        enabledModulesList.Clear();

        Dictionary<string, bool> moduleList = CarbonMod.EnabledModules!;

        foreach (KeyValuePair<string, bool> moduleInfo in moduleList)
        {
            UIModuleItem moduleItem = new(moduleInfo.Key)
            {
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f)
            };

            moduleItem.Append(AddToggleButton(moduleInfo.Key, moduleInfo.Value));

            if (moduleInfo.Value)
            {
                EnabledModuleElements(moduleItem);

                enabledModulesList.Add(moduleItem);
            }
            else
            {
                AvailableModuleElements(moduleItem);

                availableModulesList.Add(moduleItem);
            }
        }

        UpdateTitles();
    }

    private void EnabledModuleElements(UIModuleItem module)
    {
    }

    private void AvailableModuleElements(UIModuleItem module)
    {
    }

    private UIElement AddToggleButton(string key, bool enabled)
    {
        Asset<Texture2D> asset = Main.Assets.Request<Texture2D>("Images/UI/TexturePackButtons");

        UIImage button = new(asset)
        {
            Left = StyleDimension.FromPixelsAndPercent(-80, 1),
            Width = StyleDimension.FromPixels(80),
            Height = StyleDimension.Fill
        };

        button.OnMouseOver += delegate 
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
        };

        string moduleKey = key;
        bool isEnabled = enabled;

        button.OnLeftClick += delegate 
        {
            SoundEngine.PlaySound(SoundID.MenuTick);

            CarbonMod.EnabledModules![moduleKey] = !isEnabled;

            PopulateModuleList();
        };

        return button;
    }
}
