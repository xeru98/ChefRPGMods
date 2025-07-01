using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using UnityEngine;

namespace XeruUtils;

public static class SpriteLoader
{
    
    public static Texture2D LoadTexture(string filePath, ManualLogSource logger)
    {
        if (!File.Exists(filePath))
        {
            logger.LogError($"SpriteLoader: File not found {filePath}");
            return null;
        }
        
        byte[] data = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        texture.LoadImage(data);
        texture.filterMode = FilterMode.Point;
        texture.Apply(false);
        
        return texture;
    }

    public static string GetAssetTexturePath(string fileName)
    {
        return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", fileName);
    }

    public static Dictionary<T, Sprite> SliceSpriteSheet<T>(Texture2D spriteTexture, ICollection<SpriteSheetSliceWithId<T>> slices)
    {
        Dictionary<T, Sprite> retVal = new Dictionary<T, Sprite>();
        foreach (SpriteSheetSliceWithId<T> slice in slices)
        {
            retVal[slice.id] = GetSpriteFromSlice(spriteTexture, slice);
        }
        return retVal;
    }

    public static Sprite GetSpriteFromSlice(Texture2D spriteTexture, SpriteSheetSlice slice)
    {
        if (slice.border == Vector4.zero)
        {
            return Sprite.Create(spriteTexture, slice.rect, slice.pivot, slice.ppu);
        }
        return Sprite.Create(spriteTexture, slice.rect, slice.pivot, slice.ppu, 0, SpriteMeshType.FullRect, slice.border);
    }

    public static Sprite LoadSpriteFromAssetFile(string filePath, SpriteSheetSlice slice, ManualLogSource logger)
    {
        Texture2D texture = LoadTexture(GetAssetTexturePath(filePath), logger);
        return GetSpriteFromSlice(texture, slice);
    }

    public static Dictionary<T, Sprite> LoadSpritesFromAssetFile<T>(string filePath, List<SpriteSheetSliceWithId<T>> slices, ManualLogSource logger)
    {
        Texture2D texture = LoadTexture(GetAssetTexturePath(filePath), logger);
        return SliceSpriteSheet<T>(texture, slices);
    }
}

public class SpriteSheetSlice
{
    public readonly Rect rect;
    public readonly float ppu;
    public readonly Vector4 border;
    public readonly Vector2 pivot;

    public SpriteSheetSlice(Rect rect, int ppu, Vector4? border = null, Vector2? pivot = null)
    {
        this.rect = rect;
        this.ppu = ppu;
        this.border = border ?? Vector4.zero;
        this.pivot = pivot ?? new Vector2(0.5f, 0.5f);
    }
    
    public SpriteSheetSlice(Vector2 position, Vector2 size, int ppu, Vector4? border = null, Vector2? pivot = null) 
        : this(new Rect(position, size), ppu, border, pivot) 
    {} 
}

public class SpriteSheetSliceWithId<T> : SpriteSheetSlice
{
    public readonly T id;

    public SpriteSheetSliceWithId(T id, Rect rect, int ppu, Vector4? border = null, Vector2? pivot = null)
        : base(rect, ppu, border, pivot)
    {
        this.id = id;
    }
    public SpriteSheetSliceWithId(T id, Vector2 position, Vector2 size, int ppu, Vector4? border = null, Vector2? pivot = null)
        : base(new Rect(position, size), ppu, border, pivot)
    {
        this.id = id;
    }
}