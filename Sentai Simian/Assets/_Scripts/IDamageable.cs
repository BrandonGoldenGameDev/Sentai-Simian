using System;

public interface IDamageable
{
    /// <summary>
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="onDamaged">Triggered when the damageable object is damaged.</param>
    /// <param name="onDeath">Triggered when the damageable object is killed.</param>
    public void TakeDamage(float damage, Action<float> onDamaged, Action onDeath);
}
