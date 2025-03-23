using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSnowStorm : PoolableObject
{
    ParticleSystem snowStormVFX;
    public float autoDestroyTime = 4f;
    public int damage;
    public LayerMask enemyLayer;

    private HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
    private Dictionary<IDamageable, float> damageTimers = new Dictionary<IDamageable, float>();
    private float damageInterval = 1f; // Gây damage 

    private void Awake()
    {
        if (snowStormVFX == null)
        {
            snowStormVFX = GetComponentInChildren<ParticleSystem>();
        }
    }

    protected virtual void OnEnable()
    {
        CancelInvoke(nameof(Disable));
        Invoke(nameof(Disable), autoDestroyTime);
    }

    public virtual void Spawn(Vector3 position, int damage)
    {
        this.damage = damage;
        transform.position = position;
        snowStormVFX.Play();
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (!damageTimers.ContainsKey(damageable))
                {
                    damageTimers[damageable] = Time.time;
                    damageable.TakeDamage(damage);
                   // Debug.Log($"First hit on {other.name} - Damage: {damage}");
                }
                else if (Time.time - damageTimers[damageable] >= damageInterval)
                {
                    damageTimers[damageable] = Time.time;
                    damageable.TakeDamage(damage);
                   // Debug.Log($"Repeated hit on {other.name} - Damage: {damage}");
                }
            }
        }
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damagedTargets.Remove(damageable);
                damageTimers.Remove(damageable);
            }
        }
    }

    protected void Disable()
    {
        CancelInvoke(nameof(Disable));
        gameObject.SetActive(false);
    }
}
