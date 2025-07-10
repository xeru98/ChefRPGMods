using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using ModConfigMenu.Components;
using ModConfigMenu.Framework.ModOption;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XeruUtils;
using XeruUtils.Components;
using Object = UnityEngine.Object;

namespace ModConfigMenu.Framework;

internal class ModConfigUI
{
    private static readonly Vector2 SAVE_RESET_BUTTON_ANCHOR_POSITION = new Vector2(26, 7);
    private GameObject startMenuWindow;
    
    private ScrollView scrollView;
    private TextMeshProUGUI titleText;
    private GameObject backButton;
    private PluginTuple currentPlugin = null;
    private GameObject saveButton;
    private GameObject resetButton;
    
    private readonly List<BaseModOption> modOptions = new List<BaseModOption>();

    public ModConfigUI()
    {
        RegisterSprites();
    }

    public GameObject GetStartMenuWindow()
    {
        return startMenuWindow;
    }
    
    public void OpenFromFrontMenu()
    {
        StartMenuManager startMenuManager = Object.FindObjectOfType<StartMenuManager>();
        FieldHelpers.GetPrivateFieldValue<GameObject, StartMenuManager>(startMenuManager, "FrontMenu").SetActive(false);
        FieldHelpers.GetPrivateFieldValue<GameObject, StartMenuManager>(startMenuManager, "LoadSlots").SetActive(false);
        FieldHelpers.GetPrivateFieldValue<GameObject, StartMenuManager>(startMenuManager, "Settings").SetActive(false);
        FieldHelpers.GetPrivateFieldValue<GameObject, StartMenuManager>(startMenuManager, "Credits").SetActive(false);
        OpenStartMenu();
        startMenuWindow.SetActive(true);
    }
    
    private void RegisterSprites()
    {
        Plugin.SpriteCache.LoadSpriteFromAssetTexture(SpriteConstants.MAIN_MENU_UI_PANEL_TEXTURE_FILENAME, SpriteConstants.MAIN_MENU_UI_PANEL_SPRITE_SLICE, Plugin.Logger);
        Plugin.SpriteCache.LoadButtonSpritesFromAssetTexture(SpriteConstants.CLOSE_BUTTON_TEXTURE_FILENAME, SpriteConstants.CLOSE_BUTTON_SPRITE_SLICES, Plugin.Logger, SpriteConstants.CLOSE_BUTTON_SPRITE_CACHE_KEY);
        Plugin.SpriteCache.LoadButtonSpritesFromAssetTexture(SpriteConstants.BACK_BUTTON_TEXTURE_FILENAME, SpriteConstants.BACK_BUTTON_SPRITE_SLICES, Plugin.Logger, SpriteConstants.BACK_BUTTON_SPRITE_CACHE_KEY);
        Plugin.SpriteCache.LoadButtonSpritesFromAssetTexture(SpriteConstants.SAVE_BUTTON_TEXTURE_FILENAME, SpriteConstants.SAVE_RESET_BUTTON_SPRITE_SLICES, Plugin.Logger);
        Plugin.SpriteCache.LoadButtonSpritesFromAssetTexture(SpriteConstants.RESET_BUTTON_TEXTURE_FILENAME, SpriteConstants.SAVE_RESET_BUTTON_SPRITE_SLICES, Plugin.Logger);
        Plugin.SpriteCache.LoadButtonSpritesFromAssetTexture(SpriteConstants.SCROLLBAR_HANDLE_TEXTURE_FILENAME, SpriteConstants.SCROLLBAR_HANDLE_SPRITE_SLICES, Plugin.Logger);
    }
    
