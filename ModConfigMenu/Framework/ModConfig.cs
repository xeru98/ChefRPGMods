using System;
using System.Collections.Generic;
using BepInEx;
using ModConfigMenu.Framework.ModOption;

namespace ModConfigMenu.Framework;

internal class ModConfig
{
    /// <summary>This is the stored metadata for the mod</summary>
    public BepInPlugin PluginMetadata { get; }
    
    /// <summary>
    /// This is the resolved mod name that we will display in the mod config menu
    /// </summary>
    public string ModName => PluginMetadata.Name;
    
    /// <summary>
    /// Resets a mod's config to the default values
    /// </summary>
    public Action Reset { get; }
    
    /// <summary>
    /// Saves the current config values to disk
    /// </summary>
    public Action Save { get; }
    
    /// <summary>
    /// The callback functions to call when a value changes.
    /// </summary>
    public List<Action<string, object>> ChangeHandlers { get; } = new();
    
    public List<BaseModOption> Options { get; } = new();

    public ModConfig(BepInPlugin plugin, Action reset, Action save)
    {
        PluginMetadata = plugin;
        Reset = reset;
        Save = save;
    }

    public void AddOption(BaseModOption option)
    {
        Options.Add(option);
    }

    public void NotifyPreMenuOpen()
    {
        foreach (BaseModOption option in Options)
        {
            option.PreMenuOpened();
        }
    }

    public void NotifyPreMenuClose()
    {
        foreach (BaseModOption option in Options)
        {
            option.PreMenuClosed();
        }
    }
}