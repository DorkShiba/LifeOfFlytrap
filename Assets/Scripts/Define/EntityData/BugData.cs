using System;
using UnityEngine;

[Serializable]
public class BugData : ScriptableObject
{
    [SerializeField] private int hp = 1;

    [Header("Bug AI 스탯")]
    [Tooltip("이동 속도 (BugController.MoveSpeed)")]
    [SerializeField] private float moveSpeed = 2.5f;
    [Tooltip("매 주기마다 트랩에 접근할 기본 확률(%) (BugController.BaseApproachChancePercent)")]
    [SerializeField] private float baseApproachChancePercent = 20f;
    [Tooltip("잡혔을 때 식물에 주는 에너지량 (BugController.EnergyValue)")]
    [SerializeField] private float energyValue = 5f;
    [Header("Spawn Settings")]
    [Tooltip("이 벌레가 처음 등장하는 달 (1~12)")]
    [SerializeField] private int firstAppearMonth = 1;
    [Tooltip("가중치 랜덤 추첨 시 사용되는 가중치. 값이 클수록 더 자주 등장")]
    [SerializeField] private float spawnWeight = 100f;
    [Tooltip("Managers.Resource.Instantiate()에 전달할 프리팹 이름 (예: \"Fly\", \"Dragonfly\")")]
    [SerializeField] private string prefabName = "";

    public int HP => hp;

    // BugController AI 스탯
    public float MoveSpeed => moveSpeed;
    public float BaseApproachChancePercent => baseApproachChancePercent;
    public float EnergyValue => energyValue;

    public int FirstAppearMonth => firstAppearMonth;
    public float SpawnWeight => spawnWeight;
    public string PrefabName => prefabName;
}
