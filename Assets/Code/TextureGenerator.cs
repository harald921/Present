using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureGenerator : MonoBehaviour
{
    [SerializeField] Sprite[] _sprites;

    Color[][] _spritesPixels;

    void Awake()
    {
        _spritesPixels = new Color[_sprites.Length][];

        for (int i = 0; i < _sprites.Length; i++)
        {
            _spritesPixels[i] = _sprites[i].texture.GetPixels();
        }
    }

    public Color[] GetSpritePixels(int inSpriteID)
    {
        return _spritesPixels[inSpriteID];
    }
}