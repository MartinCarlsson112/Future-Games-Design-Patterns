using UnityEngine;
public abstract class UnitBase : MonoBehaviour, IDamageable, ISlowable
{
    public event System.Action UnitDied;

    public abstract void ApplySlow(float amount, float duration);
    public abstract void TakeDamage(float damageAmount);
    public abstract void Initialize(PlayerBase playerBase, UnitManager unitManager);
    public abstract int GetRemainingStepsOnPath();
    public abstract void Destroy();

    public void InvokeUnitDied()
    {
        UnitDied?.Invoke();
    }
}