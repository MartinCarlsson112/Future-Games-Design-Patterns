using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class NormalUnit : UnitBase
{
    [SerializeField]
    private UnitType m_UnitType;

    [SerializeField]
    private float m_MovementSpeed;
    [SerializeField]
    private float m_MaxHealth;
    [SerializeField]
    private float m_DamageAmount;

    private bool m_HasPath = false;
    private float m_UpdateTime = 0.5f;
    private float m_UpdateTimer = 0;
    private float m_Health = 0;
    private float m_CurrentMovementSpeed;
    private float m_MinDistanceToStep = 0.1f;
    private float m_Offset = 0;
    private List<Vector3> m_CurrentPath;
    private UnitManager m_Manager;
    private PlayerBase m_PlayerBase;
    private Collider m_Collider;


    public UnitType UnitType => m_UnitType;



    private void Start()
    {
        m_Collider = GetComponentInChildren<Collider>();
        m_Health = m_MaxHealth;
        m_CurrentMovementSpeed = m_MovementSpeed;
    }

    private void Update()
    {
        if(!m_HasPath)
        {
            m_UpdateTimer -= Time.deltaTime;
            if(m_UpdateTimer < 0)
            {
                RequestPath();
                m_UpdateTimer = m_UpdateTime;
            }
        }
        else
        {
            ExecutePathing();
        }   
    }

    private void ExecutePathing()
    {
        if(m_CurrentPath.Count <= 0)
        {
            var overlaps = Physics.OverlapBox(transform.position, m_Collider.bounds.extents);
            foreach(var overlap in overlaps)
            {
                if(overlap.gameObject == m_PlayerBase.gameObject)
                {
                    m_PlayerBase.TakeDamage(m_DamageAmount);
                    Destroy();
                }
            }
            return;
        }

        Vector3 targetPosition = m_CurrentPath[0] + Vector3.up * m_Offset;
        Vector3 direction = (targetPosition - transform.position).normalized;

        if(direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            transform.position += direction * Time.deltaTime * m_CurrentMovementSpeed;
        }
        else
        {
            m_CurrentPath.RemoveAt(0);
        }

        Vector3 directionAfterMove = (targetPosition - transform.position).normalized;
        if(Vector3.Dot(directionAfterMove, direction) <= 0)
        {
            m_CurrentPath.RemoveAt(0);
        }
    }

    public override void Destroy()
    {
        InvokeUnitDied();
        m_Manager.DestroyUnit(this);
        m_HasPath = false;
        m_UpdateTimer = 0;
        m_CurrentPath.Clear();
        m_Health = m_MaxHealth;
    }

    public override int GetRemainingStepsOnPath()
    {
        if(m_CurrentPath != null)
        {
            return m_CurrentPath.Count;
        }
        return -1;
    }


    public override void TakeDamage(float damageAmount)
    {
        m_Health -= damageAmount;
        if(m_Health <= 0)
        {
            Destroy();
        }
    }
       
    void RequestPath()
    {
        Vector3Int start = Vector3Int.FloorToInt(transform.position);
        Vector3Int end = Vector3Int.FloorToInt(m_PlayerBase.transform.position);
        m_HasPath = true;
        m_CurrentPath = m_Manager.RequestPath(start, end, this);
    }

    IEnumerator SlowTimer(float slowAmount, float duration)
    {
        m_CurrentMovementSpeed = m_MovementSpeed * slowAmount;
        yield return new WaitForSeconds(duration);
        m_CurrentMovementSpeed = m_MovementSpeed;
    }

    public override void ApplySlow(float amount, float duration)
    {
        StopCoroutine("SlowTimer");
        StartCoroutine(SlowTimer(amount, duration));
    }

    public override void Initialize(PlayerBase playerBase, UnitManager unitManager)
    {
        m_Manager = unitManager;
        m_PlayerBase = playerBase;
        m_Offset = GetComponentInChildren<Collider>().bounds.extents.y;
    }
}