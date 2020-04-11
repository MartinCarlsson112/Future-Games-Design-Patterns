using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class UnitManager : MonoBehaviour
{
    public GameObject m_NormalUnitPrefab;
    public GameObject m_LargeUnitPrefab;

    ComponentPool<NormalUnit> m_NormalUnitPool;
    ComponentPool<LargeUnit> m_LargeUnitPool;
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

    public struct WaveData
    {
        uint m_TypeOne, m_TypeTwo;
    }
    List<WaveData> m_WaveData;

    private void Start()
    {
        m_NormalUnitPool = new ComponentPool<NormalUnit>(5, m_NormalUnitPrefab);
        m_LargeUnitPool = new ComponentPool<LargeUnit>(5, m_LargeUnitPrefab);
        StartCoroutine(SpawnWave(5, 0, 1.0f));
        //get wave data from world grid
    }

    IEnumerator SpawnWave(uint nTypeOne, uint nTypeTwo, float delayBetweenSpawns)
    {
        for(int i = 0; i < nTypeOne; i++)
        {
            NormalUnit unit = m_NormalUnitPool.Rent(true);
            unit.Initialize(playerBase, this);
            unit.transform.position = m_EnemyBase.transform.position;
            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        for(int i = 0; i < nTypeTwo; i++)
        {
            LargeUnit unit = m_LargeUnitPool.Rent(true);
            unit.Initialize(playerBase, this);
            unit.transform.position = m_EnemyBase.transform.position;
            yield return new WaitForSeconds(delayBetweenSpawns);
        }
    }

    public List<Vector2Int> RequestPath(Vector3Int start, Vector3Int end, NormalUnit unit)
    {
        return WorldGrid.GetPath(new Vector2Int(start.x / 2, start.z /2 ), new Vector2Int(end.x /2, end.z /2)).ToList();
    }
  

    public void DestroyUnit(NormalUnit unit)
    {
        m_NormalUnitPool.Return(unit);
    }
}
