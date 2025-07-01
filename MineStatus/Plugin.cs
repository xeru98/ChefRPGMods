using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using XeruUtils;


namespace MineStatus;

[BepInPlugin("com.xeru98.chefrpgmods.minestatus", "Mine Status", "1.1.1")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    internal static Plugin Instance;
    
    internal readonly List<String> MINE_SCENES = new List<String>()
    {
        "121 Caves Level 1",
        "122 Volcano Caves",
        "123 Caves Level 2"
    };

    // Texture loading params
    private readonly string TEXTURE_PATH = Path.Combine("Assets", "RawMaterials.png");
    private readonly static Vector2 SPRITE_DIMENSIONS = new Vector2(18, 18);
    private readonly static int SPRITE_PPU = 24;
    private readonly string FONT_NAME = "ThickPixel_8pt";
    private readonly List<SpriteSheetSliceWithId<int>> SPRITE_SLICES = new List<SpriteSheetSliceWithId<int>>()
    {
        new (1, new Vector2(18, 162), SPRITE_DIMENSIONS, SPRITE_PPU), // Quartz
        new (2, new Vector2(36, 162), SPRITE_DIMENSIONS, SPRITE_PPU), // Emerald
        new (3, new Vector2(54, 162), SPRITE_DIMENSIONS, SPRITE_PPU), // Ruby
        new (4, new Vector2(72, 162), SPRITE_DIMENSIONS, SPRITE_PPU), // Diamond
        new (5, new Vector2(126, 144), SPRITE_DIMENSIONS, SPRITE_PPU), // Amethyst
        new (10, new Vector2(0, 144), SPRITE_DIMENSIONS, SPRITE_PPU), // Sulfur
        new (11, new Vector2(90, 162), SPRITE_DIMENSIONS, SPRITE_PPU), // Iron
        new (12, new Vector2(108, 162), SPRITE_DIMENSIONS, SPRITE_PPU), // Copper
        new (13, new Vector2(144, 162), SPRITE_DIMENSIONS, SPRITE_PPU), // Gold
        //new (14, new Vector2(126, 162), SPRITE_DIMENSIONS, SPRITE_PPU), // Rhodium (Bugged due to Rhodium Ore not spawning)
        new (15, new Vector2(18, 126), SPRITE_DIMENSIONS, SPRITE_PPU) //Obsidian
    };
    private Dictionary<int, Sprite> ROCK_LEVEL_TO_SPIRTE = new Dictionary<int, Sprite>();
    private Dictionary<int, MiningRockCountCanvasComponents> ROCK_LEVEL_TO_CANVAS_COMPONENTS = new Dictionary<int, MiningRockCountCanvasComponents>(); // used to update text without deleting

    // canvas drawing params
    private GameObject canvasObj;
    private Canvas canvas;
    private TMP_FontAsset font;
    private readonly Vector2 DEFAULT_SPACING = new Vector2(5, 5);
    private readonly Vector2 DEFAULT_PADDING = new Vector2(5, -5); // measured from top left (5px right, 5px down)
    private readonly int FONT_SIZE = 11;
    
    // Must be static because of harmony patch function
    private static Dictionary<int, int> miningRockCountMap = new Dictionary<int, int>();
    private static bool areRockCountsLoaded = false;
        
    private void Awake()
    {
        // Plugin startup logic
        Instance = this;
        Logger = base.Logger;
        Logger.LogInfo($"MineStatus Plugin Loaded");

        if (Chainloader.PluginInfos.ContainsKey("com.xeru98.chefrpgmods.fixrhodium"))
        {
            Logger.LogInfo($"Found FixRhodium Plugin Adding Rhodium To Counts");
            SPRITE_SLICES.Add(new (14, new Vector2(126, 162), SPRITE_DIMENSIONS, SPRITE_PPU));
        }
        
        //Load the texture and slice the sprite sheet
        Texture2D texture = SpriteLoader.LoadTexture(Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            TEXTURE_PATH),
            Logger);
        ROCK_LEVEL_TO_SPIRTE = SpriteLoader.SliceSpriteSheet(texture, SPRITE_SLICES);
        
        Harmony.CreateAndPatchAll(typeof(Plugin));
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Logger.LogDebug("Scene Loaded");
        // clear the refs to prevent them from being stale
        miningRockCountMap.Clear();
        areRockCountsLoaded = false;
        
        if (!MINE_SCENES.Contains(scene.name))
        {
            // DestroyCanvas
            Destroy(canvasObj);
            return;
        }

        SetupCanvas();
    }

    

    [HarmonyPatch(typeof(MapRespawningRocks), "LoadRockData")]
    [HarmonyPostfix]
    static void LoadRockData_Patch(MapRespawningRocks __instance)
    {
        Logger.LogDebug("Rocks done loading, getting counts");
        
        MiningRock[] miningRocks = FieldHelpers.GetPrivateFieldValue<MiningRock[], MapRespawningRocks>(__instance, "MiningRocks");
        foreach (MiningRock rock in miningRocks)
        {
            if (!miningRockCountMap.ContainsKey(rock.RockLevel))
            {
                miningRockCountMap[rock.RockLevel] = 1;
            }
            else
            {
                miningRockCountMap[rock.RockLevel] += 1;
            }
        }

        Logger.LogDebug($"Count Dump");
        foreach (KeyValuePair<int, int> pair in miningRockCountMap)
        {
            Logger.LogDebug($"Mining Rock Level {pair.Key}: {pair.Value}");
        }
        
        Instance?.CreateCanvasComponents();
    }

    [HarmonyPatch(typeof(MiningRock), "DepleteRock")]
    [HarmonyPrefix] // can't do this after since it resets the level
    static bool DepleteRock_Patch(MiningRock __instance)
    {
        miningRockCountMap[__instance.RockLevel] -= 1;
        Instance?.UpdateRockCountsOnCanvas();
        return true;
    }
    
    private void SetupCanvas()
    {
        if (!font)
        {
            TMP_FontAsset[] loadedTMPFonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
            font = loadedTMPFonts.First(f => f.name == FONT_NAME);
        }
        
        if (canvas != null)
        {
            Logger.LogDebug("Skipping Canvas Setup But Clearing Components");
            foreach (Transform child in canvas.transform)
            {
                if (child.GetComponent<HorizontalLayoutGroup>() != null)
                {
                    Destroy(child.gameObject);
                }
            }
            return;
        }
        
        Logger.LogDebug("Initializing Canvas");
        canvasObj = new GameObject("Canvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scalar = canvasObj.AddComponent<CanvasScaler>();
        scalar.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        canvasObj.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(canvasObj);
        canvas.enabled = true;
    }

    private void CreateCanvasComponents()
    {
        // Use the GameManager to get canvas scale
        GameManager gm = FindObjectOfType<GameManager>();
        if (!gm)
        {
            Logger.LogError("Game Manager not found");
        }
        Canvas inGameCanvas = FieldHelpers.GetPrivateFieldValue<Canvas, GameManager>(gm, "InGameCanvas");
        float guiScale = inGameCanvas.scaleFactor;
        Logger.LogDebug($"Canvas Scale {guiScale}");
        
        Logger.LogDebug("Creating Canvas Out Wrapper");

        // create the border that will have everything
        GameObject hboxObj = new GameObject($"MiningStatus_HBox");
        hboxObj.transform.SetParent(canvasObj.transform, false);
        RectTransform parentRt = hboxObj.AddComponent<RectTransform>();
        parentRt.anchorMin = parentRt.anchorMax = parentRt.pivot = new Vector2(0, 1);
        parentRt.anchoredPosition = DEFAULT_PADDING * guiScale;
        
        HorizontalLayoutGroup hbox = hboxObj.AddComponent<HorizontalLayoutGroup>();
        hbox.childAlignment = TextAnchor.MiddleCenter;
        hbox.spacing = DEFAULT_SPACING.x * guiScale;
        hbox.childForceExpandHeight = hbox.childForceExpandWidth = false;
        hbox.childControlHeight = hbox.childControlWidth =false;
        
        Logger.LogDebug("Attaching Relevant Sprites to Canvas");
        foreach (KeyValuePair<int, int> pair in miningRockCountMap)
        {
            // avoids trying to display things like perfect quartz or empty nodes
            if (!ROCK_LEVEL_TO_SPIRTE.ContainsKey(pair.Key))
            {
                continue;
            }
            
            // VBox
            GameObject vboxObj = new GameObject($"MiningStatus_{pair.Key}_VBox");
            vboxObj.transform.SetParent(hboxObj.transform, false);
            VerticalLayoutGroup vbox = vboxObj.AddComponent<VerticalLayoutGroup>();
            vbox.childAlignment = TextAnchor.UpperCenter;
            vbox.spacing = DEFAULT_SPACING.y * guiScale;
            vbox.childForceExpandHeight = vbox.childForceExpandWidth = false;
            vbox.childControlHeight = vbox.childControlWidth =false;
            
            // Image
            GameObject imageObj = new GameObject($"MiningStatus_{pair.Key}_Image");
            imageObj.transform.SetParent(vboxObj.transform, false);
            Image image = imageObj.AddComponent<Image>();
            image.sprite = ROCK_LEVEL_TO_SPIRTE[pair.Key];
            image.rectTransform.sizeDelta = image.sprite.rect.size * inGameCanvas.scaleFactor;
            image.color = Color.white; // ensures there is no tint
            
            // Text
            GameObject textObj = new GameObject($"MiningStatus_{pair.Key}_Text");
            textObj.transform.SetParent(vboxObj.transform, false);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = $"{pair.Value}";
            text.font = font;
            text.fontSize = FONT_SIZE * inGameCanvas.scaleFactor;
            text.color = Color.gray;
            text.alignment = TextAlignmentOptions.Center;

            // store refs to these values for easy updating
            ROCK_LEVEL_TO_CANVAS_COMPONENTS[pair.Key] = new MiningRockCountCanvasComponents(vboxObj, text);
        }
    }

    private void UpdateRockCountsOnCanvas()
    {
        foreach (KeyValuePair<int, MiningRockCountCanvasComponents> pair in ROCK_LEVEL_TO_CANVAS_COMPONENTS)
        {
            // if there is nothing left of this type, then we can stop displaying it
            if (!miningRockCountMap.ContainsKey(pair.Key) || miningRockCountMap[pair.Key] == 0)
            {
                Destroy(pair.Value.vbox);
            }
            else // otherwise update the count displayed
            {
                pair.Value.text.text = miningRockCountMap[pair.Key].ToString();
            }
        }
    }
    
    private struct MiningRockCountCanvasComponents
    {
        public readonly GameObject vbox;
        public readonly TextMeshProUGUI text;

        public MiningRockCountCanvasComponents(GameObject vbox, TextMeshProUGUI text)
        {
            this.vbox = vbox;
            this.text = text;
        }
    }
}

