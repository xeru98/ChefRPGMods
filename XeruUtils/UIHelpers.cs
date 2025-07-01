using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XeruUtils;

public class UIHelpers
{
    public static void SetupRectTransform(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2? sizeDelta = null, Vector2? anchoredPosition = null, Vector2? offsetMin = null, Vector2? offsetMax = null)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = pivot;
        if (sizeDelta.HasValue)
        {
            rectTransform.sizeDelta = sizeDelta.Value;
        }

        if (anchoredPosition.HasValue)
        {
            rectTransform.anchoredPosition = anchoredPosition.Value;
        }

        if (offsetMin.HasValue && offsetMax.HasValue)
        {
            rectTransform.offsetMin = offsetMin.Value;
            rectTransform.offsetMax = offsetMax.Value;
        }
    }

    public static void SetupRectTransform(RectTransform rectTransform, Vector2 anchorAndPivot, Vector2? sizeDelta = null, Vector2? anchoredPosition = null)
    {
        SetupRectTransform(rectTransform, anchorAndPivot, anchorAndPivot, anchorAndPivot, sizeDelta, anchoredPosition);
    }

    public static void SetupFillRectTransform(RectTransform rectTransform)
    {
        SetupRectTransform(rectTransform, AnchorPosition.BottomLeft, AnchorPosition.TopRight, AnchorPosition.Center, offsetMin: Vector2.zero, offsetMax: Vector2.zero);
    }

    public static void SetupTextMesh(TextMeshProUGUI textMeshPro, TMP_FontAsset font, int fontSize, Color textColor, string text, TextAlignmentOptions alignment = TextAlignmentOptions.Center)
    {
        textMeshPro.text = text;
        textMeshPro.font = font;
        textMeshPro.color = textColor;
        textMeshPro.fontSize = fontSize;
        textMeshPro.alignment = alignment;
        textMeshPro.raycastTarget = false;
    }

    public static GameObject SetupSpriteSwapButton(GameObject outer, string buttonName, Sprite defaultSprite, Sprite hoveredSprite, Sprite pressedSprite, Sprite selectedSprite, Sprite disabledSprite)
    {
        GameObject buttonObj = new GameObject($"{(outer != null ? outer.name : "ModConfigMenu")}_{buttonName}_Button");

        // Image displayed the default sprite when not hovered or clicked
        Image image = buttonObj.AddComponent<Image>();
        image.sprite = defaultSprite;
        image.rectTransform.sizeDelta = image.sprite.rect.size;

        SpriteState spriteState = new SpriteState();
        spriteState.highlightedSprite = hoveredSprite;
        spriteState.pressedSprite = pressedSprite;
        if (selectedSprite)
        {
            spriteState.selectedSprite = selectedSprite;
        }

        if (disabledSprite)
        {
            spriteState.disabledSprite = disabledSprite;
        }

        Button button = buttonObj.AddComponent<Button>();
        button.transition = Selectable.Transition.SpriteSwap;
        button.targetGraphic = image;
        button.spriteState = spriteState;

        return buttonObj;
    }

    public static GameObject SetupSpriteSwapButton(GameObject outer, string buttonName, Dictionary<ButtonState, Sprite> buttonSpriteMap)
    {
        return SetupSpriteSwapButton(outer, buttonName,
            buttonSpriteMap[ButtonState.Default],
            buttonSpriteMap[ButtonState.Hovered],
            buttonSpriteMap[ButtonState.Pressed],
            buttonSpriteMap.TryGetValue(ButtonState.Selected, out var selectedSprite) ? selectedSprite : null,
            buttonSpriteMap.TryGetValue(ButtonState.Disabled, out var disabledSprite) ? disabledSprite : null
        );
    }
}