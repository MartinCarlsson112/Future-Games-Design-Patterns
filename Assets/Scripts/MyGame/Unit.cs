﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private bool m_HasPath = false;
    private float m_UpdateTimer = 0.0f;
    private float m_UpdateTime = 0.5f;
    private float m_MovementSpeed = 10f;

    public UnitManager m_Manager;

    [SerializeField]
    private List<Vector2Int> m_CurrentPath;

    //TODO: Make into property
    private PlayerBase m_PlayerBase;

    public void Initialize(UnitManager unitManager, PlayerBase playerBase)
    {
        m_Manager = unitManager;
        m_PlayerBase = playerBase;
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

    void RequestPath()
    {
        Vector3Int start = Vector3Int.FloorToInt(transform.position);
        Vector3Int end = Vector3Int.FloorToInt(m_PlayerBase.transform.position);
        m_HasPath = true;
        m_CurrentPath = m_Manager.AddPathRequest(start, end, this);

    }
    bool Approx(Vector3 a, Vector3 b)
    {
        Vector3 v = a - b;
        if (Mathf.Abs(v.x) > 0.1f || Mathf.Abs(v.y) > 0.1f || Mathf.Abs(v.z) > 0.1f)
        {
            return false;
        }
        return true;
    }

    void ExecutePathing()
    {
        if(m_CurrentPath.Count <= 0)
        {
            m_HasPath = false;
            return;
        }
        Vector3 targetPosition = new Vector3(m_CurrentPath[0].x * 2, 0, m_CurrentPath[0].y * 2);
        Vector3 direction = (targetPosition - transform.position).normalized;

        if(direction != Vector3.zero)
        {
            transform.position += direction * Time.deltaTime * m_MovementSpeed;
        }
   
        if (Approx(transform.position, targetPosition))
        {
            m_CurrentPath.RemoveAt(0);
        }
    }

}