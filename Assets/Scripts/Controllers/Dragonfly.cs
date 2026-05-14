using System.Collections;
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

    private Vector3 idleAnchor;
    private float noiseSeedX;
    private float noiseSeedY;

    void Awake() {
        Managers.TrapLogic.AddBug(gameObject);
        data = Managers.Resource.Load<DragonflyData>("EntityData/DragonflyData");
        if (data != null)
        {
            InitializeHP(data.HP);
        }
        EnterIdle();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Move()
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
            transform.position -= (Vector3)(direction * data.Speed *
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

    Vector2 selectDirection()
    {
        return new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;
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
        direction = selectDirection();
        velocity = direction * data.Speed;
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
