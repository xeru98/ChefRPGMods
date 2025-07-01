using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using XeruUtils;

namespace FixRhodium
{
    [BepInPlugin("com.xeru98.chefrpgmods.fixrhodium", "Fix Rhodium", "1.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        
        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"FixRhodium Plugin Loaded");
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(MiningRock), "ActivateRhodium")]
        [HarmonyPostfix]
        static void ActivateRhodium_Patch(MiningRock __instance)
        {
            GameObject extraRock = FieldHelpers.GetPrivateFieldValue<GameObject, MiningRock>(__instance, "ExtraRock");
            extraRock.SetActive(true);
        }
    }
}