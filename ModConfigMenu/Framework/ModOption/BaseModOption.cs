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
using UnityEngine;

namespace ModConfigMenu.Framework.ModOption;

internal abstract class BaseModOption
{
    public string FieldId { get; }
    
    public Func<string> Name { get; }
    
    public Func<string> Tooltip { get; }
    
    public ModConfig Owner { get; } 

    /** Events **/
    
    /// <summary>Perform any logic before the value is reset to the default</summary>
    public abstract void PreReset();

    /// <summary>Perform any logic before the value is reset to the default</summary>
    public abstract void PostReset();

    /// <summary>Perform any logic before the value is reset to the default</summary>
    public abstract void PreSave();
    
    /// <summary>Perform any logic before the value is reset to the default</summary>
    public abstract void PostSave();

    /// <summary>Perform any logic before the value is reset to the default</summary>
    public abstract void PreMenuOpened();
    
    /// <summary>Perform any logic before the value is reset to the default</summary>
    public abstract void PreMenuClosed();

    public abstract GameObject GetUIGameObject();
    
    // Protect the Ctor
    /// <summary> Construct a new instance</summary>
    /// <param name="fieldId">The GUID of the field we are creating</param>
    /// <param name="name">The human-readable name to display</param>
    /// <param name="tooltip">The tool tip that displays when hovered</param>
    /// <param name="owner">The mod that owns this option</param>
    protected BaseModOption(string fieldId, Func<string> name, Func<string> tooltip, ModConfig owner)
    {
        fieldId = Guid.NewGuid().ToString("N");
        tooltip ??= () => null;
        
        FieldId = fieldId;
        Name = name;
        Tooltip = tooltip;
        Owner = owner;
    }
}