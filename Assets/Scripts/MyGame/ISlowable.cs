public interface ISlowable
{
    void ApplySlow(float amount, float duration);
    void ClearSlow();
}