using System.Collections.Generic;
using UnityEngine;

public struct TilePrefabPair
{
    private readonly TileType m_Type;
    private readonly GameObject m_Prefab;

    public TileType Type => m_Type;
    public GameObject Prefab => m_Prefab;

    public TilePrefabPair(TileType type, GameObject prefab)
    {
        m_Type = type;
        m_Prefab = prefab;
    }
}

public class WorldGrid : MonoBehaviour
{
    private bool m_HasMapSpawned = false;
    private MapData m_MapData;
    //TODO: Propogate tilescale to tile prefabs
    private float tileScale = 2.0f;

    private AI.IPathFinder m_PathFinder;

    [SerializeField]
    private Dictionary<TileType, GameObject> m_PrefabDictionary;

    private void Awake()
    {
        TilePrefabScriptableObject so = Resources.Load<TilePrefabScriptableObject>("ScriptableObjects/TilePrefabs");
        List <TilePrefabPair> prefabs = new List<TilePrefabPair>();
        for (int i = 0; i < so.m_TileTypes.Count; i++)
        {
            prefabs.Add(new TilePrefabPair(so.m_TileTypes[i], so.m_GameObjects[i]));
        }
        Initialize("Assets/Resources/MapSettings/map_1.txt", prefabs);
        m_PathFinder = new AI.Dijkstra(m_MapData.Accessibles);
    }


    public void Initialize(string fileName, IEnumerable<TilePrefabPair> prefabs)
    {
        m_PrefabDictionary = new Dictionary<TileType, GameObject>();
        foreach(TilePrefabPair prefab in prefabs)
        {
            m_PrefabDictionary.Add(prefab.Type, prefab.Prefab);
        }

        MapReader.LoadFromFile(fileName, out m_MapData);
        BuildTiles();
    }

    private void BuildTiles()
    {
        if (m_HasMapSpawned)
        {
            DestroyTiles();
        }
        for (int y = 0; y < m_MapData.Bounds.y; y++)
        {
            for (int x = 0; x < m_MapData.Bounds.x; x++)
            {
                int tileId = m_MapData.Grid[x + (y * m_MapData.Bounds.x)];
                TileType tileType = TileMethods.TypeById[tileId];
                var go = Instantiate(m_PrefabDictionary[tileType], transform);
                go.transform.position = new Vector3(x * tileScale, 0, y * tileScale);
            }
        }
        m_HasMapSpawned = true;
    }

    private void DestroyTiles()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        m_HasMapSpawned = false;
    }

    public IEnumerable<Vector2Int> GetPath(Vector2Int start, Vector2Int end)
    {
        return m_PathFinder.FindPath(start, end);
    }

    public void GetSpawnData()
    {
        //return MapData.SpawnData
    }
}
