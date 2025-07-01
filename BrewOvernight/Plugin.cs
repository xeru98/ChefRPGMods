using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using XeruUtils;

namespace BrewOvernight
{
    
    [BepInPlugin("com.xeru98.chefrpgmods.brewovernight", "Brew Overnight", "1.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"{Info} Plugin Loaded");
        
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }
        
        [HarmonyPatch(typeof(BrewingAppliance), "StartBrewing_Phase_1")]
        [HarmonyPatch(typeof(BrewingAppliance), "StartBrewing_Phase_2")]
        [HarmonyPatch(typeof(BrewingAppliance), "StartBrewing_Phase_3")]
        [HarmonyPostfix]
        static void StartBrewing_Patch(BrewingAppliance __instance)
        {
            Logger.LogDebug($"StartBrewing_Patch");
            BrewingIndicator brewingIndicator = FieldHelpers.GetPrivateFieldValue<BrewingIndicator, BrewingAppliance>(__instance, "brewingIndicator");

            if (!brewingIndicator)
            {
                Logger.LogError("BrewingIndicator field not found or is null");
            }

            float newEndTime = ((((((PlayStaticStats.WorldYears - 1) * 4) + (PlayStaticStats.CurrentSeason - 1)) * 28) + (PlayStaticStats.WorldDate - (PlayStaticStats.WorldHours < 7 ? 1 : 0))) * 24 + 6) * 60;
            brewingIndicator.InitializeProgressBar(brewingIndicator.TargetItemID, brewingIndicator.ItemAmount, brewingIndicator.StartTime, newEndTime);
        }
    }
}