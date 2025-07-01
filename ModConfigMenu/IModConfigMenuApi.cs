/**
The MIT License (MIT)

Copyright 2016–2025 Jesse Plamondon-Willard (Pathoschild)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

THIS API IS COPIED FROM SPACECHASE0's GENERICMODCONFIGMENU FOR STARDEW VALLEY AND ADAPTED TO BEPINEX
If you want to support the author of this mod you can find the owner's github here:
https://github.com/spacechase0/StardewValleyMods/blob/develop/GenericModConfigMenu/IGenericModConfigMenuApi.cs
 */

using System;
using BepInEx;
using UnityEngine;

namespace ModConfigMenu;

/// <summary>The API which lets other mods add a config UI through Mod Config Menu.</summary>
public interface IModConfigMenuApi
{
    /*********
     ** Methods
     *********/
    /****
     ** Must be called first
     ****/
    /// <summary>Register a mod whose config can be edited through the UI.</summary>
    /// <param name="metadata">The mod's manifest.</param>
    /// <param name="reset">Reset the mod's config to its default values.</param>
    /// <param name="save">Save the mod's current config to the <c>config.json</c> file.</param>
    /// <remarks>Each mod can only be registered once, unless it's deleted via <see cref="Unregister"/> before calling this again.</remarks>
    void Register(BepInPlugin metadata, Action reset, Action save);

    /****
     ** Basic options
     ****/
    /// <summary>Add a section title at the current position in the form.</summary>
    /// <param name="metadata">The mod's manifest.</param>
    /// <param name="text">The title text shown in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the title, or <c>null</c> to disable the tooltip.</param>
    void AddSectionTitle(BepInPlugin metadata, Func<string> text, Func<string> tooltip = null);

    /// <summary>Add a paragraph of text at the current position in the form.</summary>
    /// <param name="metadata">The mod's manifest.</param>
    /// <param name="text">The paragraph text to display.</param>
    void AddParagraph(BepInPlugin metadata, Func<string> text);

    /// <summary>Add a boolean option at the current position in the form.</summary>
    /// <param name="metadata">The mod's manifest.</param>
    /// <param name="getValue">Get the current value from the mod config.</param>
    /// <param name="setValue">Set a new value in the mod config.</param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
    void AddBoolOption(BepInPlugin metadata, 
        Func<bool> getValue, 
        Action<bool> setValue, 
        Func<string> name, 
        Func<string> tooltip = null, 
        string fieldId = null
    );

    /// <summary>Add an integer option at the current position in the form.</summary>
    /// <param name="metadata">The mod's manifest.</param>
    /// <param name="getValue">Get the current value from the mod config.</param>
    /// <param name="setValue">Set a new value in the mod config.</param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="min">The minimum allowed value, or <c>null</c> to allow any.</param>
    /// <param name="max">The maximum allowed value, or <c>null</c> to allow any.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="formatValue">Get the display text to show for a value, or <c>null</c> to show the number as-is.</param>
    /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
    void AddNumberOption(BepInPlugin metadata, 
        Func<int> getValue, 
        Action<int> setValue, 
        Func<string> name, 
        Func<string> tooltip = null, 
        int? min = null, 
        int? max = null, 
        int? interval = null,
        Func<int, string> formatValue = null, 
        string fieldId = null
    );

    /// <summary>Add a float option at the current position in the form.</summary>
    /// <param name="metadata">The mod's manifest.</param>
    /// <param name="getValue">Get the current value from the mod config.</param>
    /// <param name="setValue">Set a new value in the mod config.</param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="min">The minimum allowed value, or <c>null</c> to allow any.</param>
    /// <param name="max">The maximum allowed value, or <c>null</c> to allow any.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="formatValue">Get the display text to show for a value, or <c>null</c> to show the number as-is.</param>
    /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
    void AddNumberOption(BepInPlugin metadata, 
        Func<float> getValue, 
        Action<float> setValue, 
        Func<string> name,
        Func<string> tooltip = null, 
        float? min = null, 
        float? max = null, 
        float? interval = null,
        Func<float, string> formatValue = null, 
        string fieldId = null
    );

