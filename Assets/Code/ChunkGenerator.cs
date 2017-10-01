using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkGenerator
{
    NoiseSettings.Parameters _noiseParameters;

    NoiseThreader   _noiseGenerator;
    MeshThreader    _meshGenerator;
    TextureThreader _textureGenerator;

    Mesh _mesh = new Mesh();

    // Constructor
    public ChunkGenerator(NoiseSettings.Parameters inNoiseParameters)
    {
        _noiseParameters = inNoiseParameters;

        _noiseGenerator   = new NoiseThreader();
        _meshGenerator    = new MeshThreader(inNoiseParameters.size);
        _textureGenerator = new TextureThreader(inNoiseParameters.size, 32, GameObject.Find("World").GetComponent<TextureGenerator>());

        CreateCachedMesh();
    }

    // External
    public Chunk GenerateChunk(Vector2 inOffset)
    {
        // Generate noisemap
        float[,] newNoiseMap = Noise.Generate(_noiseParameters, inOffset);

        // Create new GameObject, set position and name
        GameObject newGO = new GameObject();
        newGO.transform.position = new Vector3(inOffset.x * _noiseParameters.size, 0, -inOffset.y * _noiseParameters.size);
        newGO.name = inOffset.ToString();

        // Set mesh of GameObject
        MeshFilter meshFilter = newGO.AddComponent<MeshFilter>();
        meshFilter.mesh = _mesh;

        // Generate and set texture of GameObject
        MeshRenderer meshRenderer = newGO.AddComponent<MeshRenderer>();
        meshRenderer.material.mainTexture = _textureGenerator.Generate(newNoiseMap);
        meshRenderer.material.SetFloat("_Glossiness", 0.0f);

        return new Chunk(newGO);
    }

    public void GenerateChunk(NoiseSettings.Parameters inNoiseParameters, Vector2 inOffset, Chunk inChunk)
    {
        _noiseParameters = inNoiseParameters;

        float[,] newNoiseMap = Noise.Generate(inNoiseParameters, inOffset);

        inChunk.gameObject.GetComponent<MeshRenderer>().material.mainTexture = _textureGenerator.Generate(newNoiseMap);
    }

    // Internal
    void CreateCachedMesh()
    {
        MeshThreader.MeshData meshData = _meshGenerator.Generate();
        _mesh.vertices  = meshData.vertices;
        _mesh.uv        = meshData.uv;
        _mesh.triangles = meshData.triangles;

        _mesh.RecalculateNormals();
    }
}






public class NoiseThreader
{
    public float[,] Generate(NoiseSettings.Parameters inNoiseParameters, Vector2 inOffset)
    {
        return Noise.Generate(inNoiseParameters, inOffset);
    }
}

public class MeshThreader
{
    public struct MeshData
    {
        public Vector3[] vertices;
        public Vector2[] uv;
        public int[] triangles;
    }
    readonly MeshData _meshData;

    readonly int _size;
    readonly int _tileCount;
    readonly int _triangleCount;
    readonly int _vertexSize;
    readonly int _vertexCount;

    public MeshThreader(int inSize)
    {
        _size          = inSize;
        _tileCount     = _size * _size;
        _triangleCount = _tileCount * 2;
        _vertexSize    = _size + 1;
        _vertexCount   = _vertexSize * _vertexSize;

        _meshData.vertices  = new Vector3[_vertexCount];
        _meshData.uv        = new Vector2[_vertexCount];
        _meshData.triangles = new int[_triangleCount * 3];

        // Generate vertices and UV
        for (int z = 0; z < _vertexSize; z++)
            for (int x = 0; x < _vertexSize; x++)
            {
                int currentIndex = z * _vertexSize + x;

                _meshData.vertices[currentIndex] = new Vector3(x, 0, z);
                _meshData.uv[currentIndex] = new Vector2((float)x / _size, (float)z / _size); // TODO: Rewrite this so that division isn't used
            }

        // Generate the triangles
        for (int z = 0; z < _size; z++)
            for (int x = 0; x < _size; x++)
            {
                int currentTileID = z * _size + x;
                int triVertOffset = z * _vertexSize + x;
                int triangleOffset = currentTileID * 6;

                _meshData.triangles[triangleOffset + 0] = triVertOffset + 0;
                _meshData.triangles[triangleOffset + 1] = triVertOffset + _vertexSize + 0;
                _meshData.triangles[triangleOffset + 2] = triVertOffset + _vertexSize + 1;

                _meshData.triangles[triangleOffset + 3] = triVertOffset + 0;
                _meshData.triangles[triangleOffset + 4] = triVertOffset + _vertexSize + 1;
                _meshData.triangles[triangleOffset + 5] = triVertOffset + 1;
            }
    }

    public MeshData Generate()
    {
        return _meshData;
    }
}

public class TextureThreader
{
    TextureGenerator _textureGenerator;

    readonly int _size;
    readonly int _tileSize;
    readonly Color[] _pixels;

    public TextureThreader(int inSize, int inTileSize, TextureGenerator inTextureGenerator)
    {
        _textureGenerator = inTextureGenerator;

        _size = inSize;
        _tileSize = inTileSize;
        _pixels = new Color[_size * _size * _tileSize * _tileSize];
    }

    public Texture2D Generate(float[,] inNoiseMap)
    {
        int sizeTimesTileSize = _size * _tileSize;
        Texture2D newTexture = new Texture2D(sizeTimesTileSize, sizeTimesTileSize);

        for (int y = 0; y < _size; y++)
            for (int x = 0; x < _size; x++)
            {
                newTexture.SetPixels(x * _tileSize, y * _tileSize, _tileSize, _tileSize, _textureGenerator.GetSpritePixels(0));

            }

        newTexture.filterMode = FilterMode.Point;
        newTexture.Apply();

        return newTexture;
    }
}