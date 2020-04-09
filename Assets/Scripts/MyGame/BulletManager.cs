using UnityEngine;

public class BulletManager
{
    //TODO: Component pool
    GameObjectPool m_BulletPool;
    private uint m_InitBullets;
    private uint m_ExpandBulletsBy;
    GameObject m_BulletPrefab;
    Transform m_Parent;

    public BulletManager(uint initBullets, GameObject bulletPrefab, uint expandBulletsBy = 1, Transform parent = null)
    {
        m_InitBullets = initBullets;
        m_ExpandBulletsBy = expandBulletsBy;
        m_Parent = parent;
        m_BulletPrefab = bulletPrefab;
        m_BulletPool = new GameObjectPool(m_InitBullets, m_BulletPrefab, m_ExpandBulletsBy, m_Parent);
    }


    GameObject RentBullet()
    {
        return m_BulletPool.Rent(true);
    }

    void DestroyBullet(Bullet bullet)
    {
        m_BulletPool.Return(bullet.gameObject);
    }
}
