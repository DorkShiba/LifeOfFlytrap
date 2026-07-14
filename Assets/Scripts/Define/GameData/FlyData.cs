using System;
using UnityEngine;

/// <summary>
/// 파리(Fly) 전용 ScriptableObject 데이터.
/// 공통 AI 스탯(MoveSpeed, BaseApproachChancePercent, EnergyValue)은 부모 BugData에서 설정한다.
/// Fly 전용 스탯(JitterDegPerSecond, LandDurationSeconds)는 여기서 설정한다.
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "New Fly Data", menuName = "GameData/Bug/FlyData", order = 1)]
public class FlyData : BugData
{
    [Header("Fly 전용 AI 스탯")]
    [Tooltip("배회 시 방향 흘델림 속도(도/초) (BugController.JitterDegPerSecond)")]
    [SerializeField] private float jitterDegPerSecond = 25f;

    [Tooltip("트랩에 착지 후 머무는 시간(초) (BugController.LandDurationSeconds)")]
    [SerializeField] private float landDurationSeconds = 2f;

    public float JitterDegPerSecond => jitterDegPerSecond;
    public float LandDurationSeconds => landDurationSeconds;

    // 구 스텔스 AI 파라미터 (미사용 주석 보존)
    // [SerializeField] private float wanderRadius = 1f;
    // ...
}
