using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SkillSwordWave : PoolableObject
{
    ParticleSystem swordWaveVFX;
    public float autoDestroyTime = 1.5f; 
    public float moveSpeed = 20f;
    public int damage ;
    public Rigidbody _rigidbody;
    public LayerMask enemyLayer;

    private List<IDamageable> damagedTargets = new List<IDamageable>(); 

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (swordWaveVFX == null)
        {
            swordWaveVFX = GetComponentInChildren<ParticleSystem>();
        }
    }

    protected virtual void OnEnable()
    {
        CancelInvoke(nameof(Disable));
        Invoke(nameof(Disable), autoDestroyTime);
    }

    public virtual void Spawn(Vector3 forward, int damage)
    {
        this.damage = damage;
        _rigidbody.velocity = forward * moveSpeed;
        swordWaveVFX.Play();
        damagedTargets.Clear(); // Reset danh sách kẻ địch đã trúng
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null && !damagedTargets.Contains(damageable))
            {
                damageable.TakeDamage(damage);
                damagedTargets.Add(damageable); // Đánh dấu mục tiêu đã trúng
            }
        }
    }

    protected void Disable()
    {
        CancelInvoke(nameof(Disable));
        _rigidbody.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}
