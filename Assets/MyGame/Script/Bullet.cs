using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Bullet : PoolableObject
{
    public float autoDestroyTime = 5f;
    public float moveSpeed = 2f;
    public int damage = 5;
    public Rigidbody rigibody;

    protected Transform _target;
    protected const string DISABLE_METHOD_NAME = "Disable";

    private void Awake()
    {
        rigibody = GetComponent<Rigidbody>();
    }

    protected virtual void OnEnable()
    {
        CancelInvoke(DISABLE_METHOD_NAME);
        Invoke(DISABLE_METHOD_NAME, autoDestroyTime);
    }

    public virtual void Spawn(Vector3 forward, int damage, Transform target)
    {
        this.damage = damage;
        this._target = target;
        rigibody.AddForce(forward * moveSpeed, ForceMode.VelocityChange);
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        IDamageable damageable;
        if (other.TryGetComponent<IDamageable>(out damageable))
        {
            damageable.TakeDamage(damage);
        }
        Disable();
    }
    protected void Disable()
    {
        CancelInvoke(DISABLE_METHOD_NAME);
        rigibody.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}
