using System.Collections;
using UnityEngine;

public class BugController : MonoBehaviour
{
    [SerializeField] private BugData data;
    [SerializeField] private float minRotateSpeed = 0.01f;
    [SerializeField] private Animator animator;

    public Vector2 velocity;
    private float wanderAngle;
    private Vector2 velocitySmoothRef;
    private bool isDying;

    void Awake() {
        Managers.TrapLogic.AddBug(gameObject);
    }

    void Update()
    {
        if (isDying) return;

        if (data == null) return;

        Vector2 wanderForce = GetWanderVector();
        Vector2 attractionForce = GetAttractionVector();
        Vector2 totalForce =
            wanderForce * data.WanderWeight +
            attractionForce * data.AttractionWeight;

        Vector2 targetVelocity = Vector2.ClampMagnitude(
            velocity + totalForce * Time.deltaTime,
            data.Speed
        );

        // 이동 속도를 부드럽게 보간해 떨림을 줄인다.
        velocity = Vector2.SmoothDamp(
            velocity,
            targetVelocity,
            ref velocitySmoothRef,
            data.AccelerationSmoothTime
        );
        velocity = Vector2.ClampMagnitude(velocity, data.Speed);

        transform.position += (Vector3)(velocity * Time.deltaTime);
        LookAt(transform.position + (Vector3)velocity);
    }

    void LookAt(Vector3 target) {
        if (velocity.sqrMagnitude < minRotateSpeed * minRotateSpeed)
        {
            return;
        }

        float lookAngle = AngleBetweenTwoPoints(transform.position, target) - 90;

        transform.eulerAngles = new Vector3(0, 0, lookAngle);
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg;
    }

    Vector2 GetWanderVector()
    {
        wanderAngle += Random.Range(-data.WanderJitter, data.WanderJitter);
        wanderAngle = Mathf.Repeat(wanderAngle, Mathf.PI * 2f);

        Vector2 wanderTarget = new Vector2(
            Mathf.Cos(wanderAngle),
            Mathf.Sin(wanderAngle)
        ) * data.WanderRadius;

        Vector2 desiredVelocity = (wanderTarget - velocity).normalized * data.Speed;
        return desiredVelocity - velocity;
    }

    Vector2 GetAttractionVector()
    {
        if (Managers.TrapLogic.traps == null)
        {
            return Vector2.zero;
        }

        Vector2 currentPos = transform.position;
        Vector2 attractionVector = Vector2.zero;

        foreach (var trap in Managers.TrapLogic.traps)
        {
            if (trap == null)
            {
                continue;
            }
            TrapController trapController = trap.GetComponent<TrapController>();

            Vector2 trapPos = trap.transform.position;
            float distance = Vector2.Distance(currentPos, trapPos);
            float range = trapController.AttractionRange;

            if (range <= 0f || distance > range)
            {
                continue;
            }

            Vector2 direction = (trapPos - currentPos).normalized;
            float falloff = 1f - (distance / range);
            attractionVector += direction * falloff * trapController.AttractionStrength;
        }

        return attractionVector;
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

    private IEnumerator DieRoutine()
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
