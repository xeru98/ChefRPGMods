using System;
using System.Collections.Generic;
using System.Data;
using BepInEx;
using BepInEx.Configuration;

namespace ModConfigMenu.Framework.ModOption;

public class ModOptionFactory
{
    private static readonly Dictionary<Type, Func<BepInPlugin, ConfigEntryBase, BaseModOption>> CustomWidgetFactories = new()
    {
        {typeof(bool), (plugin, entry) => new ToggleModOption(plugin, (ConfigEntry<bool>) entry)},
        {typeof(int), (plugin, entry) => new NumericModOption<int>(plugin, (ConfigEntry<int>) entry)},
        {typeof(float), (plugin, entry) => new NumericModOption<float>(plugin, (ConfigEntry<float>) entry)},
        {typeof(string), (plugin, entry) => new StringModOption(plugin, (ConfigEntry<string>) entry)}
    };

    public static void RegisterCustomWidget(Type optionType, Func<BepInPlugin, ConfigEntryBase, BaseModOption> customWidgetFactory)
    {
        if (CustomWidgetFactories.ContainsKey(optionType))
        {
            throw new DuplicateNameException($"Type {optionType.Namespace}_{optionType.Name} is already registered with a custom widget factory");
        }
        CustomWidgetFactories.Add(optionType, customWidgetFactory);
    }
    
    public static BaseModOption Construct(BepInPlugin plugin, ConfigEntryBase entry)
    {
        Type t = entry.SettingType;
        if(CustomWidgetFactories.TryGetValue(t, out Func<BepInPlugin, ConfigEntryBase, BaseModOption> foundFactory))
        {
            return foundFactory(plugin, entry);
        }
        else
        {
            throw new KeyNotFoundException($"Could not find a custom widget factory for type {t.Namespace}_{t.Name}");
        }
    }
}