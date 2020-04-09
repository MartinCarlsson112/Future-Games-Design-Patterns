using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField]
    private int m_Health = 10;

    public void TakeDamage(int damage)
    {
        m_Health -= damage;
        if(m_Health <= 0)
        {
            m_Health = 0;
            //Fire game lost event
        }
    }
}
