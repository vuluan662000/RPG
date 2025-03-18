using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RangeAttackRadius : AttackRadius
{
    public NavMeshAgent agent;
    public Bullet bulletPrefab;
    public Vector3 bulletSpawnOffset = new Vector3(0,2,0);
    public LayerMask layerMask;
    private ObjectPool bulletPool;

    [SerializeField] private float spherecastRadius = 1f;
    private RaycastHit hit;
    private IDamageable targetDamageable;
    private Bullet bullet;
    private void Start()
    {
        CreateBulletPool();
    }
    public void CreateBulletPool()
    {
        if (bulletPool == null)
        {
            bulletPool = ObjectPool.CreateInstance(bulletPrefab, Mathf.CeilToInt((1 / attackDelay) * bulletPrefab.autoDestroyTime));
        }
    }

    protected override IEnumerator Attack()
    {
        WaitForSeconds wait = new WaitForSeconds(attackDelay);

        yield return wait;

        while (_damageables.Count > 0)
        {
            for(int i = 0;  i < _damageables.Count; i++)
            {
                if (HasLineOfSightTo(_damageables[i].GetTransform()))
                {
                    targetDamageable = _damageables[i];
                    OnAttack?.Invoke(_damageables[i]);
                    agent.enabled = false;
                    //agent.isStopped = true;
                    break;
                }    
            }

            if (targetDamageable != null) 
            {
                PoolableObject poolableObject = bulletPool.GetObject();
                if (poolableObject != null) 
                {
                    bullet = poolableObject.GetComponent<Bullet>();

                    bullet.transform.position = transform.position + transform.forward * 1.5f + bulletSpawnOffset;
                    bullet.transform.rotation = agent.transform.rotation;

                    bullet.Spawn(agent.transform.forward, damage, targetDamageable.GetTransform());
                }
            }
            else
            {
               // agent.enabled = true;
                //agent.isStopped = false;
            }
            yield return wait;

            if (targetDamageable == null || !HasLineOfSightTo(targetDamageable.GetTransform()))
            {
                agent.enabled = true;
               // agent.isStopped = false;
            }
            _damageables.RemoveAll(DisabledDamageable);             
        }

        agent.enabled = true;
       //agent.isStopped = false;
        attackCoroutine = null;

    }

    private bool HasLineOfSightTo(Transform target)
    {
        Vector3 origin = transform.position + bulletSpawnOffset; 
        Vector3 direction = (target.position + bulletSpawnOffset - origin).normalized;
        float maxDistance = sphereCollider.radius; 

        Debug.DrawRay(origin, direction * maxDistance, Color.red, 2.0f);

        if (Physics.SphereCast(origin, spherecastRadius, direction, out hit, maxDistance, layerMask))
        {
            IDamageable damageable;
            if (hit.collider.TryGetComponent<IDamageable>(out damageable))
            {
                bool isCorrectTarget = damageable.GetTransform() == target;
                return isCorrectTarget;
            }
        }
        return false;
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (attackCoroutine == null)
        {
            agent.enabled =true;
        }
    }
}
