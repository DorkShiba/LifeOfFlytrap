using UnityEngine;
using System;

/// <summary>
/// 파리(Fly) 전용 ScriptableObject 데이터.
/// 공통 AI 스탯(MoveSpeed, JitterDegPerSecond, BaseApproachChancePercent,
/// LandDurationSeconds, EnergyValue)은 부모 BugData에서 설정한다.
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "New Fly Data", menuName = "EntityData/FlyData", order = 1)]
public class FlyData : BugData
{
    // ── 아래 필드들은 구 스텔스 AI 코드의 잔재입니다 ──
    // 현재 BugController(FSM 기반)는 이 값들을 사용하지 않습니다.
    // 새 로직이 필요하면 재활성화하거나 삭제하세요.

    // [Header("구 스텔스 AI 파라미터 (미사용)")]
    // [SerializeField] private float wanderRadius = 1f;
    // [SerializeField] private float wanderDistance = 1.5f;
    // [SerializeField] private float wanderJitter = 0.15f;
    // [SerializeField] private float wanderWeight = 0.7f;
    // [SerializeField] private float attractionWeight = 0.3f;
    // [SerializeField] private float arrivalRange = 2.0f;
    // [SerializeField] private float minSpeed = 0.2f;
    // [SerializeField] private float capturedStayTime = 3.0f;
    // [SerializeField] private float captureDecisionChance = 0.3f;

    // 파리 전용 추가 스탯이 필요하면 여기에 추가하세요.
}
