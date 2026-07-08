using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 벌레(Fly, Dragonfly 등)의 공통 AI 베이스 클래스.
/// WANDER -> APPROACH -> LANDED 상태 머신 + 스티어링 기반 이동을 담당한다.
/// 종별 스탯(속도, 지터, 접근 확률, 착지 시간)만 하위 클래스에서 정의하면 된다.
/// </summary>
public abstract class BugController : MonoBehaviour
{
    protected enum BugState { Wander, Approach, Landed }

    // 씬에 존재하는 트랩들을 등록해두는 정적 레지스트리.
    // 실제 Trap 스크립트의 OnEnable/OnDisable에서 RegisterTrap/UnregisterTrap을 호출해주세요.
    private static readonly List<ITrap> traps = new List<ITrap>();
    private static ITrapAttractionProvider attractionProvider;

    public static void RegisterTrap(ITrap trap)
    {
        if (!traps.Contains(trap)) traps.Add(trap);
    }

    public static void UnregisterTrap(ITrap trap)
    {
        traps.Remove(trap);
    }

    // 업그레이드 시스템(에너지로 트랩 유인 배율 강화)을 이 provider로 연결하세요.
    public static void SetAttractionProvider(ITrapAttractionProvider provider)
    {
        attractionProvider = provider;
    }

    [Header("맵 범위 (스포너가 Init으로 세팅)")]
    protected Vector2 mapMin;
    protected Vector2 mapMax;

    [Header("공통 튜닝")]
    [SerializeField] protected float boundaryPadding = 1.5f;
    [SerializeField] protected float approachCheckInterval = 2f;

    protected BugState currentState;
    protected float currentAngleRad;
    protected float checkTimer;
    protected float landTimer;
    protected ITrap targetTrap;

    // 종별로 반드시 정의해야 하는 스탯들
    protected abstract float MoveSpeed { get; }
    protected abstract float JitterDegPerSecond { get; }
    protected abstract float BaseApproachChancePercent { get; }
    protected abstract float LandDurationSeconds { get; }

    /// <summary>잡혔을 때 식물에 주는 에너지량. 하위 클래스에서 Data로부터 읽어온다.</summary>
    public abstract float EnergyValue { get; }

    protected virtual float TrapAttractionMultiplier =>
        attractionProvider != null ? attractionProvider.GetMultiplier() : 1f;

    // HP 및 Freeze 상태
    private int hp = 1;
    private bool isFrozen = false;

    /// <summary>스포너가 벌레를 생성할 때 맵 경계를 넘겨주며 호출.</summary>
    public virtual void Init(Vector2 min, Vector2 max)
    {
        mapMin = min;
        mapMax = max;
        currentAngleRad = Random.Range(0f, Mathf.PI * 2f);
        checkTimer = approachCheckInterval;
        currentState = BugState.Wander;
    }

    protected virtual void Update()
    {
        if (isFrozen) return;  // Freeze 중에는 AI 동작 정지
        float dt = Time.deltaTime;
        switch (currentState)
        {
            case BugState.Wander: TickWander(dt); break;
            case BugState.Approach: TickApproach(dt); break;
            case BugState.Landed: TickLanded(dt); break;
        }
    }

    protected virtual void TickWander(float dt)
    {
        // 기존 방향을 유지한 채 각도만 조금씩 흔들어준다 (지터).
        currentAngleRad += Random.Range(-1f, 1f) * JitterDegPerSecond * Mathf.Deg2Rad * dt * 6f;

        // 경계 패딩 구역에 들어오면 안쪽 방향으로 부드럽게 조향.
        Vector2 pos = transform.position;
        float? towardDeg = null;
        if (pos.x < mapMin.x + boundaryPadding) towardDeg = 0f;
        else if (pos.x > mapMax.x - boundaryPadding) towardDeg = 180f;
        else if (pos.y < mapMin.y + boundaryPadding) towardDeg = 90f;
        else if (pos.y > mapMax.y - boundaryPadding) towardDeg = -90f;

        if (towardDeg.HasValue)
        {
            float diffDeg = Mathf.DeltaAngle(currentAngleRad * Mathf.Rad2Deg, towardDeg.Value);
            currentAngleRad += diffDeg * Mathf.Deg2Rad * 2.5f * dt;
        }

        Move(MoveSpeed, dt);

        // 일정 주기마다 한 번만 확률을 굴려서 프레임레이트 영향을 없앤다.
        checkTimer -= dt;
        if (checkTimer <= 0f)
        {
            checkTimer = approachCheckInterval;
            float chance = Mathf.Min(90f, BaseApproachChancePercent * TrapAttractionMultiplier);
            if (Random.Range(0f, 100f) < chance)
            {
                ITrap trap = SelectTrap();
                if (trap != null)
                {
                    targetTrap = trap;
                    ChangeState(BugState.Approach);
                }
            }
        }
    }

