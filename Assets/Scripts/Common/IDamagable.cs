using UnityEngine;

public interface IDamagable
{
    void TakeDamage(float value, Transform damageSource = null, Vector3 force = default(Vector3));
}