    private GameObject ConstructStartMenuWindow(Canvas parent)
    {
        // Construct Window
        Plugin.Logger.LogDebug("ConstructStartMenuWindow");
        GameObject window = new GameObject("ModConfigMenu_StartMenuWindow");
        window.transform.SetParent(parent.gameObject.transform, false);
        window.AddComponent<RectTransform>();
        window.AddComponent<CanvasRenderer>();
        Image panel = window.AddComponent<Image>();
        panel.sprite = Plugin.SpriteCache.Get(SpriteConstants.MAIN_MENU_UI_PANEL_TEXTURE_FILENAME).DefaultSprite;
        panel.rectTransform.sizeDelta = panel.sprite.rect.size;
        
        scrollView = new ScrollView(window, Plugin.SpriteCache.Get(SpriteConstants.SCROLLBAR_HANDLE_TEXTURE_FILENAME).ButtonSprites[ButtonState.Default], Plugin.SpriteCache.Get(SpriteConstants.SCROLLBAR_HANDLE_TEXTURE_FILENAME).ButtonSprites);
        UIHelpers.SetupRectTransform(scrollView.Root.GetComponent<RectTransform>(), AnchorPosition.BottomLeft, sizeDelta: new Vector2(324, 172), anchoredPosition: new Vector2(21, 31));
        
        // Add extra panel components
        titleText = ConstructTitleText(window, Constants.DEFAULT_PANEL_TITLE);
        ConstructCloseButton(window);
        backButton = ConstructBackButton(window);
        resetButton = ConstructResetButton(window);
        saveButton = ConstructSaveButton(window);
        return window;
    }
    
    private void Close()
    {
        currentPlugin = null; // return to the mod list on next open
        StartMenuManager startMenuManager = Object.FindObjectOfType<StartMenuManager>();
        startMenuManager.BackToFrontMenu();
    }

    private void OpenStartMenu()
    {
        // The first time we open this menu we need to construct it and add it to the start menu canvas for scaling
        Canvas parent = Object.FindObjectsOfType<Canvas>().First(canvas => canvas.gameObject.name.Contains("Start Menu"));
        if (!startMenuWindow)
        {
            startMenuWindow = ConstructStartMenuWindow(parent);
        }

        // clear any existing children
        scrollView.ClearContent();
        modOptions.Clear();
        
        // if we have a null config then we can just load a list of all mods with configs.
        if (currentPlugin == null)
        {
            titleText.text = Constants.DEFAULT_PANEL_TITLE;
            backButton.SetActive(false);
            saveButton.GetComponent<Button>().interactable = false;
            resetButton.GetComponent<Button>().interactable = false;
            GameObject modConfigList = ConstructModConfigList(Plugin.LoadedPluginsWithConfigs);
            scrollView.SetContent(modConfigList);
        }
        else
        {
            titleText.text = currentPlugin.Metadata.Name;
            backButton.SetActive(true);
            saveButton.GetComponent<Button>().interactable = true;
            resetButton.GetComponent<Button>().interactable = true;
            GameObject modConfigWidget = ConstructSpecificModConfigMenu(currentPlugin);
            scrollView.SetContent(modConfigWidget);
        }
    }

    private void OpenModConfigMenu(PluginTuple plugin)
    {
        currentPlugin = plugin;
        OpenStartMenu();
    }
    
    private void ReturnToModList()
    {
        currentPlugin = null;
        OpenStartMenu();
    }

    /// <summary>
    /// This just encapsulated the process of creating the close button for our panel. (Only called when used from front menu)
    /// </summary>
    /// <param name="parentWindow">The parent game object we are attaching this to</param>
    private void ConstructCloseButton(GameObject parentWindow)
    {
        GameObject closeButtonObj = UIHelpers.SetupSpriteSwapButton(parentWindow, "closeButton", Plugin.SpriteCache.Get(SpriteConstants.CLOSE_BUTTON_SPRITE_CACHE_KEY).ButtonSprites);
        Button closeButton = closeButtonObj.GetComponent<Button>();
        closeButton.onClick.AddListener(Close);

        RectTransform rectTransform = closeButtonObj.GetComponent<RectTransform>();
        Image closeButtonImage = rectTransform.gameObject.GetComponent<Image>();
        rectTransform.SetParent(parentWindow.transform, false);
        UIHelpers.SetupRectTransform(rectTransform, AnchorPosition.TopRight, sizeDelta: closeButtonImage.sprite.rect.size, anchoredPosition: new Vector2(-4, -5));
    }
    
