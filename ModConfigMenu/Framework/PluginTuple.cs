using BepInEx;
using BepInEx.Configuration;

namespace ModConfigMenu.Framework;

public class PluginTuple
{
    public readonly BepInPlugin Metadata;
    public readonly ConfigFile Config;
    
    private PluginTuple(BaseUnityPlugin plugin)
    {
        Metadata = plugin.Info.Metadata;
        Config = plugin.Config;
    }
    
    public static bool isValidPlugin(BaseUnityPlugin plugin)
    {
        return plugin.Info != null && plugin.Info.Metadata != null && plugin.Config != null;
    }

    public static PluginTuple FromBaseUnityPlugin(BaseUnityPlugin plugin)
    {
        return new PluginTuple(plugin);
    }
}