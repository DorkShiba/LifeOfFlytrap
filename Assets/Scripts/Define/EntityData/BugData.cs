using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class BugData: ScriptableObject
{
    [SerializeField] protected float speed = 3f;

    [SerializeField] private float accelerationSmoothTime = 0.18f;
    [SerializeField] private float energy = 5f;
    [SerializeField] private float digestionTime = 8f;
    [SerializeField] private int hp = 1;

    public float Speed => speed;
    public float AccelerationSmoothTime => accelerationSmoothTime;
    public float Energy => energy;
    public float DigestionTime => digestionTime;
    public int HP => hp;
}
