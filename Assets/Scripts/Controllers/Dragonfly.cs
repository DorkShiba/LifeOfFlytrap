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
        data = Managers.Resource.Load<DragonflyData>("EntityData/DragonflyData");
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
    public override float EnergyValue => data != null ? data.EnergyValue : 0f;
    protected override float MoveSpeed => data != null ? data.MoveSpeed : 3f;
    protected override float JitterDegPerSecond => 0f;   // DragonFly는 배회 FSM 미사용
    protected override float BaseApproachChancePercent => data != null ? data.BaseApproachChancePercent : 0f;
    protected override float LandDurationSeconds => 0f;   // DragonFly는 착지 FSM 미사용

    // ─────────────────────────────────────────────
    // 잠자리 고유 Update (BugController FSM 미사용)
    // ─────────────────────────────────────────────
    protected override void Update()
    {
        // Freeze 중에는 이동 정지 (BugController.Update 패턴 준수)
        // base.Update()는 호출하지 않음 — 잠자리는 자체 FSM을 사용한다.
        DragonFlyMove();
    }

    private void DragonFlyMove()
    {
        timer -= Time.deltaTime;
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
            transform.position -= (Vector3)(direction * data.MoveSpeed *
                data.DecelMultiplier * Time.deltaTime);
            if (timer <= Util.EPS) EnterRush();
        }
        else if (state == DragonflyState.Rush)
        {
            transform.position += (Vector3)(velocity * Time.deltaTime);
            if (timer <= Util.EPS) EnterDecel();
        }
        else if (state == DragonflyState.Decel)
        {
            velocity = Vector2.Lerp(velocity, Vector2.zero, 1 - timer / data.DecelTime);
            transform.position += (Vector3)(velocity * Time.deltaTime);
            if (timer <= Util.EPS) EnterIdle();
        }
    }

    Vector2 SelectDirection()
    {
        // 경계 근처면 안쪽을 향하는 방향만 허용
        Vector2 pos = transform.position;
        float halfW = Util.MapWidth * 0.5f;
        float halfH = Util.MapHeight * 0.5f;
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

