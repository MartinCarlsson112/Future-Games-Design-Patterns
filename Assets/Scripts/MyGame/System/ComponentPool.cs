using System.Collections.Generic;
using UnityEngine;

public class ComponentPool<T> : Tools.IPool<T> where T : MonoBehaviour
{
    private uint m_InitSize;
    private uint m_ExpandBy;
    private GameObject m_Prefab;
    private Transform m_Parent;

    private Stack<T> m_FreeStack = new Stack<T>();
    private List<T> m_Objects = new List<T>();

    public List<T> List
    {
        get => m_Objects;
    }

    public ComponentPool(uint initSize, GameObject prefab, uint expandBy = 1, Transform parent = null)
    {
        if(initSize > 0)
        {
            m_InitSize = initSize;
        }
        else
        {
            m_InitSize = 1;
        }
        if(expandBy > 0)
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
        for(int i = 0; i < count; i++)
        {
            GameObject obj = Object.Instantiate(m_Prefab, m_Parent);
            obj.SetActive(false);
            var component = obj.GetComponent(typeof(T));
            if(!component)
            {
                Debug.LogWarning("Object subject to component pooling did not have required component! Object: " + obj.name);
                GameObject.Destroy(obj);
            }
            m_Objects.Add(component as T);
            m_FreeStack.Push(component as T);
        }
    }

    public T Rent(bool returnActive)
    {
        if(m_FreeStack.Count == 0)
        {
            Expand(m_ExpandBy);
        }
        T returnComponent = m_FreeStack.Pop();
        returnComponent.gameObject.SetActive(returnActive);
        return returnComponent;
    }

    public void Return(T returnedObject)
    {
        if(m_Objects.Contains(returnedObject) && !m_FreeStack.Contains(returnedObject))
        {
            returnedObject.gameObject.SetActive(false);
            m_FreeStack.Push(returnedObject);
        }
    }
}
