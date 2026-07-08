using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager
{
    private float MONTH_DURATION = 180f; // 3 minutes
    private float monthTimer = 0f;
    private TitleUI title;

    public bool IsFrozen { get; private set; } = false;

    public TitleUI Title => title;
    public int CurrentMonth { get; private set; }
    public float MonthTimer => monthTimer;

    public Action<int> OnMonthChanged;

    public GameManager()
    {
        title = Managers.UI.MakeWorldSpaceUI<TitleUI>(null, "TitleUI");
        title.setAnchoredPosition(title.gameObject, new Vector2(0, 0));

        // 세이브 데이터가 있으면 복원, 없으면 기본값 사용
        SaveData saved = Managers.Data.Load();
        if (saved != null)
        {
            CurrentMonth = saved.currentMonth;
            monthTimer = saved.monthTimer;
        }
        else
        {
            CurrentMonth = 3;
            monthTimer = 0f;
        }

        title.updateMonth(CurrentMonth);
        title.updateTime(monthTimer);
        if (PlantController.Data != null)
            title.updateEnergy(PlantController.Data.CurrentEnergy);
    }

    public void Update(float deltaTime)
    {
        if (IsFrozen) return;

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

    /// <summary>
    /// 클리어 조건 달성 시 호출. 게임을 멈추고 EndMonth 팝업을 띄운다.
    /// </summary>
    public void FreezeForEndMonth()
    {
        if (IsFrozen) return;
        IsFrozen = true;
        Time.timeScale = 0f;

        EndMonth popup = Managers.UI.ShowPopupUI<EndMonth>("EndMonth");
        popup.SetInfo(CurrentMonth + 1, AdvanceToNextMonth);
    }

    /// <summary>
    /// EndMonth 팝업의 NextMonthButton 클릭 시 호출. 다음 달로 진행한다.
    /// </summary>
    public void AdvanceToNextMonth()
    {
        if (!IsFrozen) return;
        IsFrozen = false;
        Time.timeScale = 1f;

        monthTimer = 0f;
        CurrentMonth++;
        title.updateMonth(CurrentMonth);
        OnMonthChanged?.Invoke(CurrentMonth);

        Managers.Data.Save();
    }
}
