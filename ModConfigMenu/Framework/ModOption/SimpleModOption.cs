using BepInEx;
using BepInEx.Configuration;

namespace ModConfigMenu.Framework.ModOption;

/// <summary>
/// This is the core of all basic mod options. Is stores a single cached value that is committed via SetValue
///  when the config is saved. Updating the mod before saving will update cachedValue
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SimpleModOption<T> : BaseModOption
{
    /// <summary>The cached value that is displayed in the widget pre save</summary>
    protected T CachedValue;

    public virtual T Value
    {
        get => CachedValue;
        set
        {
            if (!CachedValue.Equals(value))
            {
                ConfigEntry.BoxedValue = value;
                GetLatest();
            }
        }
    }

    protected void GetLatest()
    {
        CachedValue = (T)ConfigEntry.BoxedValue;
    }

    public SimpleModOption(BepInPlugin owner, ConfigEntry<T> configEntry)
        : base(owner, configEntry)
    {
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
        ConfigEntry.BoxedValue = CachedValue;
    }

    public override void PostSave() {}
}