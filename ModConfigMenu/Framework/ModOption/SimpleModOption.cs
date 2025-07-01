using System;
using BepInEx.Logging;
using UnityEngine;

namespace ModConfigMenu.Framework.ModOption;

/// <summary>
/// This is the core of all basic mod options. Is stores a single cached value that is committed via SetValue
///  when the config is saved. Updating the mod before saving will update cachedValue
/// </summary>
/// <typeparam name="T"></typeparam>
internal abstract class SimpleModOption<T> : BaseModOption
{
    /// <summary>The cached value read from the mod config</summary>
    protected T CachedValue;
    
    protected readonly Func<T> GetValue;
    protected readonly Action<T> SetValue;
    
    public Type Type => typeof(T);

    public virtual T Value
    {
        get => CachedValue;
        set
        {
            if (!CachedValue.Equals(value))
            {
                Owner.ChangeHandlers.ForEach(handler => handler(FieldId, value));
            }
        }
    }

    private void GetLatest()
    {
        CachedValue = GetValue();
    }

    public SimpleModOption(string fieldId, 
        Func<string> name, 
        Func<string> tooltip, 
        ModConfig owner, 
        Func<T> getValue,
        Action<T> setValue)
        : base(fieldId, name, tooltip, owner)
    {
        GetValue = getValue;
        SetValue = setValue;
        
        GetLatest();
    }

    public override void PreReset()
    {
        GetLatest();
    }

    public override void PostReset()
    {
        GetLatest();
    }

    public override void PreSave()
    {
        SetValue(CachedValue);
    }

    public override void PostSave() {}

    public override void PreMenuOpened()
    {
        GetLatest();
    }

    public override void PreMenuClosed()
    {}
}