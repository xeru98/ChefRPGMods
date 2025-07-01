using System;
using TMPro;
using UnityEngine;
using XeruUtils;

namespace ModConfigMenu.Framework.ModOption;

/// <summary>
/// Displays a line of readonly text. Text maps to Name() in base object and there is no fieldId
/// </summary>
internal class SectionHeaderModOption : ReadOnlyModOption
{
    public SectionHeaderModOption(Func<string> text, ModConfig owner)
        :base(text, null, owner) {}

    public override GameObject GetUIGameObject()
    {
        GameObject gameObject = new GameObject($"{Owner.PluginMetadata.GUID}_{FieldId}_Paragraph");
        TextMeshProUGUI textMesh = gameObject.AddComponent<TextMeshProUGUI>();
        UIHelpers.SetupTextMesh(textMesh, Plugin.FONT, Constants.SECTION_HEADER_FONT_SIZE, Constants.BODY_FONT_COLOR, Name());
        textMesh.alignment = TextAlignmentOptions.MidlineLeft;

        return gameObject;
    }
}