using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBullet
{
    //effect
}

public class Bullet : MonoBehaviour
{
    private float m_DamageAmount = 1;
    private Collider m_Collider;
    private float m_BulletSpeed = 10.0f;
    private Transform m_Target;
    private Vector3 m_TargetPosition;
    public Transform Target
    {
        get => m_Target;
        set { m_TargetPosition = value.position; m_Target = value; }
    }

    TowerManager m_TowerManager;
    public TowerManager TowerManager
    {
        get => m_TowerManager;
        set => m_TowerManager = value;
    }

    private BulletType m_BulletType;

    public BulletType BulletType
    {
        get => m_BulletType;

    }

    public void Initialize(TowerManager towerManager, BulletType bulletType)
    {
        m_TowerManager = towerManager;
        m_BulletType = BulletType;
        m_Collider = GetComponent<Collider>();
    }

    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_TargetPosition, m_BulletSpeed * Time.deltaTime);
    }

    public void UpdateBullet()
    {
        if(gameObject.activeSelf)
        {
            if(m_Target)
            {
                MoveToTarget();
            }

            var overlaps = Physics.OverlapBox(transform.position, m_Collider.bounds.extents);
            foreach(var overlap in overlaps)
            {
                var unitComp = overlap.transform.root.GetComponent<NormalUnit>();
                if(unitComp)
                {
                    unitComp.TakeDamage(m_DamageAmount);
                    m_TowerManager.DestroyBullet(this);
                }
            }
        }
    }
}
