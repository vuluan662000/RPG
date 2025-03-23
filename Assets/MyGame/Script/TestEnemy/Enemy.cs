using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class Enemy : PoolableObject, IDamageable
{
    public AttackRadius attackRadius;
    [SerializeField] RangeAttackRadius rangeAttackRadius;
    public Animator animator;
    public EnemyMovement movement;
    public NavMeshAgent agent;
    public int health;
    public Slider healthSlider;
    public delegate void DeathEvent(Enemy enemy);
    public DeathEvent OnDie;

    private Coroutine lookCoroutine;
    private const string ATTACK_TRIGGER = "Attack";

    private void Awake()
    {
        attackRadius.OnAttack += OnAttack;
    }
    private void OnAttack(IDamageable Target)
    {
        animator.SetTrigger(ATTACK_TRIGGER);

        if (lookCoroutine != null)
        {
            StopCoroutine(lookCoroutine);
        }

        lookCoroutine = StartCoroutine(LookAt(Target.GetTransform()));
    }
    private IEnumerator LookAt(Transform Target)
    {
        Quaternion lookRotation = Quaternion.LookRotation(Target.position - transform.position);
        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);

            time += Time.deltaTime * 2;
            yield return null;
        }

        transform.rotation = lookRotation;
    }
    public override void OnDisable()
    {
        base.OnDisable();

        agent.enabled = false;
        OnDie = null;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthSlider.value = health;

        if (health <= 0)
        {
            OnDie?.Invoke(this);
            gameObject.SetActive(false); // Vô hiệu hóa enemy khi chết
        }
    }

    public void EventShooting()
    {
        rangeAttackRadius.TestShot();
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
