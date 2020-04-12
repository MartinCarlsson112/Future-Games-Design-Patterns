using UnityEngine;
using System.Collections;
public class Tower : MonoBehaviour
{
    [SerializeField]
    private float m_TowerRange = 5.0f;
    [SerializeField]
    private float m_BulletDamageAmount = 1.0f;
    [SerializeField]
    private float m_ExplodeRange = 4;
    [SerializeField]
    private float m_BulletSpeed = 15.0f;
    [SerializeField]
    private BulletType m_BulletType = BulletType.Normal;
    [SerializeField]
    private float m_ShootTimer = 1.0f;  
    [SerializeField]
    private float m_SeekTimer = 0.5f;
    [SerializeField]
    private float m_ShootAccu = 0.0f;

    private bool m_Seeking = false;
    private uint m_BufferSize = 24;
    private int m_TargetCount;
    private Collider[] m_Targets;
    private UnitBase m_CurrentTarget;

    IEnumerator Seek()
    {
        m_Seeking = true;
        while(true)
        {
            m_TargetCount = Physics.OverlapSphereNonAlloc(transform.position, m_TowerRange, m_Targets, LayerMask.GetMask("Unit"));
            yield return new WaitForSeconds(m_SeekTimer);
        }
    }

    private void Start()
    {
        m_Targets = new Collider[m_BufferSize];
    }

    UnitBase PickBestSuitableTarget()
    {
        int shortestPathRemaining = int.MaxValue;
        int bestIndex = -1;
        for(int i = 0; i < m_TargetCount; i++)
        {
            if(m_Targets[i].transform.root.gameObject.activeSelf)
            {
                var unitComp = m_Targets[i].transform.root.GetComponent<UnitBase>();
                if(unitComp)
                {
                    int stepsRemaining = unitComp.GetRemainingStepsOnPath();
                    if(stepsRemaining != -1 && stepsRemaining < shortestPathRemaining)
                    {
                        shortestPathRemaining = stepsRemaining;
                        bestIndex = i;
                    }
                }
            }
        }
        return bestIndex != -1 ? m_Targets[bestIndex].transform.root.GetComponent<UnitBase>() : null;
    }

    void GetTarget()
    {
        if(!m_Seeking)
        {
            StartCoroutine("Seek");
        }
        m_CurrentTarget = PickBestSuitableTarget();

        if(m_CurrentTarget)
        {
            StopCoroutine("Seek");
            m_Seeking = false;
            m_CurrentTarget.UnitDied += DropTarget;
        }
    }

    void DropTarget()
    {
        m_CurrentTarget.UnitDied -= DropTarget;
        m_CurrentTarget = null;
        StartCoroutine("Seek");
    }

    void ChangeTarget()
    {
        if(m_CurrentTarget)
        {
            m_CurrentTarget.UnitDied -= DropTarget;
        }
        GetTarget();
    }

    bool CanShoot()
    {
        return m_ShootAccu >= m_ShootTimer;
    }

    public void UpdateTower(TowerManager towerManager)
    {
        m_ShootAccu += Time.deltaTime;
        if(m_CurrentTarget)
        {
            if(Vector3.Distance(transform.position, m_CurrentTarget.transform.position) > m_TowerRange)
            {
                ChangeTarget();
                return;
            }

            if(CanShoot())
            {
                m_ShootAccu = 0;
                Vector3 lookDir = (new Vector3(m_CurrentTarget.transform.position.x, 0, m_CurrentTarget.transform.position.z) - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(lookDir != Vector3.zero ? lookDir : Vector3.forward);

                if(m_BulletType == BulletType.Freezing)
                {
                    m_CurrentTarget.ApplySlow(0.5f, 2.0f);
                }
                else
                {
                    var bullet = towerManager.RequestBullet(m_BulletType);
                    bullet.Shoot(transform.position, m_CurrentTarget.transform.position, m_BulletDamageAmount, m_BulletSpeed, m_ExplodeRange);
                }
            }
        }
        else
        {
            GetTarget();
        }
    }
}
