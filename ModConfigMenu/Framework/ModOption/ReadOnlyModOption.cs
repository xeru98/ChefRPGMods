using BepInEx;

namespace ModConfigMenu.Framework.ModOption;

/// <summary>
/// This class is used for displaying readonly options (like Titles)
/// These options don't have fieldids since they can't be changed and
/// don't need to be monitored by the change handlers
/// </summary>
internal abstract class ReadOnlyModOption : BaseModOption
{
    internal string Text { get; private set;}
    
    // Since there is no value being reset these functions do nothing
    /// <inheritdoc />
    public override void PreReset() {}
    
    /// <inheritdoc />
    public override void PostReset() {}
    
    /// <inheritdoc />
    public override void PreSave() {}
    
    /// <inheritdoc />
    public override void PostSave() {}

    protected ReadOnlyModOption(BepInPlugin plugin, string text)
        : base(plugin, null)
    {
        Text = text;
    }
}