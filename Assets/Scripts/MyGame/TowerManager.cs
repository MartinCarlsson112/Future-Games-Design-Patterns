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
    public Tower[] Towers {
        get => m_Towers;
        set => m_Towers = value;
    }

    [SerializeField]
    private GameObject m_BulletPrefab;

    [SerializeField]
    private GameObject m_FreezingBulletPrefab;

    private GameObjectPool[] m_BulletPools = new GameObjectPool[(int)BulletType.COUNT];
    private void Start()
    {
        m_BulletPools[(int)BulletType.Normal] = new GameObjectPool(5, m_BulletPrefab);
        foreach(var bullet in m_BulletPools[(int)BulletType.Normal].List)
        {
            bullet.GetComponent<Bullet>().Initialize(this, BulletType.Normal);
        }
        //m_BulletPools[(int)BulletType.Freezing] = new GameObjectPool(5, m_FreezingBulletPrefab);

    }

    private void Update()
    {
        foreach (var tower in m_Towers)
        {
            tower.UpdateTower(this);
        }

        var normalBulletArray = m_BulletPools[(int)BulletType.Normal].List;
       // var freezingBulletArray = m_BulletPools[(int)BulletType.Freezing].List;
       

        foreach(var bullet in normalBulletArray)
        {
            bullet.GetComponent<Bullet>().UpdateBullet();
        }

        //foreach(var bullet in freezingBulletArray)
        //{
        //    bullet.GetComponent<Bullet>().UpdateBullet();
        //}
    }

    public Bullet RequestBullet(BulletType type)
    {
        GameObject bullet =  m_BulletPools[(int)type].Rent(false);

        var bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.Initialize(this, type);

        return bullet.GetComponent<Bullet>();
    }

    public void DestroyBullet(Bullet bullet)
    {
        m_BulletPools[(int)bullet.BulletType].Return(bullet.gameObject);
    }
}
