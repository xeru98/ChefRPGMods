using System;
using System.Collections.Generic;
using BepInEx;
using ModConfigMenu.Framework.ModOption;

namespace ModConfigMenu.Framework;

internal class ModConfigManager
{
    Dictionary<string, ModConfig> registeredConfigs = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Attempts to retrieve a registered mod config
    /// </summary>
    /// <param name="plugin">The BepInEx plugin metadata</param>
    /// <param name="assert">If true, will cause the plugin to throw an exception when a key is not found</param>
    /// <returns>The found ModConfig or null</returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public ModConfig Get(BepInPlugin plugin, bool assert)
    {
        AssertMetadata(plugin);

        lock (registeredConfigs)
        {
            if (registeredConfigs.TryGetValue(plugin.GUID, out var modConfig))
            {
                return modConfig;
            }

            return assert 
                ? throw new KeyNotFoundException($"ModConfig not found for GUID: {plugin.GUID}")
                : null;
        }
    }

    /// <summary>
    /// Returns all the ModConfigs that have been registered
    /// </summary>
    public IEnumerable<ModConfig> GetAll()
    {
        lock (registeredConfigs)
        {
            return registeredConfigs.Values;
        }
    }

    /// <summary>
    /// Checks if a plugin has been registered
    /// </summary>
    /// <param name="plugin">The BepInEx plugin metadata</param>
    /// <returns>true if the config is registered, otherwise false</returns>
    public bool IsRegistered(BepInPlugin plugin)
    {
        AssertMetadata(plugin);

        lock (registeredConfigs)
        {
            return registeredConfigs.ContainsKey(plugin.GUID);
        }
    }

    /// <summary>
    /// Sets the value of a given plugin in the cache
    /// </summary>
    /// <param name="plugin">The BepInEx plugin metadata</param>
    /// <param name="config">The ModConfig for the registered plugin</param>
    /// <exception cref="ArgumentNullException">Thrown if the input config is null</exception>
    public void Set(BepInPlugin plugin, ModConfig config)
    {
        lock (registeredConfigs)
        {
            AssertMetadata(plugin);
            registeredConfigs[plugin.GUID] = config ?? throw new ArgumentNullException(nameof(config));
        }
    }

    /// <summary>
    /// Removes a ModConfig for the given plugin if it exists
    /// </summary>
    /// <param name="plugin">The BepInEx plugin metadata</param>
    public void Remove(BepInPlugin plugin)
    {
        lock (registeredConfigs)
        {
            AssertMetadata(plugin);

            if (registeredConfigs.ContainsKey(plugin.GUID))
            {
                registeredConfigs.Remove(plugin.GUID);
            }
        }
    }

    /// <summary>
    /// Asserts that the metadata for a plugin is valid
    /// </summary>
    /// <param name="plugin">The BepInEx plugin metadata</param>
    /// <exception cref="ArgumentNullException"></exception>
    private void AssertMetadata(BepInPlugin plugin)
    {
        if (plugin == null)
        {
            throw new ArgumentNullException(nameof(plugin));
        }

        if (string.IsNullOrWhiteSpace(plugin.GUID))
        {
            throw new ArgumentNullException($"The plugin {plugin.Name} must have a valid GUID.");
        }
        
        if (string.IsNullOrWhiteSpace(plugin.Name))
        {
            throw new ArgumentNullException($"The plugin {plugin.GUID} must have a valid Name.");
        }
    }
}