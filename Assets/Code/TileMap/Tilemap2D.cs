using UnityEngine;

public class Tilemap2D : MonoBehaviour
{
	[SerializeField] Texture2D spriteSheet;             // The Sprite Sheet

    [SerializeField] int _chunkSize      = 64;          // Amount of tiles on one axis of the chunk
	[SerializeField] int _tileResolution = 8;           // Resolution of the tile graphics

    Vector2 _spriteCount;                               // Amount of sprites on each axis in the sprite sheet

    void Start ()
    {
        // Calculate the dimensions of the sprite sheet
        _spriteCount = new Vector2(spriteSheet.width / _tileResolution, spriteSheet.height / _tileResolution);  

        // Create a data texture
        Texture2D dataTexture = new Texture2D(_chunkSize, _chunkSize);
        for (int x = 0; x <_chunkSize; x++)
			for (int y = 0; y <_chunkSize; y++)
				dataTexture.SetPixel(x, y, new Color(Random.Range(0, 4) / _spriteCount.x ,0,0,0));

        dataTexture.filterMode = FilterMode.Point;
		dataTexture.Apply();

        // Send the tilemap and the data texture to the material
        Material tilemapMaterial = GetComponent<MeshRenderer>().material;

		tilemapMaterial.SetTexture("_SpriteSheet", spriteSheet);
		tilemapMaterial.SetTexture("_DataMap", dataTexture);
		tilemapMaterial.SetFloat("_DataMapSize", dataTexture.width);
	}

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Material tilemapMaterial = GetComponent<MeshRenderer>().material;

            // Create a data texture
            Texture2D dataTexture = (Texture2D)tilemapMaterial.GetTexture("_DataMap");
            for (int x = 0; x < _chunkSize; x++)
                for (int y = 0; y < _chunkSize; y++)
                    dataTexture.SetPixel(x, y, new Color(Random.Range(0, 4) / _spriteCount.x, 0, 0, 0));

            dataTexture.Apply();
        }
    }
}


