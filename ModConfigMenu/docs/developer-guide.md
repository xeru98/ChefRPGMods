# Developer Guide - Mod Config Menu (v1.0.0)

This guide will help you integrate your BepInEx plugin with the Mod Config Menu system.

## 🚀 Quick Start

The Mod Config Menu automatically detects and displays configurations from any BepInEx plugin that uses the standard `ConfigEntry<T>` system. No additional setup is required for the following types:
- **bool**
- **Integers *(int, long)*** - Supports ranges
- **Decimals *(float, double, decimal)*** - Supports ranges
- **string** - Supports Lists

### Basic Configuration Example
```csharp
class Plugin : BaseUnityPlugin 
{
    private bool myBoolSetting = false;
    
    // Do all of your configuration in the constructor NOT THE AWAKE FUNCTION
    public Plugin()
    {
        // 1. Bind your config entry
        ConfigEntry<bool> boolEntry = Config.Bind<bool>(new ConfigDefinition("", "Bool Value"), true); // sets default to true
        
        // 2. Set up a change listener
        boolEntry.SettingChanged += OnMyBoolChanged;
        
        // 3. Initialize your setting using the current value
        myBoolSetting = boolEntry.value; 
    }
    
    // The changel handler (The function signature is really important). 
    private OnMyBoolChanged(object sender, EventArgs e) 
    {
        // Cast the sender back to the expected ConfigEntry type
        ConfigEntry<bool> boolEntry = (ConfigEntry<bool>)sender;
        // Set your internal bool to that update value
        myBoolSetting = boolEntry.value;
        // do whatever else you want to do on value change
    }
}
```

## In Depth Description
### Binding your config entry
Binding a new config entry should be done in the constructor of your
`BaseUnityPlugin` subclass. This is because the mod config menu does
all of the registering the first time the menu is created. It is
theoretically possible to have an `Awake()` function that is so complex
that not all of the settings are registered by this time, so it's safest
to just do all of the registration in the ctor.

#### Simple Format
All entries are made up of 2 parts, the definition and the description. The 
description is optional but the definition is always required. Here is a basic example:
```csharp
ConfigEntry<type> entry = Config.Bind<type>(new ConfigDefinition("<section name>", "<setting name>", <default value>))
```

#### Entry Description
There is a plan to add a menu with a tooltip when you hover a setting. But it
has not been implemented yet. However, you can prepare for this support by adding
a ConfigDescription to your entry.
```csharp
ConfigDescription desc = new ConfigDescription("<description>")
Config.Bind<type>(new ConfigDefinition("<section name>", "<setting name>", <default value>), desc)
```

#### Acceptable Values
If you notice in the types above some types support either ranges or lists by default
For entries that support lists you can use the following:
```csharp
AcceptableValueList<type> values = new AcceptableValueList<type>(<option 1>, <option 2>, <option 3>))
ConfigDescription desc = new ConfigDescription("<description>", values)
Config.Bind<type>(new ConfigDefinition("<section name>", "<setting name>", <default value>), desc)
```
For these types, not adding a list of values will cause the setting to display as a text box.
![](./img/textbox.png)
And adding a list will cause the setting to display with buttons just like the vanilla game
![](./img/list.png)

Some other settings instead support ranges of values. These can be declared like so
```csharp
AcceptableValuerange<type> values = new AcceptableValueList<type>(<min value>, <max value>))
ConfigDescription desc = new ConfigDescription("<description>", values)
Config.Bind<type>(new ConfigDefinition("<section name>", "<setting name>", <default value>), desc)
```
When not defined the setting will display like a text box (see above) and when
defined will display like a slider
![](./img/slider.png)

### Listening for changes
When a setting is updated in the mod menu, the value isn't changed until the
user clicks the save button. This is to allow them to quickly undo their
previous changes. When they do click save, as a developer you want to update
your plugin based on those saved changes. To do this, we add an event handler
after you bind using the returned ConfigEntry.
```csharp
// in ctor
{
    ConfigEntry<type> entry = new ConfigEntry<type>(...);
    entry.SettingChanged += OnSettingChanged;
}

private void OnSettingChanged(object sender, EventArgs e) 
{
    // Get your new value
    ConfigEntry<type> entry = (ConfigEntry<type>)sender;
    type value = entry.Value;
    // do other stuff here
}
```
I recommend having a separate handler for each entry, but there's nothing
stopping you from reusing them.

## Advanced: Custom Configuration Types

For advanced users, you can extend the Mod Config Menu to support custom configuration types by creating your own UI controls. This involves subclassing `SimpleModOption<T>` to create custom configuration option handlers.

### Understanding the Architecture

The Mod Config Menu uses a hierarchy of classes to handle different configuration types:

- **`BaseModOption`** - Abstract base class for all configuration options
- **`SimpleModOption<T>`** - Generic base class for simple configuration types
- **Concrete implementations** - `StringModOption`, `NumericModOption`, `ToggleModOption`
- **`ModOptionFactory`** - This is the factory that actually handles the creation
of mod options based on the type of the entry.