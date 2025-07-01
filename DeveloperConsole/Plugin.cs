using BepInEx;
using BepInEx.Logging;


namespace DeveloperConsole;

[BepInPlugin("com.xeru98.chefrpgmods.developerconsole", "Developer Console", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
        
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"DeveloperConsole Plugin Loaded");
        PlayStaticStats.TestingBuild = true;
        Logger.LogInfo($"Developer Console Enabled: {PlayStaticStats.TestingBuild}");
    }
}