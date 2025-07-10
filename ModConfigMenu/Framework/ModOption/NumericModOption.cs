using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using ModConfigMenu.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XeruUtils;

namespace ModConfigMenu.Framework.ModOption;

internal class NumericModOption<T> : SimpleModOption<T> where T : struct, IComparable, IConvertible, IFormattable
{
    private ModOptionTextField InputFieldWidget;
    private TextMeshProUGUI valueText;
    private Slider slider;
    private HorizontalLayoutGroup hBox;
    
    private static readonly HashSet<Type> FLOATING_POINT_TYPES = new HashSet<Type>(){typeof(float), typeof(double), typeof(decimal)};
    private static readonly int HBOX_SPACING = 5;

    public NumericModOption(BepInPlugin owner, ConfigEntry<T> configEntry)
        : base(owner, configEntry)
    {
        RegisterSprites();
        GetLatest();
    }

    public override GameObject GetUIGameObject()
    {
        AcceptableValueBase acceptableValue = ConfigEntry.Description.AcceptableValues;
        // If we have a valid defined range then we should display a slider and the text of the value next to it
        if (acceptableValue != null && acceptableValue is AcceptableValueRange<T> range)
        {
            return ConstructNumericModOptionWidget_Slider(range);
        }
        else
        {
            return ConstructNumericModOptionWidget_InputField();
        }
    }

    /// <summary>
    /// Constructs the NumericModOption widget as a text box since there is no specified acceptable range
    /// </summary>
    /// <returns>A game object to be rendered</returns>
    private GameObject ConstructNumericModOptionWidget_InputField()
    {
        InputFieldWidget = new ModOptionTextField(this);
        InputFieldWidget.InputField.contentType = FLOATING_POINT_TYPES.Contains(ConfigEntry.SettingType) ? TMP_InputField.ContentType.DecimalNumber : TMP_InputField.ContentType.IntegerNumber;
        InputFieldWidget.InputField.onEndEdit.AddListener(OnEndEdit);
        InputFieldWidget.SetText(FormatValue(CachedValue));
        UIHelpers.SetupRectTransform(InputFieldWidget.Widget.GetComponent<RectTransform>(), AnchorPosition.CenterLeft);
        return InputFieldWidget.Widget;
    }

