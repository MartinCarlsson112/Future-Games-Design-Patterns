using System.Collections.Generic;
using UnityEngine;


public class GameObjectPool : Tools.IPool<GameObject>
{
    private uint m_InitSize;
    private uint m_ExpandBy;
    private GameObject m_Prefab;
    private Transform m_Parent;

    private Stack<GameObject> m_FreeStack = new Stack<GameObject>();
    private List<GameObject> m_Objects = new List<GameObject>();

    public List<GameObject> List
    {
        get => m_Objects;
    }

    public GameObjectPool(uint initSize,  GameObject prefab, uint expandBy = 1, Transform parent = null)
    {
        if (initSize > 0)
        {
            m_InitSize = initSize;
        }
        else
        {
            m_InitSize = 1;
        }
        if (expandBy > 0)
        {
            m_ExpandBy = expandBy;
        }
        else
        {
            m_ExpandBy = 1;
        }
        m_Parent = parent;
        m_Prefab = prefab;
        m_Prefab.SetActive(false);
        Expand(m_InitSize);
    }

    private void Expand(uint count)
    {
        for(int i =0; i < count; i++)
        {
            GameObject obj = Object.Instantiate(m_Prefab, m_Parent);
            obj.SetActive(false);
            m_Objects.Add(obj);
            m_FreeStack.Push(obj);
        }
    }

    public GameObject Rent(bool returnActive)
    {
        if (m_FreeStack.Count == 0)
        {
            Expand(m_ExpandBy);
        }
        GameObject returnObject = m_FreeStack.Pop();
        returnObject.SetActive(returnActive);
        return returnObject;
    }

    public void Return(GameObject returnedObject)
    {
        if(m_Objects.Contains(returnedObject) && !m_FreeStack.Contains(returnedObject))
        {
            returnedObject.SetActive(false);
            m_FreeStack.Push(returnedObject);
        }
    }
}
