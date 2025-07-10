using System.Collections;
using ModConfigMenu.Framework;
using ModConfigMenu.Framework.ModOption;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XeruUtils;
using Constants = ModConfigMenu.Framework.Constants;

namespace ModConfigMenu.Components;

public class ModOptionTextField
{
    
    public TMP_InputField InputField { get; private set; }
    public GameObject Widget { get; private set; }

    private static readonly Vector2 TEXT_AREA_OFFSET = new Vector2(3, -1);
    private static readonly Vector2 TEXT_OFFSET = new Vector2(1, 0);
    
    public ModOptionTextField(BaseModOption outer)
    {
        RegisterSprites();
        
        // Create the outer. This will display the background image
        Widget = UIHelpers.CreateDisabled($"{outer.FieldId()}_InputField", typeof(RectTransform), typeof(TMP_InputField));
        
        // Set up the background image. This fixes an issue where we try to double create the canvas renderer
        GameObject backgroundObj = UIHelpers.CreateDisabled($"{outer.FieldId()}_Background", typeof(RectTransform), typeof(Image));
        RectTransform backgroundRT = backgroundObj.GetComponent<RectTransform>();
        backgroundRT.SetParent(Widget.transform, false);
        UIHelpers.SetupFillRectTransform(backgroundRT);
        Image background = backgroundObj.GetComponent<Image>();
        background.sprite = Plugin.SpriteCache.Get(SpriteConstants.INPUT_FIELD_BACKGROUND_TEXTURE_FILENAME).DefaultSprite;
        
        // Set the size of the widget to match the background image
        RectTransform widgetRT = Widget.GetComponent<RectTransform>();
        widgetRT.sizeDelta = background.sprite.rect.size;
        
        // The text area defines the area actually being typed in
        GameObject textArea = UIHelpers.CreateDisabled($"{outer.FieldId()}_TextArea", typeof(RectTransform), typeof(RectMask2D), typeof(Image));
        RectTransform textAreaRT = textArea.GetComponent<RectTransform>();
        textAreaRT.SetParent(Widget.transform, false);
        UIHelpers.SetupRectTransform(textAreaRT, AnchorPosition.BottomLeft, AnchorPosition.TopRight, AnchorPosition.Center, offsetMin: TEXT_AREA_OFFSET, offsetMax: TEXT_AREA_OFFSET);
        Image textAreaImage = textArea.GetComponent<Image>();
        textAreaImage.color = new Color(1, 1, 1, 0);
        textAreaImage.raycastTarget = false;

        // The text game object is where we define the font that's going to be used
        GameObject textObj = UIHelpers.CreateDisabled($"{outer.FieldId()}_Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(textArea.transform, false);
        UIHelpers.SetupRectTransform(textAreaRT, AnchorPosition.BottomLeft, AnchorPosition.TopRight, AnchorPosition.Center, offsetMin: TEXT_OFFSET, offsetMax: TEXT_AREA_OFFSET);
        TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
        UIHelpers.SetupTextMesh(text, Plugin.PIXEL_FONT_7PX_FONT, Constants.PARAGRAPH_FONT_SIZE, Constants.INPUT_FIELD_TEXT_COLOR, "", TextAlignmentOptions.Left);
        
        // The text area will not function without a placeholder
        GameObject placeholderObj = UIHelpers.CreateDisabled($"{outer.FieldId()}_Placeholder", typeof(RectTransform), typeof(TextMeshProUGUI));
        placeholderObj.transform.SetParent(textArea.transform, false);
        UIHelpers.SetupRectTransform(textAreaRT, AnchorPosition.BottomLeft, AnchorPosition.TopRight, AnchorPosition.Center, offsetMin: TEXT_OFFSET, offsetMax: TEXT_AREA_OFFSET);
        TextMeshProUGUI placeholder = placeholderObj.GetComponent<TextMeshProUGUI>();
        UIHelpers.SetupTextMesh(placeholder, Plugin.PIXEL_FONT_7PX_FONT, Constants.PARAGRAPH_FONT_SIZE, Constants.INPUT_FIELD_TEXT_COLOR, "Enter Value...", TextAlignmentOptions.Left);
        placeholder.fontStyle = FontStyles.Italic;
        
        // Last we link up the relevant components so we can use this as a single object
        InputField = Widget.GetComponent<TMP_InputField>();
        InputField.textViewport = textAreaRT;
        InputField.textComponent = text;
        InputField.placeholder = placeholder;
        InputField.caretColor = Constants.INPUT_FIELD_TEXT_COLOR;
        InputField.caretBlinkRate = 0.85f;
        InputField.customCaretColor = true;
        InputField.selectionColor = Constants.INPUT_FIELD_SELECTION_COLOR;
        InputField.lineType = TMP_InputField.LineType.SingleLine;
        InputField.interactable = true;
        InputField.targetGraphic = background;
        InputField.characterLimit = 16;
        
        UIHelpers.ActivateWithChildren(Widget);
    }
    
    public void SetText(string text)
    {
        InputField.text = text;
    }

    /// <summary>
    /// Loads the Input Field Background Texture if not already loaded
    /// </summary>
    private void RegisterSprites()
    {
        Plugin.SpriteCache.LoadSpriteFromAssetTexture(
            SpriteConstants.INPUT_FIELD_BACKGROUND_TEXTURE_FILENAME,
            new SpriteSheetSlice(Vector2.zero, SpriteConstants.INPUT_FIELD_BACKGROUND_SPRITE_SIZE, SpriteConstants.SPRITE_PPU),
            Plugin.Logger
        );
    }
}