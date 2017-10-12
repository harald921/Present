using UnityEngine;

public class TileMap2D : MonoBehaviour
{
    enum Sprites
    {
        Grass1,
        Grass2,
        Grass3,
        Water,
    }

    const int _spriteCount = 16;  // Sprite sheets are currently limited to 16x16 sprites. Changes need to be made to shader if this is to be changed.

    [SerializeField] int _chunkSize        = 64;          // Amount of tiles on one axis of the chunk
	[SerializeField] int _spriteResolution = 32;          // Resolution of the tile graphics

    Material tileMapMaterial;

    // Start, Update
    void Start ()
    {
        GetComponent<MeshRenderer>().material.SetTexture("_DataMap", CreateDataTexture(_chunkSize));
        GetComponent<MeshRenderer>().material.SetFloat("_DataMapSize", _chunkSize);
	}

    // External
    static Color GetUVColor(Vector2 inSpriteCoords)
    {
        return new Color(inSpriteCoords.x / _spriteCount, inSpriteCoords.y / _spriteCount, 0, 0);
    }

    // Internal
    Texture2D CreateDataTexture(int inTextureSize)
    {
        Texture2D dataTexture = new Texture2D(_chunkSize, _chunkSize);

        dataTexture.filterMode = FilterMode.Point;
        dataTexture.Apply();

        return dataTexture;
    }
}


