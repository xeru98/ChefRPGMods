using System;
using UnityEngine;
using UnityEngine.UI;
using XeruUtils;

namespace ModConfigMenu.Framework.ModOption;

internal class NumericModOption<T> : SimpleModOption<T> where T : struct, IComparable, IConvertible
{
    private readonly Func<T, string> FormatValueImpl;
    
    private T? MinValue { get; }
    private T? MaxValue { get; }
    private T? Interval { get; }

    private Slider slider;

    public NumericModOption(string fieldId, Func<string> name, Func<string> tooltip, ModConfig owner, Func<T> getValue, Action<T> setValue, T? min, T? max, T? interval, Func<T, string> formatValue)
        : base(fieldId, name, tooltip, owner, getValue, setValue)
    {
        MinValue = min;
        MaxValue = max;
        Interval = interval;
        FormatValueImpl = formatValue;
        
        RegisterSprites();
    }

    public string FormatValue(T value)
    {
        return FormatValueImpl?.Invoke(Value)
            ?? Value.ToString();
    }

    public override GameObject GetUIGameObject()
    {
        GameObject gameObject = new GameObject($"${FieldId}_value_hbox", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        
        // If we have a valid defined range then we should display a slider
        if (MinValue != null && MaxValue != null && MaxValue.Value.CompareTo(MinValue.Value) < 0)
        {
            GameObject slider = GetSliderGameObject();
            slider.transform.SetParent(gameObject.transform);
        }
        // We should always display a text input field
        //GameObject textField = GetTextFieldGameObject();
        //textField.transform.SetParent(gameObject.transform);
        
        return gameObject;
    }

    private GameObject GetSliderGameObject()
    {
        GameObject sliderObj = new GameObject($"{FieldId}_value_slider", typeof(RectTransform), typeof(Slider), typeof(Image));
        Image sliderBg = sliderObj.GetComponent<Image>();
        sliderBg.sprite = Plugin.SpriteCache.Get(SpriteConstants.SLIDER_BAR_TEXTURE_FILENAME).DefaultSprite;
        
        GameObject handle = new GameObject($"{FieldId}_value_slider_handle", typeof(RectTransform), typeof(Image));
        handle.transform.SetParent(sliderObj.transform);
        UIHelpers.SetupFillRectTransform(handle.GetComponent<RectTransform>());
        handle.GetComponent<Image>().sprite = Plugin.SpriteCache.Get(SpriteConstants.SLIDER_BUTTON_TEXTURE_FILENAME).ButtonSprites[ButtonState.Pressed];
        
        Slider slider = sliderObj.GetComponent<Slider>();
        slider.direction = Slider.Direction.LeftToRight;
        slider.wholeNumbers = typeof(T) == typeof(int);
        slider.targetGraphic = sliderBg;
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.minValue = MinValue.Value.ToSingle(null);
        slider.maxValue = MaxValue.Value.ToSingle(null);
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        return sliderObj;
    }

    private GameObject GetTextFieldGameObject()
    {
        return null;
    }

    private void OnTextValueChanged(T value)
    {
        if (MinValue != null)
        {
            value = value.CompareTo(MinValue.Value) < 0 ? MinValue.Value : value;
        }

        if (MaxValue != null)
        {
            value = value.CompareTo(MaxValue.Value) > 0 ? MaxValue.Value : value;
        }
    }

    private void OnSliderValueChanged(float value)
    {
        
    }

    private void RegisterSprites()
    {
        Plugin.SpriteCache.LoadButtonSpritesFromAssetTexture(SpriteConstants.SLIDER_BUTTON_TEXTURE_FILENAME, SpriteConstants.SLIDER_BUTTON_SPRITE_SLICES, Plugin.Logger);
        Plugin.SpriteCache.LoadSpriteFromAssetTexture(SpriteConstants.SLIDER_BAR_TEXTURE_FILENAME, SpriteConstants.SLIDER_BAR_SPRITE_SLICE, Plugin.Logger);
    }
}