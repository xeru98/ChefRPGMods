using HarmonyLib;
using TMPro;
using UnityEngine;
using XeruUtils;
using UnityEngine.InputSystem;

namespace ModConfigMenu;

public class StartMenuPatches
{
    
    /// <summary>
    /// This checks if the menu button (Escape by default) was pressed
    ///  and if the front menu is hidden but the modConfigMenu is visible.
    ///  If so then we want to trigger BackToFrontMenu()
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPatch(typeof(StartMenuManager), "Update")]
    [HarmonyPostfix]
    private static void Update_Patch(StartMenuManager __instance)
    {
        PlayerInput pi = FieldHelpers.GetPrivateFieldValue<PlayerInput, StartMenuManager>(__instance, "PlayerInput");
        GameObject frontMenu = FieldHelpers.GetPrivateFieldValue<GameObject, StartMenuManager>(__instance, "FrontMenu");
        if (pi.actions["Menu"].WasPressedThisFrame() 
            && !frontMenu.activeInHierarchy
            && Plugin.Instance.modConfigUI.GetStartMenuWindow().activeInHierarchy)
        {
            __instance.BackToFrontMenu();
        }
    }

    /// <summary>
    /// This hides the modConfigMenu whenever we are navigating to one of the other
    ///  start scene menus.
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPatch(typeof(StartMenuManager), "BackToFrontMenu")]
    [HarmonyPatch(typeof(StartMenuManager), "OpenLoadSlots")]
    [HarmonyPatch(typeof(StartMenuManager), "OpenSettings")]
    [HarmonyPatch(typeof(StartMenuManager), "OpenCredits")]
    [HarmonyPostfix]
    private static void BackToFrontMenu_Patch(StartMenuManager __instance)
    {
        Plugin.Instance.modConfigUI.GetStartMenuWindow()?.SetActive(false);
    }
}