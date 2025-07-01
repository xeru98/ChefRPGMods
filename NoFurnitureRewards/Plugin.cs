using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;


namespace NoFurnitureRewards;

[BepInPlugin("com.xeru98.chefrpgmods.nofurniturerewards", "No Furniture Rewards", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
        
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"NoFurnitureRewards Plugin Loaded");
        Harmony.CreateAndPatchAll(typeof(Plugin));
    }

    [HarmonyPatch(typeof(RestaurantGameUI), "RandomRewardType")]
    [HarmonyPrefix]
    static bool RandomRewardType_Patch(ref int __result)
    {
        Logger.LogDebug("Rolling Random Reward Type from NoFurnitureRewards Mod");
        Random r = new Random();
        List<int> rewardTypes = new List<int>(){2, 15};
        __result = rewardTypes[r.Next(0, rewardTypes.Count)];
        return false;
    }
}