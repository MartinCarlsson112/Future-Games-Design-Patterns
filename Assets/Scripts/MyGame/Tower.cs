using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Tower : MonoBehaviour
{
    BulletType m_BulletType;

    bool m_Seeking = false;

    float m_SeekTimer = 0.5f;
    float m_TowerRange = 5.0f;
    uint m_BufferSize = 24;
    int m_TargetCount;

    float m_ShootTimer = 1.0f;
    float m_ShootAccu = 0.0f;


    Collider[] m_Targets;
    Transform m_CurrentTarget;

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

    Transform PickBestSuitableTarget()
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
        return bestIndex != -1 ? m_Targets[bestIndex].transform : null;
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
            //Subscribe to target ondestroy
        }
    }

    void ChangeTarget()
    {
        if(m_CurrentTarget)
        {
            //Unsubscribe
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
        if(m_CurrentTarget && m_CurrentTarget.transform.root.gameObject.activeSelf)
        {
            if(Vector3.Distance(transform.position, m_CurrentTarget.position) > m_TowerRange)
            {
                ChangeTarget();
            }

            if(CanShoot())
            {
                m_ShootAccu = 0;
                var bullet = towerManager.RequestBullet(m_BulletType);
                bullet.transform.position = transform.position;
                bullet.Target = m_CurrentTarget;
                bullet.gameObject.SetActive(true);
            }
        }
        else
        {
            GetTarget();
        }
    }
}
