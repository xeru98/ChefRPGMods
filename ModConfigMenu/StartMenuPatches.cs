using HarmonyLib;
using UnityEngine;
using XeruUtils;
using UnityEngine.InputSystem;

namespace ModConfigMenu;

public class StartMenuPatches
{
    
    [HarmonyPatch(typeof(StartMenuManager), "Update")]
    [HarmonyPostfix]
    static void Update_Patch(StartMenuManager __instance)
    {
        PlayerInput pi = FieldHelpers.GetPrivateFieldValue<PlayerInput, StartMenuManager>(__instance, "PlayerInput");
        GameObject frontMenu = FieldHelpers.GetPrivateFieldValue<GameObject, StartMenuManager>(__instance, "FrontMenu");
        
        if (pi.actions["Menu"].WasPressedThisFrame() && !frontMenu.activeInHierarchy)
        {
            if (Plugin.Instance.modConfigUI.GetStartMenuWindow().activeInHierarchy)
            {
                __instance.BackToFrontMenu();
            }
        }
    }

    [HarmonyPatch(typeof(StartMenuManager), "BackToFrontMenu")]
    [HarmonyPatch(typeof(StartMenuManager), "OpenLoadSlots")]
    [HarmonyPatch(typeof(StartMenuManager), "OpenSettings")]
    [HarmonyPatch(typeof(StartMenuManager), "OpenCredits")]
    [HarmonyPostfix]
    static void BackToFrontMenu_Patch(StartMenuManager __instance)
    {
        Plugin.Instance.modConfigUI.GetStartMenuWindow()?.SetActive(false);
    }
}