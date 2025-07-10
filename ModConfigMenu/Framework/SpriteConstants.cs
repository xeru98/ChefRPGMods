using System.Collections.Generic;
using UnityEngine;
using XeruUtils;

namespace ModConfigMenu.Framework;

public class SpriteConstants
{
    public static readonly int SPRITE_PPU = 24;
    
    public static readonly string COLLECTIONS_BUTTON_TEXTURE_FILENAME = "Collections_Button.png";
    public static readonly Vector2 COLLECTIONS_BUTTON_SPRITE_SIZE = new Vector2(25, 26);
    public static readonly List<SpriteSheetSliceWithId<ButtonState>> COLLECTIONS_BUTTON_SPRITE_SLICES = new List<SpriteSheetSliceWithId<ButtonState>>()
    {
        new (ButtonState.Default, new Vector2(84, 0), COLLECTIONS_BUTTON_SPRITE_SIZE, SPRITE_PPU),
        new (ButtonState.Hovered, new Vector2(56, 0), COLLECTIONS_BUTTON_SPRITE_SIZE, SPRITE_PPU),
        new (ButtonState.Pressed, new Vector2(28, 0), COLLECTIONS_BUTTON_SPRITE_SIZE, SPRITE_PPU),
        new (ButtonState.Selected, new Vector2(0, 0), COLLECTIONS_BUTTON_SPRITE_SIZE, SPRITE_PPU)
    };
    
    public static readonly string MAIN_MENU_BUTTON_TEXTURE_FILENAME = "77x16_Buttons.png";
    public static readonly string SAVE_BUTTON_TEXTURE_FILENAME = "Save_Buttons.png";
    public static readonly string RESET_BUTTON_TEXTURE_FILENAME = "Reset_Buttons.png";
    public static readonly Vector2 BUTTON_77x16_SPRITE_SIZE = new Vector2(77, 16);

    public static readonly List<SpriteSheetSliceWithId<ButtonState>> MAIN_MENU_BUTTON_SPRITE_SLICES = new List<SpriteSheetSliceWithId<ButtonState>>()
    {
        new(ButtonState.Default, new Vector2(0, 54), BUTTON_77x16_SPRITE_SIZE, SPRITE_PPU),
        new(ButtonState.Hovered, new Vector2(0, 36), BUTTON_77x16_SPRITE_SIZE, SPRITE_PPU),
        new(ButtonState.Pressed, new Vector2(0, 18), BUTTON_77x16_SPRITE_SIZE, SPRITE_PPU),
        new(ButtonState.Selected, new Vector2(0, 0), BUTTON_77x16_SPRITE_SIZE, SPRITE_PPU)
    };
    
    public static readonly List<SpriteSheetSliceWithId<ButtonState>> SAVE_RESET_BUTTON_SPRITE_SLICES = new List<SpriteSheetSliceWithId<ButtonState>>()
    {
        new(ButtonState.Disabled, new Vector2(0, 54), BUTTON_77x16_SPRITE_SIZE, SPRITE_PPU),
        new(ButtonState.Hovered, new Vector2(0, 36), BUTTON_77x16_SPRITE_SIZE, SPRITE_PPU),
        new(ButtonState.Pressed, new Vector2(0, 18), BUTTON_77x16_SPRITE_SIZE, SPRITE_PPU),
        new(ButtonState.Default, new Vector2(0, 0), BUTTON_77x16_SPRITE_SIZE, SPRITE_PPU)
    };
        
    
    public static readonly string TOGGLE_BUTTON_TEXTURE_FILENAME = "Toggle_Buttons.png";
    public static readonly Vector2 TOGGLE_BUTTON_SPRITE_SIZE = new Vector2(30, 19);
    public static readonly List<SpriteSheetSliceWithId<ButtonState>> TOGGLE_BUTTON_SPRITE_SLICES = new List<SpriteSheetSliceWithId<ButtonState>>()
    {
        new (ButtonState.Default, new Vector2(0, 63), TOGGLE_BUTTON_SPRITE_SIZE, SPRITE_PPU),
        new (ButtonState.Hovered, new Vector2(0, 42), TOGGLE_BUTTON_SPRITE_SIZE, SPRITE_PPU),
        new (ButtonState.Pressed, new Vector2(0, 21), TOGGLE_BUTTON_SPRITE_SIZE, SPRITE_PPU),
        new (ButtonState.Selected, new Vector2(0, 0), TOGGLE_BUTTON_SPRITE_SIZE, SPRITE_PPU)
    };
    
    public static readonly string SLIDER_BUTTON_TEXTURE_FILENAME = "Slider_Buttons.png";
    public static readonly Vector2 SLIDER_BUTTON_SPRITE_SIZE = new Vector2(15, 15);
    public static readonly List<SpriteSheetSliceWithId<ButtonState>> SLIDER_BUTTON_SPRITE_SLICES = new List<SpriteSheetSliceWithId<ButtonState>>()
    {
        new(ButtonState.Default, new Vector2(0, 0), SLIDER_BUTTON_SPRITE_SIZE, SPRITE_PPU),
        new(ButtonState.Hovered, new Vector2(18, 0), SLIDER_BUTTON_SPRITE_SIZE, SPRITE_PPU),
        new(ButtonState.Pressed, new Vector2(36, 0), SLIDER_BUTTON_SPRITE_SIZE, SPRITE_PPU)
    };
    
