using UnityEngine;

/// <summary>
/// 파리. BugController의 배회/접근/착지 FSM을 그대로 사용하고
/// 파리만의 스탯을 FlyData ScriptableObject에서 읽어온다.
/// </summary>
public class Fly : BugController
{
    [SerializeField] private FlyData data;

    void Awake()
    {
        Managers.TrapLogic.AddBug(gameObject);
        if (data == null)
            data = Managers.Resource.Load<FlyData>("EntityData/FlyData");
    }

    public override float EnergyValue => data != null ? data.EnergyValue : 0f;
    protected override float MoveSpeed => data != null ? data.MoveSpeed : 2.5f;
    protected override float JitterDegPerSecond => data != null ? data.JitterDegPerSecond : 25f;
    protected override float BaseApproachChancePercent => data != null ? data.BaseApproachChancePercent : 20f;
    protected override float LandDurationSeconds => data != null ? data.LandDurationSeconds : 2f;

    protected override void OnStateChanged(BugState newState)
    {
        // TODO: 상태별 애니메이션 전환 (예: Landed 진입 시 날개 접기 애니메이션 재생)
    }
}