using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class GameManager {
    private float MONTH_DURATION = 180f; // 3 minutes
    private float monthTimer = 0f;
    public int CurrentMonth { get; private set; }

    public Action<int> OnMonthChanged;

    public GameManager()
    {
        CurrentMonth = 3;
    }

    public void Update(float deltaTime)
    {
        monthTimer += deltaTime;
        if (monthTimer >= MONTH_DURATION)
        {
            monthTimer = 0f;
            CurrentMonth++;
            Debug.Log($"Month {CurrentMonth} has started.");
        }
    }
}
