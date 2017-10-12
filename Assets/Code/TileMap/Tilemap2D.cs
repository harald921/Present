using UnityEngine;

public class TileMap2D : MonoBehaviour
{
    const int _spriteCount = 16;  // Sprite sheets are currently limited to 16x16 sprites. Changes need to be made to shader if this is to be changed.

    [SerializeField] int _chunkSize        = 64;          // Amount of tiles on one axis of the chunk
	[SerializeField] int _spriteResolution = 32;          // Resolution of the tile graphics


    
    // Start, Update
    void Start()
    {
        GetComponent<MeshRenderer>().material.SetTexture("_DataMap", CreateDataTexture(_chunkSize));
        GetComponent<MeshRenderer>().material.SetFloat("_DataMapSize", _chunkSize);
	}


    // Internal
    Texture2D CreateDataTexture(int inTextureSize)
    {
        Texture2D dataTexture = new Texture2D(_chunkSize, _chunkSize);

        dataTexture.filterMode = FilterMode.Point;
        dataTexture.Apply();

        return dataTexture;
    }

    Color GetUVColor(Sprite inSprite)
    {
        Vector2 spriteCoords = new Vector2
        {
            x = (int)inSprite % _spriteCount,
            y = Mathf.FloorToInt((int)inSprite / _spriteCount)
        };

        return new Color(spriteCoords.x / _spriteCount, spriteCoords.y / _spriteCount, 0, 0);
    }

    enum Sprite
    {
        Grass1,
        Grass2,
        Grass3,
        Water,
    }
}