    /// <summary>
    /// This just encapsulated the process of creating the back button for our panel. (Only active when modconfig != null)
    /// </summary>
    /// <param name="parentWindow">The parent game object we are attaching this to</param>
    private GameObject ConstructBackButton(GameObject parentWindow)
    {
        GameObject closeButtonObj = UIHelpers.SetupSpriteSwapButton(parentWindow, "backButton", Plugin.SpriteCache.Get(SpriteConstants.BACK_BUTTON_SPRITE_CACHE_KEY).ButtonSprites);
        Button closeButton = closeButtonObj.GetComponent<Button>();
        closeButton.onClick.AddListener(ReturnToModList);

        RectTransform rectTransform = closeButtonObj.GetComponent<RectTransform>();
        Image closeButtonImage = rectTransform.gameObject.GetComponent<Image>();
        rectTransform.SetParent(parentWindow.transform, false);
        UIHelpers.SetupRectTransform(rectTransform, AnchorPosition.TopLeft, sizeDelta: closeButtonImage.sprite.rect.size, anchoredPosition: new Vector2(4, -5));
        return closeButtonObj;
    }

    private TextMeshProUGUI ConstructTitleText(GameObject parentWindow, string text)
    {
        GameObject titleTextObj = new GameObject($"{parentWindow.name}_TitleText");
        TextMeshProUGUI textComponent = titleTextObj.AddComponent<TextMeshProUGUI>();
        UIHelpers.SetupTextMesh(textComponent, Plugin.THICK_PIXEL_8PT_FONT, Constants.TITLE_FONT_SIZE, Constants.TITLE_FONT_COLOR, text);
        
        RectTransform rectTransform = textComponent.rectTransform;
        rectTransform.SetParent(parentWindow.transform, false);
        UIHelpers.SetupRectTransform(rectTransform, AnchorPosition.TopCenter, sizeDelta: Constants.PANEL_TITLE_SIZE, anchoredPosition: new Vector2(0, -2));
        return textComponent;
    }

    private GameObject ConstructResetButton(GameObject parentWindow)
    {
        GameObject resetButton = UIHelpers.SetupSpriteSwapButton(parentWindow, "saveButton", Plugin.SpriteCache.Get(SpriteConstants.RESET_BUTTON_TEXTURE_FILENAME).ButtonSprites);
        RectTransform rectTransform = resetButton.GetComponent<RectTransform>();
        UIHelpers.SetupRectTransform(rectTransform, AnchorPosition.BottomLeft, sizeDelta: SpriteConstants.BUTTON_77x16_SPRITE_SIZE, anchoredPosition: SAVE_RESET_BUTTON_ANCHOR_POSITION);
        rectTransform.SetParent(parentWindow.transform, false);
        resetButton.GetComponent<Button>().onClick.AddListener(OnResetClick);
        return resetButton;
    }

    private GameObject ConstructSaveButton(GameObject parentWindow)
    {
        GameObject saveButton = UIHelpers.SetupSpriteSwapButton(parentWindow, "saveButton", Plugin.SpriteCache.Get(SpriteConstants.SAVE_BUTTON_TEXTURE_FILENAME).ButtonSprites);
        RectTransform rectTransform = saveButton.GetComponent<RectTransform>();
        UIHelpers.SetupRectTransform(rectTransform, AnchorPosition.BottomRight, sizeDelta: SpriteConstants.BUTTON_77x16_SPRITE_SIZE, anchoredPosition: SAVE_RESET_BUTTON_ANCHOR_POSITION * new Vector2(-1, 1));
        rectTransform.SetParent(parentWindow.transform, false);
        saveButton.GetComponent<Button>().onClick.AddListener(OnSaveClick);
        return saveButton;
    }

