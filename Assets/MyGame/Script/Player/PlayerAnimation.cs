using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
     public Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }
    public void MoveAnimation()
    {
        _animator.SetFloat("Speed", 1);
    }
    public void IdleAnimation()
    {
        _animator.SetFloat("Speed", 0);
    }    
    public void FallAnimation(bool isFall)
    {
        _animator.SetBool("AirBone", isFall);
    } 
    public void AttackAnimation()
    {
        _animator.SetTrigger("Attack");
    }
    public void DeadAnimation()
    {
        _animator.SetTrigger("Dead");
    }    
    public float GetAttackAnimationLength()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }
    public void AttackAnimationCombo(int comboIndex)
    {
        _animator.SetInteger("ComboIndex", comboIndex);
        _animator.SetTrigger("Attack");
    }

}
