using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
[CreateAssetMenu(fileName = "New Fly Data", menuName = "EntityData/FlyData", order = 1)]
public class FlyData: BugData
{
    // Wandering 파라미터
    [SerializeField] private float wanderRadius = 1f;
    [SerializeField] private float wanderDistance = 1.5f;
    [SerializeField] private float wanderJitter = 0.15f;

    // 가중치
    [SerializeField] private float wanderWeight = 0.7f;
    [SerializeField] private float attractionWeight = 0.3f;

    // Arrival Behavior 파라미터
    [SerializeField] private float arrivalRange = 2.0f;
    [SerializeField] private float minSpeed = 0.2f;

    // Captured 상태 파라미터
    [SerializeField] private float capturedStayTime = 3.0f;

    // 앉기 결정 확률 (0~1)
    [SerializeField] private float captureDecisionChance = 0.3f;

    public float WanderRadius => wanderRadius;
    public float WanderDistance => wanderDistance;
    public float WanderJitter => wanderJitter;
    public float WanderWeight => wanderWeight;
    public float AttractionWeight => attractionWeight;
    public float ArrivalRange => arrivalRange;
    public float MinSpeed => minSpeed;
    public float CapturedStayTime => capturedStayTime;
    public float CaptureDecisionChance => captureDecisionChance;
}
