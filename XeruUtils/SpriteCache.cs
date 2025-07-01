using System;
using System.Collections.Generic;
using BepInEx.Logging;
using UnityEngine;

namespace XeruUtils;

public class SpriteCache
{
    private readonly Dictionary<string, SpriteCacheGroup> __cache = new Dictionary<string, SpriteCacheGroup>();

    public SpriteCacheGroup Get(string key)
    {
        lock (__cache)
        {
            return __cache[key];
        }
    }

    public void Add(string key, Sprite sprite)
    {
        lock (__cache)
        {
            if (__cache.ContainsKey(key))
            {
                return;
            }
            __cache.Add(key, new SpriteCacheGroup(sprite));
        }
    }
    
    public void Add(string key, Dictionary<ButtonState, Sprite> sprites)
    {
        lock (__cache)
        {
            if (__cache.ContainsKey(key))
            {
                return;
            }
            __cache.Add(key, new SpriteCacheGroup(sprites));
        }
    }
    
    public void Add(string key, Dictionary<string, Sprite> sprites)
    {
        lock (__cache)
        {
            if (__cache.ContainsKey(key))
            {
                return;
            }
            __cache.Add(key, new SpriteCacheGroup(sprites));
        }
    }

    public bool Contains(string key)
    {
        return __cache.ContainsKey(key);
    }

    public bool Remove(string key)
    {
        lock (__cache)
        {
            return __cache.Remove(key);
        }
    }

    public void LoadSpriteFromAssetTexture(string assetTextureName, SpriteSheetSlice slice, ManualLogSource logger)
    {
        if (Contains(assetTextureName))
        {
            return;
        }
        Add(assetTextureName, SpriteLoader.LoadSpriteFromAssetFile(assetTextureName, slice, logger));
    }

    public void LoadButtonSpritesFromAssetTexture(string assetTextureName, List<SpriteSheetSliceWithId<ButtonState>> slices, ManualLogSource logger, string cacheKey = null)
    {
        if (String.IsNullOrEmpty(cacheKey))
        {
            cacheKey = assetTextureName;
        }

        if (Contains(cacheKey))
        {
            return;
        }
        Dictionary<ButtonState, Sprite> sprites = SpriteLoader.LoadSpritesFromAssetFile(assetTextureName, slices, logger);
        Add(cacheKey, sprites);
    }
}

public class SpriteCacheGroup
{
    public readonly Sprite DefaultSprite;
    public readonly Dictionary<ButtonState, Sprite> ButtonSprites;
    public readonly Dictionary<string, Sprite> NamedSprites;

    public SpriteCacheGroup(Sprite defaultSprite)
    {
        DefaultSprite = defaultSprite;
    }

    public SpriteCacheGroup(Dictionary<ButtonState, Sprite> buttonSprites)
    {
        ButtonSprites = buttonSprites;
    }

    public SpriteCacheGroup(Dictionary<string, Sprite> namedSprites)
    {
        NamedSprites = namedSprites;
    }
}