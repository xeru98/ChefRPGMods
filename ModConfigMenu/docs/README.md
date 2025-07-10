# Mod Config Menu

**Version:** 1.0.0  
**Author:** Xeru98  
**BepInEx Plugin ID:** `com.xeru98.chefrpg.modconfigmenu`

A powerful and user-friendly configuration menu system for Chef RPG mods. This plugin provides an intuitive UI for managing mod settings directly from the game's main menu.

## 🎯 Features

- **Automatic Configuration Detection** - Automatically discovers and displays configurations from all installed BepInEx plugins
- **Rich UI Controls** - Supports boolean toggles, numeric sliders, text inputs, and dropdown selections
- **Seamless Integration** - Adds a "Mod Settings" button to the main menu
- **Developer Friendly** - Simple API for plugin developers to create custom configuration UIs
- **Type Safety** - Supports all common configuration types with proper validation

## 📦 Installation

### For Users

1. **Prerequisites:**
    - [BepInEx 5.x](https://github.com/BepInEx/BepInEx/releases) installed
    - Chef RPG game

2. **Installation Steps:**
    1. Download the latest release from the [releases page](https://github.com/your-repo/releases)
    2. Extract the contents to your `BepInEx/plugins/` folder
    3. The folder structure should look like:
       ```
       BepInEx/
       └── plugins/
           └── ModConfigMenu/
               ├── ModConfigMenu.dll
               └── Assets/
                   └── (sprite files)
       ```

3. **Usage:**
    - Launch Chef RPG
    - From the main menu, click the "Mod Settings" button
    - Configure your installed mods using the intuitive UI

## 🔧 Configuration Types Supported

The plugin automatically detects and creates appropriate UI controls for:

- **Boolean** - Toggle switches
- **Integer** - Text input or sliders (with range)
- **Float** - Text input or sliders (with range)
- **String** - Text input or selection (with options)

## 📖 Developer Documentation

See the [Developer Guide](./developer-guide.md) for detailed instructions on integrating your plugin with Mod Config Menu.

## 🐛 Troubleshooting

### Common Issues

**"Mod Settings button not appearing"**
- Ensure BepInEx is properly installed
- Check the BepInEx console for error messages
- Verify the plugin is in the correct folder

**"I can't see a mod I have installed in the menu"**
- Ensure that the mod actually has configurations. Not all mods have them
and those that don't will not show up.

## 📄 License

This project is licensed under the MIT License for everything except the ASSETS. The
assets are from the ChefRPG files and may not be reused without explicit permission
from PixelArtist (the developer of ChefRPG)

## 🙏 Acknowledgments

- BepInEx team for the excellent modding framework
- Unity Technologies for the UI system
- Chef RPG community for feedback and testing