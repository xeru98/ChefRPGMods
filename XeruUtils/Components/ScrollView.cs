using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace XeruUtils.Components;

public class ScrollView
{
    private readonly Sprite backgroundSprite;
    private readonly Dictionary<ButtonState, Sprite> handleSprites;
    public GameObject Root { get; private set; }

    private GameObject viewport;
    private ScrollRect scrollRect;


    public ScrollView(GameObject outer, Sprite backgroundSprite, Sprite handleSprite)
        :this(outer, backgroundSprite, new Dictionary<ButtonState, Sprite>(){{ButtonState.Default, handleSprite}})
    {}
    
    public ScrollView(GameObject outer, Sprite backgroundSprite, Dictionary<ButtonState, Sprite> handleSprites)
    {
        this.backgroundSprite = backgroundSprite;
        this.handleSprites = handleSprites;
        if (!handleSprites.ContainsKey(ButtonState.Default))
        {
            throw new ArgumentException($"{outer.name} is missing a default sprite for the handle");
        }
        Construct(outer);
    }

    public void ClearContent()
    {
        if (!scrollRect || !scrollRect.content || !scrollRect.content.gameObject)
        {
            return;
        }
        
        Object.Destroy(scrollRect.content.gameObject);
    }

    private void Construct(GameObject outer)
    {
        Root = new GameObject($"ScrollView_{outer.name}", typeof(RectTransform));
        Root.transform.SetParent(outer.transform, false);
        GameObject scrollbar = ConstructScrollbar();
        scrollbar.transform.SetParent(Root.transform, false);

        GameObject scrollRectObj = new GameObject("ScrollRect", typeof(RectTransform), typeof(ScrollRect));
        scrollRectObj.transform.SetParent(Root.transform, false);
        RectTransform scrollRectRT = scrollRectObj.GetComponent<RectTransform>();
        UIHelpers.SetupFillRectTransform(scrollRectRT);
        scrollRect = scrollRectObj.GetComponent<ScrollRect>();
        scrollRect.verticalScrollbar = scrollbar.GetComponent<Scrollbar>();
        scrollRect.vertical = true;
        scrollRect.horizontal = false;
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
        scrollRect.scrollSensitivity = 10.0f;
        
        viewport = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D), typeof(Image));
        RectTransform viewportRT = viewport.GetComponent<RectTransform>();
        viewportRT.SetParent(scrollRect.transform, false);
        UIHelpers.SetupFillRectTransform(viewportRT, offsetMax: new Vector2(-(scrollbar.GetComponent<Image>().sprite.rect.width * 1.5f), 0));
        scrollRect.viewport = viewportRT;
        Image viewportImage = viewport.GetComponent<Image>();
        viewportImage.color = new Color(0, 0, 0, 0);
        
        scrollbar.transform.SetAsLastSibling();
    }

    public void SetContent(GameObject content)
    {
        content.transform.SetParent(viewport.transform, false);
        scrollRect.content = content.GetComponent<RectTransform>();
    }
    
    private GameObject ConstructScrollbar()
    {
        // Setup the scrollbar
        GameObject scrollbarObj = new GameObject("Scrollbar", typeof(RectTransform), typeof(Image), typeof(Scrollbar));
        RectTransform scrollbarRT = scrollbarObj.GetComponent<RectTransform>();
        UIHelpers.SetupRectTransform(scrollbarRT, AnchorPosition.BottomRight, AnchorPosition.TopRight, AnchorPosition.CenterRight);

        var scrollbarBackground = scrollbarObj.GetComponent<Image>();
        scrollbarBackground.type = Image.Type.Sliced;
        scrollbarBackground.sprite = backgroundSprite;
        scrollbarBackground.color = Color.white;
        scrollbarRT.sizeDelta = new Vector2(scrollbarBackground.sprite.rect.width, 0); // had to save this until we had the background loaded
        scrollbarRT.anchoredPosition = new Vector2(-(scrollbarBackground.sprite.rect.width / 4), 0);


        // Next we set up the scroll area to bind the handle (we use offset instead of size delta here to make sure it stretches to fill the parent)
        RectTransform scrollArea = new GameObject("ScrollArea", typeof(RectTransform)).GetComponent<RectTransform>();
        scrollArea.SetParent(scrollbarRT, false);
        UIHelpers.SetupFillRectTransform(scrollArea);


        // The last component of the scrollbar is the handle (the actual thing used for scrolling)
        GameObject handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
        handle.transform.SetParent(scrollArea, false);
        RectTransform handleRT = handle.GetComponent<RectTransform>();

        Image handleImage = handle.GetComponent<Image>();
        handleImage.sprite = handleSprites[ButtonState.Default];
        handleImage.type = Image.Type.Sliced;
        handleRT.sizeDelta = new Vector2(0, handleImage.sprite.rect.height);
        
        // Since the handle is sliced we need to limit the range of motion to prevent overflow
        scrollArea.offsetMin = new Vector2(0, handleImage.sprite.rect.height/2);
        scrollArea.offsetMax = new Vector2(0, -handleImage.sprite.rect.height/2);
        
        // The final step is to hook up the scrollbar
        Scrollbar scrollbar = scrollbarObj.GetComponent<Scrollbar>();
        scrollbar.SetDirection(Scrollbar.Direction.BottomToTop, false);
        scrollbar.targetGraphic = handleImage;
        scrollbar.handleRect = handleRT;
        
        // If we handed it button transition states then we can set up the SpriteSwap
        if (handleSprites.Count > 1)
        {
            scrollbar.transition = Scrollbar.Transition.SpriteSwap;
            SpriteState handleSpriteState = new SpriteState();
            handleSpriteState.highlightedSprite = handleSprites.TryGetValue(ButtonState.Hovered, out Sprite hoveredSprite) ? hoveredSprite : null;
            handleSpriteState.pressedSprite = handleSprites.TryGetValue(ButtonState.Pressed, out Sprite pressedSprite) ? pressedSprite : null;
            scrollbar.spriteState = handleSpriteState;
        }

        return scrollbarObj;
    }
}