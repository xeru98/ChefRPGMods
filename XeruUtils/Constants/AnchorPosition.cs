using UnityEngine;

namespace XeruUtils;

public static class AnchorPosition
{
    public static readonly Vector2 TopLeft = new Vector2(0, 1);
    public static readonly Vector2 TopCenter = new Vector2(0.5f, 1);
    public static readonly Vector2 TopRight = new Vector2(1, 1);
    public static readonly Vector2 CenterLeft = new Vector2(0, 0.5f);
    public static readonly Vector2 Center = new Vector2(0.5f, 0.5f);
    public static readonly Vector2 CenterRight = new Vector2(1, 0.5f);
    public static readonly Vector2 BottomLeft = new Vector2(0, 0);
    public static readonly Vector2 BottomCenter = new Vector2(0.5f, 0);
    public static readonly Vector2 BottomRight = new Vector2(1, 0);
}
