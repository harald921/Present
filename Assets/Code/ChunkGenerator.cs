using UnityEngine;

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public class ChunkGenerator
{
    public Noise.Parameters _parameters { private get; set; }

    NoiseGenerator   _noiseGenerator;
    TextureGenerator _textureGenerator;

    public Queue<Action> _meshThreadInfoQueue    = new Queue<Action>();
    public Queue<Action> _textureThreadInfoQueue = new Queue<Action>();

    Transform _worldTransform;

    Mesh _chunkMesh;

    public ChunkGenerator(Noise.Parameters inParameters)
    {
        _worldTransform = GameObject.Find("World").transform;

        _parameters = inParameters;

        _chunkMesh = GenerateChunkMesh(_parameters.size);

        _noiseGenerator   = new NoiseGenerator();
        _textureGenerator = new TextureGenerator(this, inParameters.size);
    }


    // External
    public void Update()
    {
        ProcessQueues();
    }

    public Chunk GenerateChunk(Vector2 inOffset)
    {
        GameObject newGO = GenerateChunkGO(inOffset, _chunkMesh);
        Chunk newChunk = new Chunk(newGO);

        new Thread(() => GenerateChunkData(inOffset, newChunk)).Start();

        return newChunk;
    }


    // Internal
    void ProcessQueues()
    {
        while (_textureThreadInfoQueue.Count > 0)
            _textureThreadInfoQueue.Dequeue()();
    }

    void GenerateChunkData(Vector2 inOffset, Chunk inChunk)
    {
        NoiseGenerator.Result noiseResult = new NoiseGenerator.Result();
        Thread noiseThread = new Thread(() => _noiseGenerator.Generate(noiseResult, _parameters, inOffset, inChunk));

        noiseThread.Start();
        noiseThread.Join();

        new Thread(() => _textureGenerator.Generate(noiseResult, inChunk)).Start();
    }

    Mesh GenerateChunkMesh(int inSize)
    {
        int vertexSize  = inSize + 1;
        int vertexCount = vertexSize * vertexSize;

        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uv       = new Vector2[vertexCount];
        int[] triangles    = new int[inSize * inSize * 6];

        // Generate the vertices of the mesh
        for (int y = 0; y < vertexSize; y++)
            for (int x = 0; x < vertexSize; x++)
                vertices[y * vertexSize + x] = new Vector3 { x = x, y = 0, z = y };

        // Generate the and UVs
        for (int y = 0; y < vertexSize; y++)
            for (int x = 0; x < vertexSize; x++)
                uv[y * vertexSize + x] = new Vector2((float)x / inSize, (float)y / inSize);

        // Generate the triVertID's
        for (int y = 0; y < inSize; y++)
            for (int x = 0; x < inSize; x++)
            {
                int triVertOffset  = y * vertexSize + x;
                int triangleOffset = (y * inSize + x) * 6;

                triangles[triangleOffset + 0] = triVertOffset + 0;
                triangles[triangleOffset + 1] = triVertOffset + vertexSize + 0;
                triangles[triangleOffset + 2] = triVertOffset + vertexSize + 1;

                triangles[triangleOffset + 3] = triVertOffset + 0;
                triangles[triangleOffset + 4] = triVertOffset + vertexSize + 1;
                triangles[triangleOffset + 5] = triVertOffset + 1;
            }

        Mesh newMesh = new Mesh();
        newMesh.vertices  = vertices;
        newMesh.uv        = uv;
        newMesh.triangles = triangles;

        newMesh.RecalculateNormals();

        return newMesh;
    }

    GameObject GenerateChunkGO(Vector2 inOffset, Mesh inChunkMesh) // This is temporary, since all chunks will have the same mesh, simply move them around and change their texture
    {
        GameObject newGO = new GameObject(inOffset.ToString());
        newGO.transform.SetParent(_worldTransform);
        newGO.transform.position = new Vector3(inOffset.x * _parameters.size, 0, -inOffset.y * _parameters.size);

        MeshFilter filter = newGO.AddComponent<MeshFilter>();
        MeshRenderer renderer = newGO.AddComponent<MeshRenderer>();

        renderer.material.SetFloat("_Glossiness", 0.0f);

        filter.mesh = inChunkMesh;

        return newGO;
    }

    public void OnTextureDataRecieved(TextureGenerator.Result inResult, Chunk inChunk)
    {
        if (!inChunk.gameObject)
            return;

        Texture2D newTexture = new Texture2D(_parameters.size, _parameters.size);
        newTexture.filterMode = FilterMode.Point;
        newTexture.SetPixels(inResult.pixels);
        newTexture.Apply();

        inChunk.gameObject.GetComponent<MeshRenderer>().material.mainTexture = newTexture;
    }
}




public class NoiseGenerator
{
    public class Result
    {
        public float[,] heightMap;
    }

    public void Generate(Result inResult, Noise.Parameters inParameters, Vector2 inOffset, Chunk inChunk)
    {
        inResult.heightMap = Noise.Generate(inParameters, inOffset);
    }
}


public class TextureGenerator
{
    readonly ChunkGenerator _chunkGenerator;

    public class Result
    {
        public Color[] pixels;
    }

    readonly int size;

    public TextureGenerator(ChunkGenerator inChunkGenerator, int inSize)
    {
        _chunkGenerator = inChunkGenerator;

        size = inSize;
    }


    public void Generate(NoiseGenerator.Result inNoiseResult, Chunk inChunk)
    {
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                pixels[y * size + x] = Color.Lerp(Color.black, Color.white, inNoiseResult.heightMap[x,y]);

        Result result = new Result();
        result.pixels = pixels;

        lock (_chunkGenerator._textureThreadInfoQueue)
            _chunkGenerator._textureThreadInfoQueue.Enqueue(() => _chunkGenerator.OnTextureDataRecieved(result, inChunk));
    }
}