    public static readonly string SLIDER_BAR_TEXTURE_FILENAME = "Slider_Background.png";
    public static readonly SpriteSheetSlice SLIDER_BAR_SPRITE_SLICE = new SpriteSheetSlice(Vector2.zero, new Vector2(85, 13), SPRITE_PPU);

    public static readonly List<string> FRONT_MENU_BUTTON_NAMES_TO_MOVE = new List<string>()
    {
        "Credits Button",
        "Quit Game Button"
    };

    public static readonly string MAIN_MENU_UI_PANEL_TEXTURE_FILENAME = "Main_Menu_UI_Panel.png";
    public static readonly SpriteSheetSlice MAIN_MENU_UI_PANEL_SPRITE_SLICE = new SpriteSheetSlice(Vector2.zero, new Vector2(366, 241), SPRITE_PPU);
    
    public static readonly string CLOSE_BUTTON_TEXTURE_FILENAME = "Close_Menu_Button.png";
    public static readonly Vector2 CLOSE_BUTTON_SPRITE_SIZE = new Vector2(30, 19);
    public static readonly List<SpriteSheetSliceWithId<ButtonState>> CLOSE_BUTTON_SPRITE_SLICES = new List<SpriteSheetSliceWithId<ButtonState>>()
    {
        new(ButtonState.Default, new Vector2(0, 105), CLOSE_BUTTON_SPRITE_SIZE, SPRITE_PPU),
        new(ButtonState.Hovered, new Vector2(0, 84), CLOSE_BUTTON_SPRITE_SIZE, SPRITE_PPU),
        new(ButtonState.Pressed, new Vector2(0, 63), CLOSE_BUTTON_SPRITE_SIZE, SPRITE_PPU)
    };
    public static readonly string CLOSE_BUTTON_SPRITE_CACHE_KEY = $"{CLOSE_BUTTON_TEXTURE_FILENAME}_CLOSE";
    
    public static readonly string BACK_BUTTON_TEXTURE_FILENAME = CLOSE_BUTTON_TEXTURE_FILENAME;
    public static readonly Vector2 BACK_BUTTON_SPRITE_SIZE = CLOSE_BUTTON_SPRITE_SIZE;
    public static readonly List<SpriteSheetSliceWithId<ButtonState>> BACK_BUTTON_SPRITE_SLICES = new List<SpriteSheetSliceWithId<ButtonState>>()
    {
        new(ButtonState.Default, new Vector2(0, 42), BACK_BUTTON_SPRITE_SIZE, SPRITE_PPU),
        new(ButtonState.Hovered, new Vector2(0, 21), BACK_BUTTON_SPRITE_SIZE, SPRITE_PPU),
        new(ButtonState.Pressed, new Vector2(0, 0), BACK_BUTTON_SPRITE_SIZE, SPRITE_PPU)
    };
    public static readonly string BACK_BUTTON_SPRITE_CACHE_KEY = $"{BACK_BUTTON_TEXTURE_FILENAME}_BACK";
    
    public static readonly string SCROLLBAR_HANDLE_TEXTURE_FILENAME = "Scrollbar_Background.png";
    public static readonly Vector2 SCROLLBAR_HANDLE_SPRITE_SIZE = new Vector2(14, 13);
    public static readonly Vector4 SCROLLBAR_HANDLE_BORDER = new Vector4(6, 6, 6, 6);

    public static readonly List<SpriteSheetSliceWithId<ButtonState>> SCROLLBAR_HANDLE_SPRITE_SLICES = new List<SpriteSheetSliceWithId<ButtonState>>()
    {
        new(ButtonState.Default, new Vector2(0, 0), SCROLLBAR_HANDLE_SPRITE_SIZE, SPRITE_PPU, SCROLLBAR_HANDLE_BORDER),
        new(ButtonState.Hovered, new Vector2(16, 0), SCROLLBAR_HANDLE_SPRITE_SIZE, SPRITE_PPU, SCROLLBAR_HANDLE_BORDER),
        new(ButtonState.Pressed, new Vector2(32, 0), SCROLLBAR_HANDLE_SPRITE_SIZE, SPRITE_PPU, SCROLLBAR_HANDLE_BORDER)
    };
    
    public static readonly string INPUT_FIELD_BACKGROUND_TEXTURE_FILENAME = "Input_Field_Background.png";
    public static readonly Vector2 INPUT_FIELD_BACKGROUND_SPRITE_SIZE = new Vector2(116, 26);
    
    public static readonly string ARROW_BUTTON_TEXTURE_FILENAME = "15x18_Arrow_Buttons.png";
    public static readonly Vector2 ARROW_BUTTON_SPRITE_SIZE = new Vector2(15, 18);
    public static readonly List<SpriteSheetSliceWithId<ButtonState>> ARROW_BUTTON_SPRITE_SLICES = new List<SpriteSheetSliceWithId<ButtonState>>()
    {
        new(ButtonState.Default, new Vector2(0, 63), ARROW_BUTTON_SPRITE_SIZE, SPRITE_PPU),
        new(ButtonState.Hovered, new Vector2(0, 42), ARROW_BUTTON_SPRITE_SIZE, SPRITE_PPU),
        new(ButtonState.Pressed, new Vector2(0, 21), ARROW_BUTTON_SPRITE_SIZE, SPRITE_PPU)
    };
}