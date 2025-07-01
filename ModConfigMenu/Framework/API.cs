using System;
using System.Linq;
using BepInEx;
using ModConfigMenu.Framework.ModOption;
using UnityEngine;
using UnityEngine.UI;

namespace ModConfigMenu.Framework;

public class API : IModConfigMenuApi
{
    private ModConfigManager manager;
    public void Register(BepInPlugin metadata, Action reset, Action save)
    {
        AssertNotNull(reset);
        AssertNotNull(save);
        
        if (manager.IsRegistered(metadata))
        {
            throw new InvalidOperationException($"ModConfigMenu is already registered for {metadata.GUID}");
        }
        manager.Set(metadata, new ModConfig(metadata, reset, save));
    }
    
    public void Unregister(BepInPlugin metadata)
    {
        if (!manager.IsRegistered(metadata))
        {
            throw new InvalidOperationException($"ModConfigManager could not find registered ModConfig for {metadata.GUID}");
        }
        manager.Remove(metadata);
    }

    public void AddSectionTitle(BepInPlugin metadata, Func<string> text, Func<string> tooltip = null)
    {
        throw new NotImplementedException();
    }

    public void AddParagraph(BepInPlugin metadata, Func<string> text)
    {
        throw new NotImplementedException();
    }

    public void AddBoolOption(BepInPlugin metadata, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null,
        string fieldId = null)
    {
        AssertNotNull(name);
        AssertNotNull(getValue);
        AssertNotNull(setValue);
        
        ModConfig config = manager.Get(metadata, true);
        
        config.AddOption(new ToggleModOption(fieldId, name, tooltip, config, getValue, setValue));
    }

    public void AddNumberOption(BepInPlugin metadata, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null,
        int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null)
    {
        AddNumericOption<int>(metadata, name, tooltip, getValue, setValue, min, max, interval, fieldId, formatValue);
    }

    public void AddNumberOption(BepInPlugin metadata, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null,
        float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null)
    {
        AddNumericOption<float>(metadata, name, tooltip, getValue, setValue, min, max, interval, fieldId, formatValue);
    }

    public void AddTextOption(BepInPlugin metadata, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null,
        string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null)
    {
        throw new NotImplementedException();
    }

    public void AddPage(BepInPlugin metadata, string pageId, Func<string> pageTitle = null)
    {
        throw new NotImplementedException();
    }

    public void AddPageLink(BepInPlugin metadata, string pageId, Func<string> text, Func<string> tooltip = null)
    {
        throw new NotImplementedException();
    }

    public void AddComplexOption(BepInPlugin metadata, Func<string> name, Func<GameObject> widget, Func<string> tooltip = null, Action beforeMenuOpened = null,
        Action beforeSave = null, Action afterSave = null, Action beforeReset = null, Action afterReset = null,
        Action beforeMenuClosed = null, Func<int> height = null, string fieldId = null)
    {
        throw new NotImplementedException();
    }

    public void SetTitleScreenOnlyForNextOptions(BepInPlugin metadata, bool titleScreenOnly)
    {
        throw new NotImplementedException();
    }

    public void OnFieldChanged(BepInPlugin metadata, Action<string, object> onChange)
    {
        throw new NotImplementedException();
    }

    private void AddNumericOption<T>(BepInPlugin metadata, Func<string> name, Func<string> tooltip, Func<T> getValue, Action<T> setValue, T? min, T? max, T? interval, string fieldId, Func<T, string> formatValue) where T : struct, IComparable, IConvertible
    {
        AssertNotNull(name);
        AssertNotNull(getValue);
        AssertNotNull(setValue);
        
        Type[] validTypes = new Type[] { typeof(int), typeof(float) };
        if (!validTypes.Contains(typeof(T)))
        {
            throw new ArgumentException($"Type '{typeof(T)}' registered by {metadata.GUID} is not a valid type for numberic mod option. Valid types are [int, float]");
        }
        
        ModConfig config = manager.Get(metadata, true);
        
        config.AddOption(new NumericModOption<T>(fieldId, name, tooltip, config, getValue, setValue, min, max, interval, formatValue));
    }
    private void AssertNotNull(object value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
    }
}