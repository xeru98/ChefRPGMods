using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using ModConfigMenu.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XeruUtils;

namespace ModConfigMenu.Framework.ModOption;

internal class StringModOption : SimpleModOption<string>
{
    private ModOptionTextField InputFieldWidget;
    private LinkedList<string> Options = new LinkedList<string>();
    private TextMeshProUGUI CurrentOptionText;
    private LinkedListNode<string> currentOption;
    
    public StringModOption(BepInPlugin owner, ConfigEntry<string> configEntry) 
        : base(owner, configEntry)
    {
        RegisterSprites();
        GetLatest();
    }

    public override GameObject GetUIGameObject()
    {
        AcceptableValueBase acceptableValue = ConfigEntry.Description.AcceptableValues;
        // If we have a valid list of options then we should display option menu instead of a text box
        if (acceptableValue != null && acceptableValue is AcceptableValueList<string> options)
        {
            return ConstructStringModOptionWidget_Options(options);
        }
        else
        {
            return ConstructStringModOptionWidget_InputField();
        }
    }

    private GameObject ConstructStringModOptionWidget_InputField()
    {
        InputFieldWidget = new ModOptionTextField(this);
        InputFieldWidget.InputField.contentType = TMP_InputField.ContentType.Standard;
        InputFieldWidget.InputField.onEndEdit.AddListener(OnEndEdit);
        InputFieldWidget.SetText(CachedValue);
        UIHelpers.SetupRectTransform(InputFieldWidget.Widget.GetComponent<RectTransform>(), AnchorPosition.CenterLeft);
        return InputFieldWidget.Widget;
    }

    private GameObject ConstructStringModOptionWidget_Options(AcceptableValueList<string> options)
    {
        // copy the options into a linked list
        foreach (string value in options.AcceptableValues)
        {
            LinkedListNode<string> node = Options.AddLast(value);
            if (CachedValue.Equals(value))
            {
                this.currentOption = node;
            }
        }
        
        // safety check to make sure we have a valid option. Otherwise we just use the default
        if (this.currentOption == null)
        {
            Plugin.Logger.LogError($"Could not find value \"{CachedValue}\" in options [{String.Join("\", \"", options.AcceptableValues)}]. Defaulting to first valid option");
            this.currentOption = Options.First;
            CachedValue = this.currentOption.Value;
        }
        
        // Create the container for the buttons and the current option
        GameObject Container = new GameObject($"{FieldId()}_Hbox", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        HorizontalLayoutGroup hBox = Container.GetComponent<HorizontalLayoutGroup>();
        hBox.childAlignment = TextAnchor.MiddleLeft;
        hBox.childControlHeight = hBox.childControlWidth = true;
        hBox.childForceExpandHeight = hBox.childForceExpandWidth = false;
        hBox.spacing = 5;
        
        // CREATE THE BUTTONS
        
        // Previous option button (sprite needs to be flipped)
        Material flipMaterial = new Material(Shader.Find("UI/Default"));
        flipMaterial.mainTextureScale = new Vector2(-1, 1);
        GameObject prevButton = UIHelpers.SetupSpriteSwapButton(Container, "Prev", Plugin.SpriteCache.Get(SpriteConstants.ARROW_BUTTON_TEXTURE_FILENAME).ButtonSprites);
        Image prevButtonImage = prevButton.GetComponent<Image>();
        prevButtonImage.material = flipMaterial;
        LayoutElement prevLE = prevButton.AddComponent<LayoutElement>();
        prevLE.preferredWidth = prevButtonImage.sprite.rect.width;
        prevLE.preferredHeight = prevButtonImage.sprite.rect.height;
        prevLE.flexibleWidth = 0;
        prevButton.transform.SetParent(Container.transform, false);
        prevButton.GetComponent<Button>().onClick.AddListener(OnPrevButtonClick);
        
        // Current option
        GameObject currentOption = new GameObject($"{FieldId()}_CurrentOption", typeof(RectTransform), typeof(LayoutElement), typeof(TextMeshProUGUI));
        CurrentOptionText = currentOption.GetComponent<TextMeshProUGUI>();
        UIHelpers.SetupTextMesh(CurrentOptionText, Plugin.THICK_PIXEL_8PT_FONT, Constants.PARAGRAPH_FONT_SIZE, Constants.BODY_FONT_COLOR, CachedValue);
        LayoutElement currentOptionLE = currentOption.GetComponent<LayoutElement>();
        currentOptionLE.preferredHeight = CurrentOptionText.GetPreferredValues().y;
        currentOptionLE.flexibleWidth = 1;
        currentOption.transform.SetParent(Container.transform, false);
        
        
        // Next option button
        GameObject nextButton = UIHelpers.SetupSpriteSwapButton(Container, "Next", Plugin.SpriteCache.Get(SpriteConstants.ARROW_BUTTON_TEXTURE_FILENAME).ButtonSprites);
        Vector2 nextButtonSize = nextButton.GetComponent<Image>().sprite.rect.size;
        LayoutElement nextLE = nextButton.AddComponent<LayoutElement>();
        nextLE.preferredWidth = nextButtonSize.x;
        nextLE.preferredHeight = nextButtonSize.y;
        nextLE.flexibleWidth = 0;
        nextButton.transform.SetParent(Container.transform, false);
        nextButton.GetComponent<Button>().onClick.AddListener(OnNextButtonClick);
        
        // Last we size the container to fill horizontally but only be as tall as the sprites or the text
        float containerHeight = Mathf.Max(prevButtonImage.sprite.rect.height, UIHelpers.GetTextSize(CurrentOptionText, "aA").y);
        UIHelpers.SetupRectTransform(Container.GetComponent<RectTransform>(), AnchorPosition.CenterLeft, AnchorPosition.CenterRight, AnchorPosition.Center, sizeDelta: new Vector2(0, containerHeight));
        return Container;
    }
    
    /// <summary>
    /// Handler for when we have a text input field and we complete
    ///  an edit action. This will try to parse to a double
    ///  just in case we care about precision and then will
    ///  cast to the SettingsType. In the event the cast fails
    ///  we just reset the text to the cached value.
    /// </summary>
    /// <param name="text">The new text in the field</param>
    private void OnEndEdit(string text)
    {
        CachedValue = text;
    }
    
    private void RegisterSprites()
    {
        Plugin.SpriteCache.LoadButtonSpritesFromAssetTexture(SpriteConstants.ARROW_BUTTON_TEXTURE_FILENAME, SpriteConstants.ARROW_BUTTON_SPRITE_SLICES, Plugin.Logger);
    }

    private void OnPrevButtonClick()
    {
        if (Options.First == currentOption)
        {
            currentOption = Options.Last;
        }
        else
        {
            currentOption = currentOption.Previous;
        }
        CachedValue = currentOption.Value;
        SetCurrentOptionText(CachedValue);
    }

    private void OnNextButtonClick()
    {
        if (Options.Last == currentOption)
        {
            currentOption = Options.First;
        }
        else
        {
            currentOption = currentOption.Next;
        }
        CachedValue = currentOption.Value;
        SetCurrentOptionText(CachedValue);
    }

    private void SetCurrentOptionText(string text)
    {
        CurrentOptionText.text = text;
    }
    
    public override void PostReset()
    {
        base.PostReset();
        if (InputFieldWidget != null)
        {
            InputFieldWidget.SetText(CachedValue);
        } else if (CurrentOptionText != null)
        {
            SetCurrentOptionText(CachedValue);
        }
    }
}