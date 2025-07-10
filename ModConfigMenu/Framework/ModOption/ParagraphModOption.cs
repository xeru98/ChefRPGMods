using BepInEx;
using TMPro;
using UnityEngine;
using XeruUtils;

namespace ModConfigMenu.Framework.ModOption;

/// <summary>
/// Displays a line of readonly text. Text maps to Name() in base object and there is no fieldId
/// </summary>
internal class ParagraphModOption : ReadOnlyModOption
{
    public ParagraphModOption(BepInPlugin plugin, string text)
        :base(plugin, text) {}

    public override GameObject GetUIGameObject()
    {
        GameObject gameObject = new GameObject($"{FieldId()}_Paragraph");
        TextMeshProUGUI textMesh = gameObject.AddComponent<TextMeshProUGUI>();
        UIHelpers.SetupTextMesh(textMesh, Plugin.THICK_PIXEL_8PT_FONT, Constants.PARAGRAPH_FONT_SIZE, Constants.BODY_FONT_COLOR, Text);
        textMesh.alignment = TextAlignmentOptions.MidlineLeft;

        return gameObject;
    }
}