    private GameObject ConstructModConfigList(List<PluginTuple> plugins)
    {
        GameObject modConfigListObj = new GameObject("ModConfigList", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        
        // Add VBox to make content automatically stack
        VerticalLayoutGroup vbox = modConfigListObj.GetComponent<VerticalLayoutGroup>();
        vbox.childAlignment = TextAnchor.UpperCenter;
        vbox.spacing = 2;
        vbox.childForceExpandWidth = true;
        vbox.childForceExpandHeight = false;
        vbox.childControlHeight = vbox.childControlWidth = false;
        
        ContentSizeFitter contentFitter = modConfigListObj.GetComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // For each registered plugin we are going to create a button that opens a menu with their options
        foreach (PluginTuple plugin in plugins)
        {
            GameObject modConfigButton =  UIHelpers.SetupSpriteSwapButton(modConfigListObj, plugin.Metadata.Name, Plugin.SpriteCache.Get(SpriteConstants.MAIN_MENU_BUTTON_TEXTURE_FILENAME).ButtonSprites);
            modConfigButton.transform.SetParent(modConfigListObj.transform, false);
            Image image = modConfigButton.GetComponent<Image>();
            
            Button button = modConfigButton.GetComponent<Button>();
            button.onClick.AddListener(delegate { OpenModConfigMenu(plugin); });

            // Set the button text with the mod name
            GameObject innerTextObject = new GameObject($"{plugin.Metadata.Name}_SettingsMenuButton_Text");
            innerTextObject.transform.SetParent(modConfigButton.transform, false);
            RectTransform innerTextRT = innerTextObject.AddComponent<RectTransform>();
            UIHelpers.SetupRectTransform(innerTextRT, new Vector2(0.5f, 0.5f), image.sprite.rect.size);
            innerTextRT.SetParent(modConfigButton.transform, false);
            TextMeshProUGUI innerText = innerTextObject.AddComponent<TextMeshProUGUI>();
            UIHelpers.SetupTextMesh(innerText, Plugin.THICK_PIXEL_8PT_FONT, Constants.PARAGRAPH_FONT_SIZE, Constants.BODY_FONT_COLOR, plugin.Metadata.Name);
        }
        
        RectTransform modConfigListRT = modConfigListObj.GetComponent<RectTransform>();
        UIHelpers.SetupFillRectTransform(modConfigListRT);
        modConfigListRT.pivot = new Vector2(0.5f, 1);
        
        
        return modConfigListObj;
    }

    private GameObject ConstructSpecificModConfigMenu(PluginTuple plugin)
    {
        GameObject modConfigObj = new GameObject("ModConfig", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        
        // Add VBox to make content automatically stack
        VerticalLayoutGroup vbox = modConfigObj.GetComponent<VerticalLayoutGroup>();
        vbox.childAlignment = TextAnchor.UpperLeft;
        vbox.childForceExpandWidth = vbox.childControlWidth = true;
        vbox.childForceExpandHeight = vbox.childControlHeight = false;
        vbox.spacing = 3;
        
        ContentSizeFitter contentFitter = modConfigObj.GetComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        foreach (ConfigDefinition def in plugin.Config.Keys)
        {
            try
            {
                BaseModOption modOption = ModOptionFactory.Construct(plugin.Metadata, plugin.Config[def]);
                modOptions.Add(modOption);
                ModOptionContainer moc = new ModOptionContainer(modOption);
                moc.Container.transform.SetParent(modConfigObj.transform, false);
            }
            catch (KeyNotFoundException) {}
            
        }

        RectTransform modConfigRT = modConfigObj.GetComponent<RectTransform>();
        UIHelpers.SetupFillRectTransform(modConfigRT);
        modConfigRT.pivot = new Vector2(0.5f, 1);
        
        return modConfigObj;
    }

    private void OnResetClick()
    {
        foreach (BaseModOption modOption in modOptions)
        {
            modOption.PreReset();
            modOption.ConfigEntry.BoxedValue = modOption.ConfigEntry.DefaultValue;
            modOption.PostReset();

        }
    }

    private void OnSaveClick()
    {
        foreach (BaseModOption modOption in modOptions)
        {
            modOption.PreSave();
        }
        currentPlugin.Config.Save();
        foreach (BaseModOption modOption in modOptions)
        {
            modOption.PostSave();
        }
    }
}