    /// <summary>
    /// Constructs the NumericModOption widget as a slider since there is a specified acceptable range
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    private GameObject ConstructNumericModOptionWidget_Slider(AcceptableValueRange<T> range)
    {
        GameObject hboxObj = new GameObject($"${FieldId()}_value_hbox", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
            
        hBox = hboxObj.GetComponent<HorizontalLayoutGroup>();
            
        GameObject slider = GetSliderWidget(range.MinValue, range.MaxValue);
        slider.transform.SetParent(hboxObj.transform, false);
            
        GameObject valueTextObj = new GameObject($"{FieldId()}_value", typeof(RectTransform), typeof(TextMeshProUGUI));
        RectTransform valueTextRT = valueTextObj.GetComponent<RectTransform>();
        valueTextRT.SetParent(hboxObj.transform, false);
        valueText = valueTextObj.GetComponent<TextMeshProUGUI>();
        UIHelpers.SetupTextMesh(valueText, Plugin.THICK_PIXEL_8PT_FONT, Constants.PARAGRAPH_FONT_SIZE, Constants.BODY_FONT_COLOR, FormatValue(CachedValue));
            
        // After creating the text mesh we want to cache the max size we expect it to be (the size when the maximum value is entered).
        // This only works with mono-width fonts
        Vector2 maxValuePreferredTextSize = UIHelpers.GetTextSize(valueText, FormatValue(range.MaxValue));
            
        valueTextRT.sizeDelta = maxValuePreferredTextSize;
            
        hboxObj.GetComponent<RectTransform>().sizeDelta = new Vector2(0, Math.Max(slider.GetComponent<RectTransform>().sizeDelta.y, maxValuePreferredTextSize.y)); // we need to set the default 
            
        ContentSizeFitter hboxFitter = hboxObj.GetComponent<ContentSizeFitter>();
        hboxFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        hboxFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        UIHelpers.SetupRectTransform(hboxObj.GetComponent<RectTransform>(), AnchorPosition.CenterLeft);
        return hboxObj;
    }

    /// <summary>
    /// Constructs the slider widget and handle for this option.
    /// </summary>
    /// <param name="minValue">The minimum value of the slider</param>
    /// <param name="maxValue">The maximum value of the slider</param>
    /// <returns>A game object to be added to the widget for this option</returns>
    private GameObject GetSliderWidget(T minValue, T maxValue)
    {
        GameObject sliderObj = new GameObject($"{FieldId()}_value_slider", typeof(RectTransform), typeof(Slider), typeof(Image));
        Image sliderBg = sliderObj.GetComponent<Image>();
        sliderBg.sprite = Plugin.SpriteCache.Get(SpriteConstants.SLIDER_BAR_TEXTURE_FILENAME).DefaultSprite;
        
        // Create and size the handle
        GameObject handle = new GameObject($"{FieldId()}_value_slider_handle", typeof(RectTransform), typeof(Image), typeof(ContentSizeFitter));
        Image handleImage = handle.GetComponent<Image>();
        handleImage.sprite = Plugin.SpriteCache.Get(SpriteConstants.SLIDER_BUTTON_TEXTURE_FILENAME).ButtonSprites[ButtonState.Pressed];
        RectTransform handleRT = handle.GetComponent<RectTransform>();
        handleRT.SetParent(sliderObj.transform);
        UIHelpers.SetupRectTransform(handleRT, AnchorPosition.Center, sizeDelta: handleImage.sprite.rect.size);
        handle.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        //Update the padding on the hbox to prevent overflow
        int halfHandleWidth = (int)Math.Ceiling(handleImage.sprite.rect.width / 2);
        hBox.spacing = halfHandleWidth + HBOX_SPACING;
        hBox.padding = new RectOffset(halfHandleWidth, 0, 0, 0);
        
        slider = sliderObj.GetComponent<Slider>();
        slider.direction = Slider.Direction.LeftToRight;
        slider.wholeNumbers = typeof(T) == typeof(int) || typeof(T) == typeof(long);
        slider.targetGraphic = sliderBg;
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.minValue = minValue.ToSingle(null);
        slider.maxValue = maxValue.ToSingle(null);
        slider.value = CachedValue.ToSingle(null);
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        sliderObj.GetComponent<RectTransform>().sizeDelta = new Vector2(
            sliderBg.sprite.rect.width + handleImage.sprite.rect.width,
            Math.Max(sliderBg.sprite.rect.height, handleImage.sprite.rect.height)
        );

        return sliderObj;
    }
    
    // MOD OPTION VALUE CHANGE HANDLERS
    
    /// <summary>
    /// Handler for when the value of the slider is changed.
    ///  The handler only send a float so we need to make sure
    ///  to cast it to our SettingsType;
    /// </summary>
    /// <param name="value">The new float value for the slider</param>
    private void OnSliderValueChanged(float value)
    {
        CachedValue = (T)Convert.ChangeType(value, typeof(T));
        valueText.text = FormatValue(CachedValue);
    }
    
    /// <summary>
    /// Handler for when we have a text input field and we comple
    ///  and edit action. This will try to parse to a double
    ///  just in case we care about precision and then will
    ///  cast to the SettingsType. In the event the cast fails
    ///  we just reset the text to the cached value.
    ///
    /// NOTE: we can have some precision lost with very large
    ///  long input values. (But why would you need those)
    /// </summary>
    /// <param name="text">The new text in the field</param>
    private void OnEndEdit(string text)
    {
        if (double.TryParse(text, out var doubleValue))
        {
            CachedValue = (T)Convert.ChangeType(doubleValue, typeof(T));
        }
        else
        {
            InputFieldWidget.SetText(FormatValue(CachedValue));
        }
    }

    /// <summary>
    /// Gets the formatted string for the value. If the SettingType is a floating point value then we want to format to 2 decimals
    /// </summary>
    /// <param name="value">The value being formatted</param>
    /// <returns>A formatted string for the current value</returns>
    private string FormatValue(T value)
    {
        
        if (FLOATING_POINT_TYPES.Contains(typeof(T)))
        {
            return value.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
        }
        
        return value.ToString();
    }
    
    /// <summary>
    /// Registers the sprites required to display the slider bar
    /// </summary>
    private void RegisterSprites()
    {
        Plugin.SpriteCache.LoadButtonSpritesFromAssetTexture(SpriteConstants.SLIDER_BUTTON_TEXTURE_FILENAME, SpriteConstants.SLIDER_BUTTON_SPRITE_SLICES, Plugin.Logger);
        Plugin.SpriteCache.LoadSpriteFromAssetTexture(SpriteConstants.SLIDER_BAR_TEXTURE_FILENAME, SpriteConstants.SLIDER_BAR_SPRITE_SLICE, Plugin.Logger);
    }

    public override void PostReset()
    {
        base.PostReset();
        if (slider != null)
        {
            slider.value = CachedValue.ToSingle(null);
            valueText.text = FormatValue(CachedValue);
        } else if (InputFieldWidget != null)
        {
            InputFieldWidget.SetText(FormatValue(CachedValue));
        }
    }
}