    protected virtual void TickApproach(float dt)
    {
        if (targetTrap == null || !targetTrap.IsAvailable)
        {
            targetTrap = null;
            ChangeState(BugState.Wander);
            return;
        }

        Vector2 pos = transform.position;
        Vector2 toTrap = (Vector2)targetTrap.Position - pos;
        float dist = toTrap.magnitude;

        // 목적지 방향으로 각도를 서서히 회전 (급격한 방향 전환 방지).
        float targetAngleDeg = Mathf.Atan2(toTrap.y, toTrap.x) * Mathf.Rad2Deg;
        float diffDeg = Mathf.DeltaAngle(currentAngleRad * Mathf.Rad2Deg, targetAngleDeg);
        currentAngleRad += diffDeg * Mathf.Deg2Rad * 4f * dt;

        // 트랩에 가까워질수록 감속.
        float slowDist = Mathf.Max(targetTrap.LandingRadius * 3f, 0.5f);
        float speed = dist < slowDist ? MoveSpeed * (dist / slowDist) : MoveSpeed;
        speed = Mathf.Max(speed, MoveSpeed * 0.15f);
        Move(speed, dt);

        if (dist <= targetTrap.LandingRadius)
        {
            transform.position = targetTrap.Position;
            targetTrap.Occupy(this);
            landTimer = LandDurationSeconds;
            ChangeState(BugState.Landed);
        }
    }

    protected virtual void TickLanded(float dt)
    {
        landTimer -= dt;
        if (landTimer <= 0f)
        {
            targetTrap?.Vacate(this);
            targetTrap = null;
            currentAngleRad = Random.Range(0f, Mathf.PI * 2f);
            ChangeState(BugState.Wander);
        }
    }

    protected void Move(float speed, float dt)
    {
        Vector3 delta = new Vector3(Mathf.Cos(currentAngleRad), Mathf.Sin(currentAngleRad), 0f) * speed * dt;
        transform.position += delta;
    }

    protected virtual void ChangeState(BugState newState)
    {
        currentState = newState;
        OnStateChanged(newState);
    }

    /// <summary>애니메이션/이펙트 전환 훅. 하위 클래스에서 override해서 사용.</summary>
    protected virtual void OnStateChanged(BugState newState) { }

    /// <summary>
    /// 접근할 트랩을 고르는 로직. 기본은 "가장 가까운 사용 가능한 트랩".
    /// 벌레 종류마다 타겟팅 방식이 다르다면(예: 잠자리는 더 먼 트랩도 노림) override.
    /// </summary>
    protected virtual ITrap SelectTrap()
    {
        ITrap nearest = null;
        float best = float.MaxValue;
        Vector2 pos = transform.position;
        for (int i = 0; i < traps.Count; i++)
        {
            ITrap t = traps[i];
            if (!t.IsAvailable) continue;
            float d = ((Vector2)t.Position - pos).sqrMagnitude;
            if (d < best) { best = d; nearest = t; }
        }
        return nearest;
    }

    /// <summary>플레이어가 트랩을 닫아 벌레를 잡았을 때 트랩 스크립트가 호출.</summary>
    public virtual void OnCaught()
    {
        targetTrap?.Vacate(this);
        Destroy(gameObject);
    }

    // ─────────────────────────────────────────────
    // HP / 피해 / 상태 API (TrapController에서 호출)
    // ─────────────────────────────────────────────

    /// <summary>HP를 초기화한다. 하위 클래스 Awake에서 Data.HP를 넘겨 호출.</summary>
    protected void InitializeHP(int maxHp)
    {
        hp = maxHp;
    }

    /// <summary>
    /// 피해를 입힌다. HP가 0 이하로 떨어지면 true를 반환한다.
    /// TrapController.ApplyBiteDamage에서 호출한다.
    /// </summary>
    public bool TakeDamage(int damage)
    {
        hp -= damage;
        return hp <= 0;
    }

    /// <summary>벌레를 제자리에 고정한다. 스냅 구간 진입 시 TrapController가 호출.</summary>
    public void Freeze()
    {
        isFrozen = true;
    }

    /// <summary>고정을 해제한다. 스냅 실패(탈출) 시 TrapController가 호출.</summary>
    public void Unfreeze()
    {
        isFrozen = false;
    }

    /// <summary>
    /// 벌레가 먹혔을 때 TrapLogicManager가 호출.
    /// 에너지 수집 후 오브젝트를 제거한다.
    /// </summary>
    public virtual void Die()
    {
        targetTrap?.Vacate(this);
        Destroy(gameObject);
    }

    protected virtual void FixedUpdate()
    {
        // Freeze 중에는 이동 억제 (Update에서 Move가 호출되기 전 상태를 막음)
        // 실제 이동 억제는 Update 오버라이드 또는 isFrozen 가드로 처리할 수 있다.
    }
}