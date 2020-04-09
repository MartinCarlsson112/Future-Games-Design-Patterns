using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UnitManager : MonoBehaviour
{
    public GameObject prefab;

    GameObjectPool m_UnitPool;
    WorldGrid m_WorldGrid;

    [Inject]
    [HideInInspector]
    public EnemyBase m_EnemyBase;

    [Inject]
    [HideInInspector]
    public PlayerBase playerBase;

    [Inject]
    public WorldGrid WorldGrid
    {
        set => m_WorldGrid = value;
        get => m_WorldGrid;
    }
    //wave data

        //make into observer
            //observe units active

    //currently active unit count

    //unitmanager scriptable object

    private void Start()
    {
        m_UnitPool = new GameObjectPool(5, prefab, 1, transform);
        SpawnUnits();
    }

    public List<Vector2Int> AddPathRequest(Vector3Int start, Vector3Int end, Unit unit)
    {
        //TODO: Do path request queue
        return WorldGrid.GetPath(new Vector2Int(start.x / 2, start.z /2 ), new Vector2Int(end.x /2, end.z /2)).ToList();
    }
    void SpawnUnits(/* input data */)
    {
        for(int i = 0; i < 5; i++)
        {
            GameObject go = m_UnitPool.Rent(true);
            var unit = go.GetComponent<Unit>();
            unit.Initialize(this, playerBase);
            go.transform.position = m_EnemyBase.transform.position;
        }
    }

    public void DestroyUnit(Unit unit)
    {
        m_UnitPool.Return(unit.gameObject);
    }
}
