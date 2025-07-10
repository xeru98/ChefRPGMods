using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;
using XeruUtils;

namespace ModConfigMenu.Framework.ModOption;

internal class ToggleModOption : SimpleModOption<bool>
{

    private Toggle toggle;
    
    public ToggleModOption(BepInPlugin owner, ConfigEntry<bool> entry)
        : base(owner, entry)
    {
        RegisterSprites();
    }
    
    public override GameObject GetUIGameObject()
    {
        Dictionary<ButtonState, Sprite> toggleSprites = Plugin.SpriteCache.Get(SpriteConstants.TOGGLE_BUTTON_TEXTURE_FILENAME).ButtonSprites;
        
        GameObject toggleObj = new GameObject($"{FieldId()}_Toggle", typeof(RectTransform), typeof(Toggle));
        
        GameObject togglebg = new GameObject($"{FieldId()}_Toggle_Background", typeof(RectTransform), typeof(Image));
        togglebg.transform.SetParent(toggleObj.transform, false);
        togglebg.GetComponent<Image>().sprite = toggleSprites[ButtonState.Default];
        UIHelpers.SetupFillRectTransform(togglebg.GetComponent<RectTransform>());
        
        GameObject toggleCheck = new GameObject($"{FieldId()}_Toggle_Check", typeof(RectTransform), typeof(Image));
        toggleCheck.transform.SetParent(togglebg.transform, false);
        toggleCheck.GetComponent<Image>().sprite = toggleSprites[ButtonState.Selected];
        UIHelpers.SetupFillRectTransform(toggleCheck.GetComponent<RectTransform>());
        
        toggle = toggleObj.GetComponent<Toggle>();
        toggle.transition = Selectable.Transition.SpriteSwap;
        toggle.targetGraphic = togglebg.GetComponent<Image>();
        toggle.graphic = toggleCheck.GetComponent<Image>();
        toggle.isOn = CachedValue;
        toggle.onValueChanged.AddListener(value =>
        {
            CachedValue = value;
            toggle.isOn = value;
        });
        
        SpriteState spriteState = new SpriteState();
        spriteState.highlightedSprite = toggleSprites[ButtonState.Hovered];
        spriteState.pressedSprite = toggleSprites[ButtonState.Pressed];
        toggle.spriteState = spriteState;
        
        RectTransform toggleRT = toggleObj.GetComponent<RectTransform>();
        UIHelpers.SetupRectTransform(toggleRT, AnchorPosition.Center, togglebg.GetComponent<Image>().sprite.rect.size);
        toggleObj.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

        UIHelpers.SetupRectTransform(toggleObj.GetComponent<RectTransform>(), AnchorPosition.CenterLeft);

        return toggleObj;
    }

    public override void PostReset()
    {
        base.PostReset();
        toggle.isOn = CachedValue;
    }

    private void RegisterSprites()
    {
        Plugin.SpriteCache.LoadButtonSpritesFromAssetTexture(SpriteConstants.TOGGLE_BUTTON_TEXTURE_FILENAME, SpriteConstants.TOGGLE_BUTTON_SPRITE_SLICES, Plugin.Logger);
    }
}