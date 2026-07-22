using UnityEngine;

enum DragonflyState
{
    Idle,
    Rotate,
    Ready,
    Rush,
    Decel
}

public class DragonFly : BugController
{
    [SerializeField] private DragonflyData data;
    [SerializeField] private bool isStopped = true;
    [SerializeField] private DragonflyState state = DragonflyState.Idle;
    [SerializeField] private float timer = 0f;
    [SerializeField] private Vector2 direction = Vector2.zero;

    // 잠자리 고유 이동 상태 (BugController에 없으므로 여기서 선언)
    private Vector2 velocity = Vector2.zero;

    void Awake()
    {
        Managers.TrapLogic.AddBug(gameObject);
        data = Managers.Resource.Load<DragonflyData>("GameData/DragonflyData");
        if (data != null)
        {
            InitializeHP(data.HP);
        }
        EnterIdle();
    }

    // ─────────────────────────────────────────────
    // BugController 추상 프로퍼티 구현
    // 잠자리는 BugController FSM(Wander/Approach/Landed)을 사용하지 않지만
    // abstract이므로 컴파일을 위해 구현해둔다.
    // ─────────────────────────────────────────────
    public override int EnergyValue => data != null ? data.EnergyValue : 0;
    public override float DigestionTime => data != null ? data.DigestionTime : 0f;
    protected override float MoveSpeed => data != null ? data.MoveSpeed : 3f;
    protected override float BaseApproachChancePercent => data != null ? data.BaseApproachChancePercent : 0f;

    // ─────────────────────────────────────────────
    // 잠자리 고유 Update (BugController FSM 미사용)
    // ─────────────────────────────────────────────
    protected override void OnFixedUpdate(float dt)
    {
        // Freeze 중에는 이동 정지 (BugController.FixedUpdate 패턴 준수)
        DragonFlyMove(dt);
    }

    private void DragonFlyMove(float dt)
    {
        timer -= dt;
        if (state == DragonflyState.Idle)
        {
            if (timer <= Util.EPS) EnterRotate();
        }
        else if (state == DragonflyState.Rotate)
        {
            if (timer <= Util.EPS) EnterReady();
        }
        else if (state == DragonflyState.Ready)
        {
            Vector2 delta = -(Vector2)direction * data.MoveSpeed * data.DecelMultiplier * dt;
            if (rb != null) rb.MovePosition(rb.position + delta);
            else transform.position += (Vector3)delta;

            if (timer <= Util.EPS) EnterRush();
        }
        else if (state == DragonflyState.Rush)
        {
            Vector2 delta = velocity * dt;
            if (rb != null) rb.MovePosition(rb.position + delta);
            else transform.position += (Vector3)delta;

            if (timer <= Util.EPS) EnterDecel();
        }
        else if (state == DragonflyState.Decel)
        {
            velocity = Vector2.Lerp(velocity, Vector2.zero, 1 - timer / data.DecelTime);
            Vector2 delta = velocity * dt;
            if (rb != null) rb.MovePosition(rb.position + delta);
            else transform.position += (Vector3)delta;

            if (timer <= Util.EPS) EnterIdle();
        }
    }

    Vector2 SelectDirection()
    {
        // 일정 확률로 가장 가까운 트랩을 조준
        float chance = GameDefines.GetCurrentLandingChance();
        if (Random.Range(0f, 100f) < chance)
        {
            ITrap trap = SelectTrap();
            if (trap != null)
            {
                return ((Vector2)trap.Position - (Vector2)transform.position).normalized;
            }
        }

        // 확률이 빗나가거나 트랩이 없으면 경계를 고려한 랜덤 방향 선택
        Vector2 pos = transform.position;
        float halfW = GameData.Instance.MapWidth * 0.5f;
        float halfH = GameData.Instance.MapHeight * 0.5f;
        float margin = data.RushDistanceBound + 1f;

        for (int i = 0; i < 10; i++)
        {
            Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            Vector2 projected = pos + dir * data.MoveSpeed * data.BaseRushTime;
            if (Mathf.Abs(projected.x) < halfW - margin && Mathf.Abs(projected.y) < halfH - margin)
                return dir;
        }
        // 10번 시도 실패 시 맵 중심 방향으로
        return ((Vector2)(Vector3.zero - transform.position)).normalized;
    }

    void EnterIdle()
    {
        state = DragonflyState.Idle;
        float stopTime = data.BaseStopTime + Random.Range(-data.StopTimeRange, data.StopTimeRange);
        timer = stopTime;
    }

    void EnterRotate()
    {
        state = DragonflyState.Rotate;
        direction = SelectDirection();
        velocity = direction * data.MoveSpeed;
        LookAt(transform.position + (Vector3)direction);
        timer = 0.5f;
    }

    void EnterReady()
    {
        state = DragonflyState.Ready;
        timer = data.ReadyRushTime;
    }

    void EnterRush()
    {
        state = DragonflyState.Rush;
        float rushTime = data.BaseRushTime + Random.Range(-data.RushTimeRange, data.RushTimeRange);
        timer = rushTime;
    }

    void EnterDecel()
    {
        state = DragonflyState.Decel;
        timer = data.DecelTime;
    }
}

