using UnityEngine;
using System.Collections.Generic;

public enum BulletType
{
    Normal,
    Freezing,
    COUNT
}


public class TowerManager : MonoBehaviour
{
    private Tower[] m_Towers;

    [Inject]
    public Tower[] Towers{ get; set;}

    private GameObjectPool[] m_BulletPools = new GameObjectPool[(int)BulletType.COUNT];

    private void Start()
    {
        m_BulletPools[(int)BulletType.Normal] = new GameObjectPool(5, new GameObject());
    }

    private void Update()
    {
        foreach (var tower in m_Towers)
        {
            tower.UpdateTower(this);
        }
    }

    public Bullet RequestShoot(Vector3 start, Vector3 end, BulletType type)
    {
        GameObject bullet =  m_BulletPools[(int)type].Rent(false);
        bullet.transform.position = start;
        bullet.SetActive(true);
        return bullet.GetComponent<Bullet>();
    }
}
