using UnityEngine;

public abstract class UnitBase : MonoBehaviour, IDamageable, ISlowable
{
    public abstract void ApplySlow(float amount, float duration);
    public abstract void ClearSlow();
    public abstract void TakeDamage(float damageAmount);
    public abstract void Initialize(PlayerBase playerBase, UnitManager unitManager);
    public abstract int GetRemainingStepsOnPath();

}