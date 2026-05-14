using System.Collections;
using UnityEngine;
using System;

public abstract class BugController : MonoBehaviour
{

    [SerializeField] protected float minRotateSpeed = 0.01f;
    [SerializeField] protected Animator animator;
    [SerializeField] protected int currentHP = 0;

    public Vector2 velocity;
    protected float wanderAngle;
    protected Vector2 velocitySmoothRef;
    protected bool isDying;

    protected virtual void Update()
    {
            if (isDying) return;
    
            Move();
    }

    protected void LookAt(Vector3 target) {
        if (velocity.sqrMagnitude < minRotateSpeed * minRotateSpeed)
        {
            return;
        }

        float lookAngle = Util.AngleBetweenTwoPoints(transform.position, target) - 90;

        transform.eulerAngles = new Vector3(0, 0, lookAngle);
    }

    protected virtual void Move()
    {
        
    }

    protected void InitializeHP(int hp)
    {
        currentHP = Mathf.Max(0, hp);
    }

    public bool OnBite(int damage)
    {
        return TakeDamage(damage);
    }

    public bool TakeDamage(int damage)
    {
        if (isDying)
        {
            return false;
        }

        if (damage <= 0)
        {
            return false;
        }

        currentHP -= damage;
        if (currentHP > 0)
        {
            return false;
        }

        currentHP = 0;
        return true;
    }

    public void Die()
    {
        if (isDying)
        {
            return;
        }

        isDying = true;
        animator.SetTrigger("isDead");
        velocity = Vector2.zero;
        StartCoroutine(DieRoutine());
    }

    protected IEnumerator DieRoutine()
    {
        if (animator == null)
        {
            Destroy(gameObject);
            yield break;
        }

        animator.Play("FlyDie", 0, 0f);

        // Play 직후에는 상태 정보가 갱신되지 않아 다음 프레임까지 기다린다.
        yield return null;

        while (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("FlyDie") && stateInfo.normalizedTime >= 1f)
            {
                break;
            }

            yield return null;
        }

        Managers.Resource.Destroy(gameObject);
    }
}
