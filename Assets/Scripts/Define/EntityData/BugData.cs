using UnityEngine;
using System;

[Serializable]
public class BugData : ScriptableObject
{
    [Header("기본 스탯")]
    [Tooltip("이동 기본 속도 (DragonFly의 Rush/Decel에서 직접 사용)")]
    [SerializeField] protected float speed = 3f;

    [SerializeField] private int hp = 1;

    [Header("Bug AI 스탯")]
    [Tooltip("이동 속도 (BugController.MoveSpeed)")]
    [SerializeField] private float moveSpeed = 2.5f;

    [Tooltip("배회 시 방향 흔들림 속도(도/초) (BugController.JitterDegPerSecond)")]
    [SerializeField] private float jitterDegPerSecond = 25f;

    [Tooltip("매 주기마다 트랩에 접근할 기본 확률(%) (BugController.BaseApproachChancePercent)")]
    [SerializeField] private float baseApproachChancePercent = 20f;

    [Tooltip("트랩에 착지 후 머무는 시간(초) (BugController.LandDurationSeconds)")]
    [SerializeField] private float landDurationSeconds = 2f;

    [Tooltip("잡혔을 때 식물에 주는 에너지량 (BugController.EnergyValue)")]
    [SerializeField] private float energyValue = 5f;

    [Header("Spawn Settings")]
    [Tooltip("이 벌레가 처음 등장하는 달 (1~12)")]
    [SerializeField] private int firstAppearMonth = 1;

    [Tooltip("가중치 랜덤 추첨 시 사용되는 가중치. 값이 클수록 더 자주 등장")]
    [SerializeField] private float spawnWeight = 100f;

    [Tooltip("Managers.Resource.Instantiate()에 전달할 프리팹 이름 (예: \"Fly\", \"Dragonfly\")")]
    [SerializeField] private string prefabName = "";

    [Header("탈출 설정")]
    [Tooltip("탈출 시 이동 속도 배수 (DragonFly: speed × exitSpeedMultiplier)")]
    [SerializeField] private float exitSpeedMultiplier = 1.5f;

    public float Speed => speed;
    public int HP => hp;

    // BugController AI 스탯
    public float MoveSpeed => moveSpeed;
    public float JitterDegPerSecond => jitterDegPerSecond;
    public float BaseApproachChancePercent => baseApproachChancePercent;
    public float LandDurationSeconds => landDurationSeconds;
    public float EnergyValue => energyValue;

    public int FirstAppearMonth => firstAppearMonth;
    public float SpawnWeight => spawnWeight;
    public string PrefabName => prefabName;
    public float ExitSpeedMultiplier => exitSpeedMultiplier;
}
