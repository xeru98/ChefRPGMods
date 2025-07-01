using System;
using ModConfigMenu.Framework.ModOption;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XeruUtils;

namespace ModConfigMenu.Components;

internal class ModOptionContainer
{
    public GameObject Container { get; private set; }
    public GameObject NameContainer { get; private set; }
    public GameObject ValueContainer { get; private set; }

    public ModOptionContainer(BaseModOption modOption, bool debug = false)
    {
        Container = new GameObject($"{modOption.FieldId}_container", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        HorizontalLayoutGroup hbox = Container.GetComponent<HorizontalLayoutGroup>();
        
        NameContainer = new GameObject($"{modOption.FieldId}_nameContainer",typeof(RectTransform), typeof(LayoutElement));
        NameContainer.transform.SetParent(Container.transform, false);
        ValueContainer = new GameObject($"{modOption.FieldId}_valueContainer",typeof(RectTransform), typeof(LayoutElement));
        ValueContainer.transform.SetParent(Container.transform, false);
        
        if (debug)
        {
            Container.AddComponent<Image>().color = new Color(0.75f, 0, 0, 0.5f);
            NameContainer.AddComponent<Image>().color = new Color(0, 0.75f, 0, 0.5f);
            ValueContainer.AddComponent<Image>().color = new Color(0, 0, 0.75f, 0.5f);
        }
        
        hbox.childControlHeight = true;
        hbox.childControlWidth = true;
        hbox.childForceExpandHeight = true;
        hbox.childForceExpandWidth = true;
        hbox.childAlignment = TextAnchor.MiddleLeft;
        hbox.spacing = 3;
        
        LayoutElement nameLE = NameContainer.GetComponent<LayoutElement>();
        nameLE.flexibleWidth = 1;
        nameLE.preferredWidth = 1;
        
        LayoutElement valueLE = ValueContainer.GetComponent<LayoutElement>();
        valueLE.flexibleWidth = 2;
        valueLE.preferredWidth = 0;
        
        GameObject name = new GameObject($"{modOption.FieldId}_name", typeof(RectTransform), typeof(TextMeshProUGUI));
        name.transform.SetParent(NameContainer.transform, false);
        UIHelpers.SetupTextMesh(name.GetComponent<TextMeshProUGUI>(), Plugin.FONT, ModConfigMenu.Framework.Constants.PARAGRAPH_FONT_SIZE, ModConfigMenu.Framework.Constants.BODY_FONT_COLOR, modOption.Name(), TextAlignmentOptions.Right);
        UIHelpers.SetupRectTransform(name.GetComponent<RectTransform>(), AnchorPosition.BottomRight, AnchorPosition.TopRight, AnchorPosition.CenterRight);

        GameObject value = modOption.GetUIGameObject();
        value.transform.SetParent(ValueContainer.transform, false);
        UIHelpers.SetupRectTransform(value.GetComponent<RectTransform>(), AnchorPosition.CenterLeft);
        
        Container.GetComponent<RectTransform>().sizeDelta = new Vector2(0, Math.Max(name.GetComponent<TextMeshProUGUI>().GetPreferredValues().y, value.GetComponent<RectTransform>().sizeDelta.y));
    }
}