using UnityEngine;

public enum BulletType
{
    Normal,
    Freezing,
    COUNT
}

public abstract class BulletBase : MonoBehaviour
{
    private float m_DamageAmount;
    private float m_BulletSpeed;

    private Collider m_Collider;
    private Vector3 m_TargetPosition;
    private TowerManager m_TowerManager;
    private BulletType m_BulletType;

    public float DamageAmount
    {
        get => m_DamageAmount;
        set => m_DamageAmount = value;
    }

    public float BulletSpeed
    {
        get => m_BulletSpeed;
        set => m_BulletSpeed = value;
    }

    public TowerManager TowerManager => m_TowerManager;
    public BulletType BulletType => m_BulletType;
    public Vector3 TargetPosition
    {
        get => m_TargetPosition;
        set => m_TargetPosition = value;
    }

    public Collider Collider => m_Collider;

    public void Initialize(TowerManager towerManager, BulletType bulletType)
    {
        m_TowerManager = towerManager;
        m_BulletType = BulletType;
        m_Collider = GetComponent<Collider>();
    }

    public abstract void UpdateBullet();
}
