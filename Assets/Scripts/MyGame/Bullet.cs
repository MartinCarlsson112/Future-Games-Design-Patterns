using UnityEngine;

public class Bullet : BulletBase
{
    private float m_ExplodeRange;
   
    public void Shoot(Vector3 startPosition, Vector3 targetPosition, float damageAmount, float bulletSpeed, float explodeRange)
    {
        transform.position = startPosition;
        TargetPosition = targetPosition;
        DamageAmount = damageAmount;
        BulletSpeed = bulletSpeed;
        m_ExplodeRange = explodeRange;
        gameObject.SetActive(true);
    }

    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, TargetPosition, BulletSpeed * Time.deltaTime);
        if(transform.position == TargetPosition)
        {
            var overlaps = Physics.OverlapBox(transform.position, Collider.bounds.extents * m_ExplodeRange);
            foreach(var overlap in overlaps)
            {
                var unitComp = overlap.transform.root.GetComponent<NormalUnit>();
                if(unitComp)
                {
                    unitComp.TakeDamage(DamageAmount);
                }
            }
            TowerManager.DestroyBullet(this);
        }
    }

    public override void UpdateBullet()
    {
        if(gameObject.activeSelf)
        {
            MoveToTarget();
        }
    }
}
