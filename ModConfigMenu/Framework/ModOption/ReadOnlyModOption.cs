using System;
using BepInEx;

namespace ModConfigMenu.Framework.ModOption;

/// <summary>
/// This class is used for displaying readonly options (like Titles)
/// These options don't have fieldids since they can't be changed and
/// don't need to be monitored by the change handlers
/// </summary>
internal abstract class ReadOnlyModOption : BaseModOption
{
    // Since there is no value being reset these functions do nothing
    /// <inheritdoc />
    public override void PreReset() {}
    
    /// <inheritdoc />
    public override void PostReset() {}
    
    /// <inheritdoc />
    public override void PreSave() {}
    
    /// <inheritdoc />
    public override void PostSave() {}
    
    /// <inheritdoc />
    public override void PreMenuOpened() {}
    
    /// <inheritdoc />
    public override void PreMenuClosed() {}

    protected ReadOnlyModOption(Func<string> name, Func<string> tooltip, ModConfig owner) 
        : base(null, name, tooltip, owner) { }
}