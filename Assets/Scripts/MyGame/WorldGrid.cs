using System.Collections.Generic;
using System.Linq;
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
    [SerializeField]
    private uint m_MapIndex = 1;
    //TODO: Propogate world scale to tileprefabs
    [SerializeField]
    private float m_WorldScale = 2.0f;

    private bool m_HasMapSpawned = false;
    private MapData m_MapData;
    private AI.IPathFinder m_PathFinder;
    public List<Wave> SpawnWaves => m_MapData.Waves;
    private Dictionary<TileType, GameObject> m_PrefabDictionary;

    private void Awake()
    {
        TilePrefabScriptableObject so = Resources.Load<TilePrefabScriptableObject>("ScriptableObjects/TilePrefabs");
        List <TilePrefabPair> prefabs = new List<TilePrefabPair>();
        for (int i = 0; i < so.m_TileTypes.Count; i++)
        {
            prefabs.Add(new TilePrefabPair(so.m_TileTypes[i], so.m_GameObjects[i]));
        }
        Initialize("Assets/Resources/MapSettings/map_" + m_MapIndex.ToString() + ".txt", prefabs);
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
                go.transform.position = new Vector3(x * m_WorldScale, 0, y * m_WorldScale);
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

    public IEnumerable<Vector3> GetPath(Vector3 start, Vector3 end)
    {
        Vector2Int scaledStart = new Vector2Int((int)(start.x / m_WorldScale), (int)(start.z / m_WorldScale));
        Vector2Int scaledEnd = new Vector2Int((int)(end.x / m_WorldScale), (int)(end.z / m_WorldScale));

        List<Vector2Int> path = m_PathFinder.FindPath(scaledStart, scaledEnd).ToList();
        List<Vector3> worldPath = new List<Vector3>();
        for(int i =0; i < path.Count; i++)
        {
            worldPath.Add(new Vector3(path[i].x * m_WorldScale, 0, path[i].y * m_WorldScale));
        }
        return worldPath;
    }

    public void GetSpawnData()
    {
        //return MapData.SpawnData
    }
}
