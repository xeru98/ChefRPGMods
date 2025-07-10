using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ModConfigMenu.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XeruUtils;
using Constants = ModConfigMenu.Framework.Constants;

namespace ModConfigMenu
{
    [BepInPlugin("com.xeru98.chefrpg.modconfigmenu", "Mod Config Menu", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        internal static Plugin Instance;
        internal static SpriteCache SpriteCache { get; } = new ();
        internal static TMP_FontAsset THICK_PIXEL_8PT_FONT;
        internal static TMP_FontAsset PIXEL_FONT_7PX_FONT;
        
        internal ModConfigUI modConfigUI = new ModConfigUI();
        
        internal static List<PluginTuple> LoadedPluginsWithConfigs = new List<PluginTuple>();

        private void Awake()
        {
            Instance = this;
            Logger = base.Logger;
            Logger.LogInfo($"ModConfigMenu Plugin Loaded");
            
            SceneManager.sceneLoaded += OnSceneLoaded;
            RegisterSprites();

            // Configure patches
            Harmony.CreateAndPatchAll(typeof(StartMenuPatches));

            // Create tuples of all valid plugins so we have access to both the config and the metadata
            LoadedPluginsWithConfigs = FindPlugins()
                .Where(PluginTuple.isValidPlugin)
                .Select(PluginTuple.FromBaseUnityPlugin)
                .ToList();
            
            // Add this to your main plugin initialization
            if (EventSystem.current == null)
            {
                Plugin.Logger.LogWarning("No EventSystem found! Creating one...");
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
            }
        }

        /// <summary>
        /// Use the BepInEx Chainloader to get all the configs of the mods that have been registered
        /// </summary>
        /// <returns></returns>
        private List<BaseUnityPlugin> FindPlugins()
        {
            return Chainloader.PluginInfos.Values.Select((PluginInfo info) => info.Instance)
                .Where((BaseUnityPlugin plugin) => plugin != null)
                .Union(FindObjectsOfType<BaseUnityPlugin>())
                .ToList();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!THICK_PIXEL_8PT_FONT || !PIXEL_FONT_7PX_FONT)
            {
                TMP_FontAsset[] loadedTMPFonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
                THICK_PIXEL_8PT_FONT = THICK_PIXEL_8PT_FONT ? THICK_PIXEL_8PT_FONT : loadedTMPFonts.First(f => f.name == Constants.THICK_PIXEL_8PT_FONT_NAME);
                PIXEL_FONT_7PX_FONT = PIXEL_FONT_7PX_FONT ? PIXEL_FONT_7PX_FONT : loadedTMPFonts.First(f => f.name == Constants.PIXEL_FONT_7PX_FONT_NAME);
            }
            
            if (scene.name == "100 Start Menu")
            {
                Logger.LogInfo($"Game scene loaded: {scene.name}");
                StartMenuManager sm = FindObjectOfType<StartMenuManager>();
                GameObject frontMenu = FieldHelpers.GetPrivateFieldValue<GameObject, StartMenuManager>(sm, "FrontMenu");
                float shift = float.NegativeInfinity;
                foreach (Transform child in frontMenu.transform)
                {
                    if (SpriteConstants.FRONT_MENU_BUTTON_NAMES_TO_MOVE.Contains(child.gameObject.name))
                    {
                        shift = Math.Max(shift, child.localPosition.y);
                        child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y - 20, child.localPosition.z);
                    }
                }

                GameObject modSettingsButton = CreateMainMenuSettingsButton();
                modSettingsButton.transform.SetParent(frontMenu.transform, false);
                RectTransform rectTransform = modSettingsButton.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, shift);
            }
        }

        private GameObject CreateMainMenuSettingsButton()
        {
            StartMenuManager sm = FindObjectOfType<StartMenuManager>();
            GameObject frontMenu = FieldHelpers.GetPrivateFieldValue<GameObject, StartMenuManager>(sm, "FrontMenu");

            GameObject modSettingsButton = UIHelpers.SetupSpriteSwapButton(frontMenu, "ModSettings", SpriteCache.Get(SpriteConstants.MAIN_MENU_BUTTON_TEXTURE_FILENAME).ButtonSprites);
            RectTransform rectTransform = modSettingsButton.GetComponent<RectTransform>();
            Vector2 sizeDelta = modSettingsButton.GetComponent<Image>().sprite.rect.size;
            UIHelpers.SetupRectTransform(rectTransform, AnchorPosition.Center, sizeDelta);
            
            Button button = modSettingsButton.GetComponent<Button>();
            button.onClick.AddListener(modConfigUI.OpenFromFrontMenu);

            GameObject innerTextObject = new GameObject("MainMenu_ModSettingsButton_Text");
            innerTextObject.transform.SetParent(modSettingsButton.transform, false);
            
            RectTransform innerTextRectTransform = innerTextObject.AddComponent<RectTransform>();
            UIHelpers.SetupRectTransform(innerTextRectTransform, AnchorPosition.Center, sizeDelta);
            
            TextMeshProUGUI innerText = innerTextObject.AddComponent<TextMeshProUGUI>();
            UIHelpers.SetupTextMesh(innerText, THICK_PIXEL_8PT_FONT, Constants.PARAGRAPH_FONT_SIZE, Constants.BODY_FONT_COLOR, "Mod Settings");

            return modSettingsButton;
        }

        private void RegisterSprites()
        {
            SpriteCache.LoadButtonSpritesFromAssetTexture(SpriteConstants.MAIN_MENU_BUTTON_TEXTURE_FILENAME, SpriteConstants.MAIN_MENU_BUTTON_SPRITE_SLICES, Logger);
        }
    }
}