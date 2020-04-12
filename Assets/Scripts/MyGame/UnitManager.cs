using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class UnitManager : MonoBehaviour
{
    public event System.Action UnitSpawningFinished; 

    [SerializeField]
    private GameObject m_NormalUnitPrefab;
    [SerializeField]
    private GameObject m_LargeUnitPrefab;
    [SerializeField]
    float m_DelayBetweenSpawns = 1.0f;

    ComponentPool<NormalUnit> m_NormalUnitPool;
    ComponentPool<NormalUnit> m_LargeUnitPool;

    [Inject]
    [HideInInspector]
    public EnemyBase m_EnemyBase;

    [Inject]
    [HideInInspector]
    public PlayerBase playerBase;

    WorldGrid m_WorldGrid;

    [Inject]
    public WorldGrid WorldGrid
    {
        set => m_WorldGrid = value;
        get => m_WorldGrid;
    }

    List<Wave> m_Waves;
    int m_WaveSpawnCounter = 0;


    private void Start()
    {
        m_NormalUnitPool = new ComponentPool<NormalUnit>(15, m_NormalUnitPrefab);
        m_LargeUnitPool = new ComponentPool<NormalUnit>(15, m_LargeUnitPrefab);
        m_Waves = m_WorldGrid.SpawnWaves;   
        Wave wave = m_Waves[m_WaveSpawnCounter];
        StartCoroutine(SpawnWave(wave.TypeOne, wave.TypeTwo, m_DelayBetweenSpawns));
    }

    IEnumerator SpawnWave(uint nTypeOne, uint nTypeTwo, float delayBetweenSpawns)
    {
        for(int i = 0; i < nTypeOne; i++)
        {
            NormalUnit unit = m_NormalUnitPool.Rent(true);
            unit.Initialize(playerBase, this);
            unit.transform.position = new Vector3(m_EnemyBase.transform.position.x, m_EnemyBase.transform.position.y + unit.GetComponentInChildren<Collider>().bounds.extents.y, m_EnemyBase.transform.position.z);
            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        for(int i = 0; i < nTypeTwo; i++)
        {
            NormalUnit unit = m_LargeUnitPool.Rent(true);
            unit.Initialize(playerBase, this);
            unit.transform.position = new Vector3(m_EnemyBase.transform.position.x, m_EnemyBase.transform.position.y + unit.GetComponentInChildren<Collider>().bounds.extents.y, m_EnemyBase.transform.position.z);
            yield return new WaitForSeconds(delayBetweenSpawns);
        }
        m_WaveSpawnCounter++;
        if(m_WaveSpawnCounter >= m_Waves.Count)
        {
            UnitSpawningFinished?.Invoke();
        }
        else
        {
            Wave wave = m_Waves[m_WaveSpawnCounter];
            StartCoroutine(SpawnWave(wave.TypeOne, wave.TypeTwo, delayBetweenSpawns));
        }
    }

    public List<Vector3> RequestPath(Vector3 start, Vector3 end, NormalUnit unit)
    {
        return WorldGrid.GetPath(start, end).ToList();
    }
  
    public void DestroyUnit(NormalUnit unit)
    {
        if(unit.UnitType == UnitType.Standard)
        {
            m_NormalUnitPool.Return(unit);
        }
        else if(unit.UnitType == UnitType.Big)
        {
            m_LargeUnitPool.Return(unit);
        }
    }
}
