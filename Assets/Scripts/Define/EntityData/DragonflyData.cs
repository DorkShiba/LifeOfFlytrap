using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
[CreateAssetMenu(fileName = "New Dragonfly Data", menuName = "EntityData/DragonflyData", order = 1)]
public class DragonflyData: BugData
{
    // Wandering 파라미터
    [SerializeField] private float baseStopTime = 5f;
    [SerializeField] private float stopTimeRange = 1f;
    [SerializeField] private float baseRushTime = 2.0f;
    [SerializeField] private float rushTimeRange = 0.6f;
    [SerializeField] private float readyRushTime = 0.5f;
    [SerializeField] private float decelMultiplier = 0.25f;
    [SerializeField] private float rushDistanceBound = 2.5f;
    [SerializeField] private float decelTime = 1f;

    public float BaseStopTime => baseStopTime;
    public float StopTimeRange => stopTimeRange;
    public float BaseRushTime => baseRushTime;
    public float RushTimeRange => rushTimeRange;
    public float ReadyRushTime => readyRushTime;
    public float DecelMultiplier => decelMultiplier;
    public float RushDistanceBound => rushDistanceBound;
    public float DecelTime => decelTime;
}
