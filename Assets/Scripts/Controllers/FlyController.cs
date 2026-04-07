using UnityEngine;

public class FlyController : MonoBehaviour
{
    // 이동 관련
    public float speed = 3f;
    public Vector2 velocity;
    private Vector2 wanderTarget;

    // 파리지옥 위치
    private Vector2 trapPos = Vector2.zero;
    public Transform flytrapTransform;  // Inspector에서 파리지옥 GameObject 할당

    // Wandering 파라미터
    public float wanderRadius = 1f;
    public float wanderDistance = 1.5f;
    public float wanderJitter = 0.3f;
    private float wanderAngle;

    // 유혹 힘 파라미터
    public float attractionRange = 10f;  // 이 범위 안에서만 유혹
    public float attractionStrength = 0.5f;

    // 가중치
    public float wanderWeight = 0.7f;
    public float attractionWeight = 0.3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 파리지옥의 위치를 가져오기
        if (flytrapTransform != null)
        {
            trapPos = new Vector2(-0.65f, 0.4f);
        }
        else
        {
            Debug.LogWarning("FlyController: flytrapTransform이 할당되지 않았습니다.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 파리지옥의 위치를 매 프레임 업데이트
        if (flytrapTransform != null)
        {
            trapPos = new Vector2(-0.65f, 0.4f);
        }
        
        float distToCenter = Vector2.Distance(trapPos, transform.position);
        
        // 1. 배회 힘
        Vector2 wanderForce = GetWanderForce();
        
        // 2. 유혹 힘 (거리에 따라 감소)
        Vector2 attractionForce = Vector2.zero;
        if (distToCenter < attractionRange)
        {
            Vector2 dirToCenter = (trapPos - (Vector2)transform.position).normalized;
            float attractionFalloff = 1f - (distToCenter / attractionRange);
            attractionForce = dirToCenter * attractionFalloff * attractionStrength;
        }
        
        // 3. 힘 합산
        Vector2 totalForce = wanderForce * wanderWeight + attractionForce * attractionWeight;
        
        velocity += totalForce * Time.deltaTime;
        velocity = Vector2.ClampMagnitude(velocity, speed);
        
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    Vector2 GetWanderForce()
    {
        wanderAngle += Random.Range(-wanderJitter, wanderJitter);
        wanderAngle = Mathf.Clamp(wanderAngle, 0, Mathf.PI * 2);
        
        wanderTarget = new Vector2(
            Mathf.Cos(wanderAngle),
            Mathf.Sin(wanderAngle)
        ) * wanderRadius;
        
        Vector2 desiredVelocity = (wanderTarget - velocity).normalized * speed;
        return desiredVelocity - velocity;
    }
}
