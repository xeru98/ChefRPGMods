using System.Collections.Generic;
using System.Linq;
using ModConfigMenu.Components;
using ModConfigMenu.Framework.ModOption;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using XeruUtils;
using XeruUtils.Components;

namespace ModConfigMenu.Framework;

internal class ModConfigUI
{
    private GameObject startMenuWindow;
    
    private ScrollView scrollView;
    private TextMeshProUGUI titleText;
    private GameObject backButton;
    private ModConfig currentConfig = null;

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
        UIHelpers.SetupRectTransform(scrollView.Root.GetComponent<RectTransform>(), Vector2.zero, sizeDelta: new Vector2(324, 172), anchoredPosition: new Vector2(20, 16));
        
        // Add extra panel components
        titleText = ConstructTitleText(window, Constants.DEFAULT_PANEL_TITLE);
        ConstructCloseButton(window);
        backButton = ConstructBackButton(window);
        return window;
    }
    
    private void Close()
    {
        currentConfig?.NotifyPreMenuClose();
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
        
        // if we have a null config then we can just load a list of all mods with configs.
        if (currentConfig == null)
        {
            titleText.text = Constants.DEFAULT_PANEL_TITLE;
            backButton.SetActive(false);
            GameObject modConfigList = ConstructModConfigList(Plugin.Instance.GetModConfigManager());
            scrollView.SetContent(modConfigList);
        }
        else
        {
            titleText.text = currentConfig.ModName;
            backButton.SetActive(true);
            GameObject modConfigWidget = ConstructSpecificModConfigMenu(currentConfig);
            scrollView.SetContent(modConfigWidget);
        }
    }

    private void OpenModConfigMenu(ModConfig config)
    {
        currentConfig = config;
        currentConfig.NotifyPreMenuOpen();
        OpenStartMenu();
    }
    
    private void ReturnToModList()
    {
        currentConfig.NotifyPreMenuClose();
        currentConfig = null;
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
        UIHelpers.SetupTextMesh(textComponent, Plugin.FONT, Constants.TITLE_FONT_SIZE, Constants.TITLE_FONT_COLOR, text);
        
        RectTransform rectTransform = textComponent.rectTransform;
        rectTransform.SetParent(parentWindow.transform, false);
        UIHelpers.SetupRectTransform(rectTransform, AnchorPosition.TopCenter, sizeDelta: Constants.PANEL_TITLE_SIZE, anchoredPosition: new Vector2(0, -2));
        return textComponent;
    }

    private GameObject ConstructModConfigList(ModConfigManager manager)
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
        
        foreach (ModConfig modConfig in manager.GetAll())
        {
            GameObject modConfigButton =  UIHelpers.SetupSpriteSwapButton(modConfigListObj, modConfig.ModName, Plugin.SpriteCache.Get(SpriteConstants.MAIN_MENU_BUTTON_TEXTURE_FILENAME).ButtonSprites);
            modConfigButton.transform.SetParent(modConfigListObj.transform, false);
            Image image = modConfigButton.GetComponent<Image>();
            
            Button button = modConfigButton.GetComponent<Button>();
            button.onClick.AddListener(delegate { OpenModConfigMenu(modConfig); });

            GameObject innerTextObject = new GameObject("MainMenu_ModSettingsButton_Text");
            innerTextObject.transform.SetParent(modConfigButton.transform, false);
            RectTransform innerTextRT = innerTextObject.AddComponent<RectTransform>();
            UIHelpers.SetupRectTransform(innerTextRT, new Vector2(0.5f, 0.5f), image.sprite.rect.size);
            innerTextRT.SetParent(modConfigButton.transform, false);
            TextMeshProUGUI innerText = innerTextObject.AddComponent<TextMeshProUGUI>();
            UIHelpers.SetupTextMesh(innerText, Plugin.FONT, Constants.PARAGRAPH_FONT_SIZE, Constants.BODY_FONT_COLOR, modConfig.ModName);
        }
        
        RectTransform modConfigListRT = modConfigListObj.GetComponent<RectTransform>();
        UIHelpers.SetupFillRectTransform(modConfigListRT);
        modConfigListRT.pivot = new Vector2(0.5f, 1);
        
        
        return modConfigListObj;
    }

    private GameObject ConstructSpecificModConfigMenu(ModConfig config)
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
        
        foreach (BaseModOption option in config.Options)
        {
            ModOptionContainer moc = new ModOptionContainer(option, false);
            moc.Container.transform.SetParent(modConfigObj.transform, false);
        }

        RectTransform modConfigRT = modConfigObj.GetComponent<RectTransform>();
        UIHelpers.SetupFillRectTransform(modConfigRT);
        modConfigRT.pivot = new Vector2(0.5f, 1);
        
        return modConfigObj;
    }
}