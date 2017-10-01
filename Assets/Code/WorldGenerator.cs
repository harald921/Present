using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

[CustomEditor(typeof(WorldGenerator))]
public class WorldGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WorldGenerator worldGeneratorScript = (WorldGenerator)target;

        if (GUILayout.Button("Re-Generate"))
        {
            if (Application.isPlaying)
                worldGeneratorScript.RegenerateChunks();

            else
                Debug.Log("Cannot generate in edit mode");
        }
    }
}

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] int _worldSize = 1;

    Dictionary<Vector2, Chunk> _worldChunks = new Dictionary<Vector2, Chunk>();

    ChunkGenerator _chunkGenerator;
    NoiseSettings _noiseSettings;

    void Awake()
    {
        _noiseSettings = GetComponent<NoiseSettings>();

        _chunkGenerator = new ChunkGenerator(_noiseSettings.parameters);
    }

    void Start()
    {
        for (int z = 0; z < _worldSize; z++)
            for (int x = 0; x < _worldSize; x++)
                _worldChunks.Add(new Vector2(x, z), _chunkGenerator.GenerateChunk(new Vector2(x, z)));
    }

    public void RegenerateChunks()
    {
        foreach (var chunkCoord in _worldChunks.Keys)
            _chunkGenerator.GenerateChunk(_noiseSettings.parameters, chunkCoord, _worldChunks[chunkCoord]);
    }
}