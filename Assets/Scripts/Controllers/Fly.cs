using UnityEngine;

public enum FlyState { Wander, Approach, Landed }

/// <summary>
/// 파리. 자체적인 배회/접근/착지 FSM을 구동하며,
/// 파리만의 스탯을 FlyData ScriptableObject에서 읽어온다.
/// </summary>
public class Fly : BugController
{
    [SerializeField] private FlyData data;

    [Header("공통 튜닝")]
    [SerializeField] protected float boundaryPadding = 1.5f;
    [SerializeField] protected float approachCheckInterval = 2f;

    public FlyState currentState;
    protected float currentAngleRad;
    protected float checkTimer;
    protected float landTimer;
    protected Vector2 targetLandingPoint;

    protected override void Awake()
    {
        base.Awake();
        Managers.TrapLogic.AddBug(gameObject);
        if (data == null)
        {
            string prefabName = gameObject.name.Replace("(Clone)", "");
            data = Managers.Resource.Load<FlyData>($"GameData/{prefabName}Data");

            // 만약 해당 이름의 데이터가 없다면 에러 방지를 위해 기본 FlyData 로드
            if (data == null)
            {
                Debug.LogWarning($"[Fly] GameData/{prefabName}Data 를 찾을 수 없어 기본 FlyData를 사용합니다.");
                data = Managers.Resource.Load<FlyData>("GameData/FlyData");
            }
        }

        if (data != null)
        {
            InitializeHP(data.HP);
        }
    }

    public override void Init(Vector2 min, Vector2 max)
    {
        base.Init(min, max);
        currentAngleRad = Random.Range(0f, Mathf.PI * 2f);
        checkTimer = approachCheckInterval;
        currentState = FlyState.Wander;
    }

    public override int EnergyValue => data != null ? data.EnergyValue : 0;
    public override float DigestionTime => data != null ? data.DigestionTime : 0f;
    protected override float MoveSpeed => data != null ? data.MoveSpeed : 2.5f;

    // Fly specific properties
    protected float JitterDegPerSecond => data != null ? data.JitterDegPerSecond : 25f;
    protected override float BaseApproachChancePercent => data != null ? data.BaseApproachChancePercent : 20f;
    protected float LandDurationSeconds => data != null ? data.LandDurationSeconds : 2f;

    protected override void OnFixedUpdate(float dt)
    {
        switch (currentState)
        {
            case FlyState.Wander: TickWander(dt); break;
            case FlyState.Approach: TickApproach(dt); break;
            case FlyState.Landed: TickLanded(dt); break;
        }
    }

    protected virtual void TickWander(float dt)
    {
        currentAngleRad += Random.Range(-1f, 1f) * JitterDegPerSecond * Mathf.Deg2Rad * dt * 6f;

        Vector2 pos = transform.position;
        Vector2 outwardNormal = Vector2.zero;

        if (pos.x < mapMin.x + boundaryPadding) outwardNormal = Vector2.left;
        else if (pos.x > mapMax.x - boundaryPadding) outwardNormal = Vector2.right;
        else if (pos.y < mapMin.y + boundaryPadding) outwardNormal = Vector2.down;
        else if (pos.y > mapMax.y - boundaryPadding) outwardNormal = Vector2.up;

        if (outwardNormal != Vector2.zero)
        {
            Vector2 currentDir = new Vector2(Mathf.Cos(currentAngleRad), Mathf.Sin(currentAngleRad));
            if (Vector2.Dot(currentDir, outwardNormal) > 0f)
            {
                Vector2 center = (mapMin + mapMax) * 0.5f;
                Vector2 toCenter = (center - pos).normalized;
                currentDir = Vector2.Lerp(currentDir, toCenter, 5f * dt).normalized;
                currentAngleRad = Mathf.Atan2(currentDir.y, currentDir.x);
            }
        }

        Move(MoveSpeed, dt);

        checkTimer -= dt;
        if (checkTimer <= 0f)
        {
            checkTimer = approachCheckInterval;
            float chance = GameDefines.GetCurrentLandingChance(BaseApproachChancePercent);
            if (Random.Range(0f, 100f) < chance)
            {
                ITrap trap = SelectTrap();
                if (trap != null)
                {
                    targetTrap = trap;
                    Vector2 colliderSize = targetTrap.ColliderSize;
                    float offsetX = Random.Range(-colliderSize.x * 0.5f, colliderSize.x * 0.5f);
                    float offsetY = Random.Range(-colliderSize.y * 0.5f, colliderSize.y * 0.5f);
                    targetLandingPoint = (Vector2)targetTrap.Position + new Vector2(offsetX, offsetY);
                    ChangeState(FlyState.Approach);
                }
            }
        }
    }

    protected virtual void TickApproach(float dt)
    {
        if (targetTrap == null || !targetTrap.IsAvailable)
        {
            targetTrap = null;
            ChangeState(FlyState.Wander);
            return;
        }

        Vector2 pos = transform.position;
        Vector2 toTrap = targetLandingPoint - pos;
        float dist = toTrap.magnitude;

        float targetAngleDeg = Mathf.Atan2(toTrap.y, toTrap.x) * Mathf.Rad2Deg;
        float diffDeg = Mathf.DeltaAngle(currentAngleRad * Mathf.Rad2Deg, targetAngleDeg);
        currentAngleRad += diffDeg * Mathf.Deg2Rad * 4f * dt;

        float slowDist = Mathf.Max(targetTrap.LandingRadius * 3f, 0.5f);
        float speed = dist < slowDist ? MoveSpeed * (dist / slowDist) : MoveSpeed;
        speed = Mathf.Max(speed, MoveSpeed * 0.15f);
        Move(speed, dt);

        if (dist <= targetTrap.LandingRadius)
        {
            landTimer = LandDurationSeconds;
            ChangeState(FlyState.Landed);
        }
    }

    protected virtual void TickLanded(float dt)
    {
        landTimer -= dt;
        if (landTimer <= 0f)
        {
            targetTrap = null;
            currentAngleRad = Random.Range(0f, Mathf.PI * 2f);
            ChangeState(FlyState.Wander);
        }
    }

    protected void Move(float speed, float dt)
    {
        Vector2 delta = new Vector2(Mathf.Cos(currentAngleRad), Mathf.Sin(currentAngleRad)) * speed * dt;
        if (rb != null)
        {
            rb.MovePosition(rb.position + delta);
        }
        else
        {
            transform.position += (Vector3)delta;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngleRad * Mathf.Rad2Deg - 90f);
    }

    protected virtual void ChangeState(FlyState newState)
    {
        currentState = newState;
        OnStateChanged(newState);
    }

    protected virtual void OnStateChanged(FlyState newState)
    {
        // TODO: 상태별 애니메이션 전환 (예: Landed 진입 시 날개 접기 애니메이션 재생)
    }
}
