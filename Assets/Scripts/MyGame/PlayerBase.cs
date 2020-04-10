using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField]
    private float m_Health = 20;

    public void TakeDamage(float damage)
    {
        m_Health -= damage;
        if(m_Health <= 0)
        {
            m_Health = 0;
            //Fire game lost event
        }
    }
}
