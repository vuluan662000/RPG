using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(SphereCollider))]
public class AttackRadius : MonoBehaviour
{
    public NavMeshAgent agent;
    public SphereCollider sphereCollider;
    protected List<IDamageable> _damageables =  new List<IDamageable>();   
    public int damage = 10;
    public float attackDelay = 0.5f;
    public float waitAnimationEnd = 1.02f;
    public delegate void AttackEvent(IDamageable target);

    public AttackEvent OnAttack;
    protected Coroutine attackCoroutine;

    protected virtual void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null )
        {
            _damageables.Add(damageable);

            if(attackCoroutine != null) StopCoroutine(attackCoroutine);
            attackCoroutine = StartCoroutine(Attack());   
        }    
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            _damageables.Remove(damageable);

            if (_damageables.Count == 0)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    protected virtual IEnumerator Attack()
    {
        WaitForSeconds wait = new WaitForSeconds(attackDelay);
        WaitForSeconds animationEnd = new WaitForSeconds(waitAnimationEnd);

        //yield return wait;
        yield return animationEnd;
        IDamageable closeseDamageable = null;
        float closeseDistance = float.MaxValue;

        while (_damageables.Count > 0) 
        {
            for (int i = 0; i < _damageables.Count; i++) 
            {
                Transform damageableTranform = _damageables[i].GetTransform();
                float distance = Vector3.Distance(transform.position, damageableTranform.position);
                if (distance < closeseDistance) 
                {
                    closeseDistance = distance;
                    closeseDamageable = _damageables[i]; 
                }
            }
            if (closeseDamageable != null) 
            {
                OnAttack?.Invoke(closeseDamageable);
                yield return wait;

                closeseDamageable.TakeDamage(damage);
               

            }
            closeseDamageable = null ;
            closeseDistance = float.MaxValue;
            _damageables.RemoveAll(DisabledDamageable);
        }

        attackCoroutine = null;
    }

    protected bool DisabledDamageable(IDamageable damageable)
    {
        return _damageables != null && !damageable.GetTransform().gameObject.activeSelf;
    }
}