    /// <summary>Add a string option at the current position in the form.</summary>
    /// <param name="metadata">The mod's manifest.</param>
    /// <param name="getValue">Get the current value from the mod config.</param>
    /// <param name="setValue">Set a new value in the mod config.</param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="allowedValues">The values that can be selected, or <c>null</c> to allow any.</param>
    /// <param name="formatAllowedValue">Get the display text to show for a value from <paramref name="allowedValues"/>, or <c>null</c> to show the values as-is.</param>
    /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
    void AddTextOption(BepInPlugin metadata, 
        Func<string> getValue, 
        Action<string> setValue, 
        Func<string> name,
        Func<string> tooltip = null, 
        string[] allowedValues = null, 
        Func<string, string> formatAllowedValue = null,
        string fieldId = null
    );


    /****
     ** Multi-page management
     ****/
    /// <summary>Start a new page in the mod's config UI, or switch to that page if it already exists. All options registered after this will be part of that page.</summary>
    /// <param name="metadata">The mod's manifest.</param>
    /// <param name="pageId">The unique page ID.</param>
    /// <param name="pageTitle">The page title shown in its UI, or <c>null</c> to show the <paramref name="pageId"/> value.</param>
    /// <remarks>You must also call <see cref="AddPageLink"/> to make the page accessible. This is only needed to set up a multi-page config UI. If you don't call this method, all options will be part of the mod's main config UI instead.</remarks>
    void AddPage(BepInPlugin metadata, string pageId, Func<string> pageTitle = null);

    /// <summary>Add a link to a page added via <see cref="AddPage"/> at the current position in the form.</summary>
    /// <param name="metadata">The mod's manifest.</param>
    /// <param name="pageId">The unique ID of the page to open when the link is clicked.</param>
    /// <param name="text">The link text shown in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the link, or <c>null</c> to disable the tooltip.</param>
    void AddPageLink(BepInPlugin metadata, string pageId, Func<string> text, Func<string> tooltip = null);


    /****
     ** Advanced
     ****/
    /// <summary>Add an option at the current position in the form using custom rendering logic.</summary>
    /// <param name="metadata">The mod's manifest.</param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="widget">The widget game object to be added to the menu</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="beforeMenuOpened">A callback raised just before the menu containing this option is opened.</param>
    /// <param name="beforeSave">A callback raised before the form's current values are saved to the config (i.e. before the <c>save</c> callback passed to <see cref="Register"/>).</param>
    /// <param name="afterSave">A callback raised after the form's current values are saved to the config (i.e. after the <c>save</c> callback passed to <see cref="Register"/>).</param>
    /// <param name="beforeReset">A callback raised before the form is reset to its default values (i.e. before the <c>reset</c> callback passed to <see cref="Register"/>).</param>
    /// <param name="afterReset">A callback raised after the form is reset to its default values (i.e. after the <c>reset</c> callback passed to <see cref="Register"/>).</param>
    /// <param name="beforeMenuClosed">A callback raised just before the menu containing this option is closed.</param>
    /// <param name="height">The pixel height to allocate for the option in the form, or <c>null</c> for a standard input-sized option. This is called and cached each time the form is opened.</param>
    /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
    /// <remarks>The custom logic represented by the callback parameters is responsible for managing its own state if needed. For example, you can store state in a static field or use closures to use a state variable.</remarks>
    void AddComplexOption(BepInPlugin metadata, 
        Func<string> name, 
        Func<GameObject> widget,
        Func<string> tooltip = null, 
        Action beforeMenuOpened = null, 
        Action beforeSave = null, 
        Action afterSave = null,
        Action beforeReset = null, 
        Action afterReset = null, 
        Action beforeMenuClosed = null, 
        Func<int> height = null,
        string fieldId = null
    );

    /// <summary>Set whether the options registered after this point can only be edited from the title screen.</summary>
    /// <param name="metadata">The mod's manifest.</param>
    /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
    /// <remarks>This lets you have different values per-field. Most mods should just set it once in <see cref="Register"/>.</remarks>
    void SetTitleScreenOnlyForNextOptions(BepInPlugin metadata, bool titleScreenOnly);

    /// <summary>Register a method to notify when any option registered by this mod is edited through the config UI.</summary>
    /// <param name="metadata">The mod's manifest.</param>
    /// <param name="onChange">The method to call with the option's unique field ID and new value.</param>
    /// <remarks>Options use a randomized ID by default; you'll likely want to specify the <c>fieldId</c> argument when adding options if you use this.</remarks>
    void OnFieldChanged(BepInPlugin metadata, Action<string, object> onChange);

    /// <summary>Remove a mod from the config UI and delete all its options and pages.</summary>
    /// <param name="metadata">The mod's manifest.</param>
    void Unregister(BepInPlugin mod);
}