using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField]
    private Tower[] m_Towers;

    [Inject]
    public Tower[] Towers {
        get => m_Towers;
        set => m_Towers = value;
    }

    [SerializeField]
    private GameObject m_BulletPrefab;

    private ComponentPool<Bullet>[] m_BulletPools = new ComponentPool<Bullet>[(int)BulletType.COUNT];
    private void Start()
    {
        m_BulletPools[(int)BulletType.Normal] = new ComponentPool<Bullet>(5, m_BulletPrefab);
        foreach(var bullet in m_BulletPools[(int)BulletType.Normal].List)
        {
            bullet.GetComponent<Bullet>().Initialize(this, BulletType.Normal);
        }
        
    }

    private void Update()
    {
        foreach (var tower in m_Towers)
        {
            tower.UpdateTower(this);
        }

        var normalBulletArray = m_BulletPools[(int)BulletType.Normal].List;

        foreach(var bullet in normalBulletArray)
        {
            bullet.GetComponent<Bullet>().UpdateBullet();
        }
    }

    public Bullet RequestBullet(BulletType type)
    {
        Bullet bullet = m_BulletPools[(int)type].Rent(false);

        var bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.Initialize(this, type);

        return bullet.GetComponent<Bullet>();
     }

    public void DestroyBullet(Bullet bullet)
    {
        m_BulletPools[(int)bullet.BulletType].Return(bullet);
    }
}
