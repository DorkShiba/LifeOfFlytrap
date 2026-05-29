using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class GameManager {
    private float MONTH_DURATION = 180f; // 3 minutes
    private float monthTimer = 0f;
    private TitleUI title;

    
    public TitleUI Title => title;
    public int CurrentMonth { get; private set; }

    public Action<int> OnMonthChanged;

    public GameManager()
    {
        CurrentMonth = 3;
        title = Managers.UI.MakeWorldSpaceUI<TitleUI>(null, "TitleUI");
        title.setAnchoredPosition(title.gameObject, new Vector2(0, 0));
        title.updateMonth(CurrentMonth);
        title.updateTime(monthTimer);
    }

    public void Update(float deltaTime)
    {
        monthTimer += deltaTime;
        title.updateTime(monthTimer);
        if (monthTimer >= MONTH_DURATION)
        {
            monthTimer = 0f;
            CurrentMonth++;
            title.updateMonth(CurrentMonth);
            OnMonthChanged?.Invoke(CurrentMonth);
        }
    }
}
