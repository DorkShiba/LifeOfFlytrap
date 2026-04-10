using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
[CreateAssetMenu(fileName = "New Bug Data", menuName = "EntityData/BugData", order = 1)]
public class BugData: ScriptableObject
{
    [SerializeField] private float speed = 3f;

    // Wandering 파라미터
    [SerializeField] private float wanderRadius = 1f;
    [SerializeField] private float wanderDistance = 1.5f;
    [SerializeField] private float wanderJitter = 0.15f;

    // 가중치
    [SerializeField] private float wanderWeight = 0.7f;
    [SerializeField] private float attractionWeight = 0.3f;

    [SerializeField] private float accelerationSmoothTime = 0.18f;
    [SerializeField] private float energy = 5f;

    public float Speed => speed;
    public float WanderRadius => wanderRadius;
    public float WanderDistance => wanderDistance;
    public float WanderJitter => wanderJitter;
    public float WanderWeight => wanderWeight;
    public float AttractionWeight => attractionWeight;
    public float AccelerationSmoothTime => accelerationSmoothTime;
    public float Energy => energy;

    void Update()
    {

    }
}
