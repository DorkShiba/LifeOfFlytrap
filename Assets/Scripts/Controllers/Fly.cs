using System.Collections;
using UnityEngine;

enum FlyState { Wandering, Attracted, Captured, Escaping }

public class Fly : BugController
{
    [SerializeField] private FlyData data;
    [SerializeField] private FlyState currentState = FlyState.Wandering;
    private float capturedTimer = 0f;

    void Awake() {
        Managers.TrapLogic.AddBug(gameObject);
        data = Managers.Resource.Load<FlyData>("EntityData/FlyData");
        if (data != null)
        {
            InitializeHP(data.HP);
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Move()
    {
        float distanceToTrap = GetClosestTrapDistance();

        switch(currentState)
        {
            case FlyState.Wandering:
                HandleWandering(distanceToTrap);
                break;
            case FlyState.Attracted:
                HandleAttracted(distanceToTrap);
                break;
            case FlyState.Captured:
                HandleCaptured();
                break;
            case FlyState.Escaping:
                HandleEscaping(distanceToTrap);
                break;
        }
    }

    void HandleWandering(float distanceToTrap)
    {
        float closestTrapRange = PlantController.AttractionRange;
        
        // 함정의 AttractionRange 내에 도달했다면 확률로 앉기로 결정
        if (distanceToTrap < closestTrapRange && Random.value < data.CaptureDecisionChance)
        {
            ChangeState(FlyState.Attracted);
            return;
        }

        // 일반 배회 로직
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

        ApplyMovement(true);
    }

    void HandleAttracted(float distanceToTrap)
    {
        // 함정에 도달하면 Captured 상태로 전환
        if (distanceToTrap < 0.7f)
        {
            ChangeState(FlyState.Captured);
            return;
        }

        // 일반 배회 감소 + 함정으로의 끌림 강화 로직
        Vector2 wanderForce = GetWanderVector();
        Vector2 attractionForce = GetAttractionVector();
        Vector2 totalForce =
            wanderForce * data.WanderWeight * 0.1f +  // 배회 영향 감소
            attractionForce * data.AttractionWeight * 2.0f;  // 트랩으로의 끌림 강화

        Vector2 targetVelocity = Vector2.ClampMagnitude(
            velocity + totalForce * Time.deltaTime,
            data.Speed
        );

        // 이동 속도를 부드럽게 보간
        velocity = Vector2.SmoothDamp(
            velocity,
            targetVelocity,
            ref velocitySmoothRef,
            data.AccelerationSmoothTime
        );
        velocity = Vector2.ClampMagnitude(velocity, data.Speed);

        ApplyMovement(true);
    }

    void HandleCaptured()
    {
        // 함정 위에서 머물면서 속도를 0으로 감소
        velocity = Vector2.SmoothDamp(
            velocity,
            Vector2.zero,
            ref velocitySmoothRef,
            0.1f
        );
        ApplyMovement(false);

        // 타이머 증가
        capturedTimer += Time.deltaTime;

        // 시간이 경과하면 Escaping 상태로 전환
        if (capturedTimer >= data.CapturedStayTime)
        {
            ChangeState(FlyState.Escaping);
        }
    }

    void HandleEscaping(float distanceToTrap)
    {
        float closestTrapRange = PlantController.AttractionRange;
        
        // 함정의 AttractionRange에서 충분히 멀어졌으면 다시 Wandering으로 복귀
        if (distanceToTrap > closestTrapRange)
        {
            ChangeState(FlyState.Wandering);
            return;
        }

        // 함정에서 벗어나려는 방향으로 배회
        Vector2 wanderForce = GetWanderVector();
        Vector2 attractionForce = GetAttractionVector();
        Vector2 totalForce =
            wanderForce * data.WanderWeight +
            attractionForce * data.AttractionWeight;

        Vector2 targetVelocity = Vector2.ClampMagnitude(
            velocity + totalForce * Time.deltaTime,
            data.Speed
        );

        // 이동 속도를 부드럽게 보간
        velocity = Vector2.SmoothDamp(
            velocity,
            targetVelocity,
            ref velocitySmoothRef,
            data.AccelerationSmoothTime
        );
        velocity = Vector2.ClampMagnitude(velocity, data.Speed);

        ApplyMovement(true);
    }

    void ApplyMovement(bool shouldLookAt)
    {
        Vector3 nextPos = transform.position + (Vector3)(velocity * Time.deltaTime);
        bool wasClamped;
        nextPos = ClampToMapBounds(nextPos, out wasClamped);
        transform.position = nextPos;

        // 경계에 닿았을 때만 배회 각도를 재설정해 자연스럽게 방향을 바꾼다.
        if (wasClamped)
        {
            wanderAngle = Random.Range(0f, Mathf.PI * 2f);
        }

        if (shouldLookAt && velocity.sqrMagnitude > Util.EPS)
        {
            LookAt(transform.position + (Vector3)velocity);
        }
    }

    Vector3 ClampToMapBounds(Vector3 position, out bool wasClamped)
    {
        float halfWidth = Util.MapWidth * 0.5f;
        float halfHeight = Util.MapHeight * 0.5f;

        float clampedX = Mathf.Clamp(position.x, -halfWidth, halfWidth);
        float clampedY = Mathf.Clamp(position.y, -halfHeight, halfHeight);
        wasClamped = !Mathf.Approximately(clampedX, position.x) || !Mathf.Approximately(clampedY, position.y);

        return new Vector3(clampedX, clampedY, position.z);
    }

    void ChangeState(FlyState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        if (newState == FlyState.Captured)
        {
            capturedTimer = 0f;
        }
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
            float range = PlantController.AttractionRange;

            if (range <= 0f || distance > range)
            {
                continue;
            }

            Vector2 direction = (trapPos - currentPos).normalized;
            float falloff = 1f - (distance / range);
            attractionVector += direction * falloff * PlantController.AttractionStrength;
        }

        return attractionVector;
    }

    float GetClosestTrapDistance()
    {
        if (Managers.TrapLogic.traps == null || Managers.TrapLogic.traps.Count == 0)
            return float.MaxValue;

        Vector2 currentPos = transform.position;
        float closestDistance = float.MaxValue;

        foreach (var trap in Managers.TrapLogic.traps)
        {
            if (trap == null) continue;
            float distance = Vector2.Distance(currentPos, trap.transform.position);
            if (distance < closestDistance)
                closestDistance = distance;
        }

        return closestDistance;
    }
}
