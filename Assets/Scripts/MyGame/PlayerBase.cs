using UnityEngine;

public class PlayerBase : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float m_Health = 20;

    public void TakeDamage(float damage)
    {
        m_Health -= damage;
        if(m_Health <= 0)
        {
            m_Health = 0;
        }
    }
}
