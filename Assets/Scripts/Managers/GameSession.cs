using System;
using UnityEngine;

public class GameSession
{
    private float MONTH_DURATION => GameData.Instance.MonthDuration;
    private float monthTimer = 0f;

    public bool IsFrozen { get; private set; } = false;
    public int CurrentMonth { get; private set; }
    public float MonthTimer => monthTimer;

    public Action<int> OnMonthChanged;

    public void Init()
    {
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

        TitleUI title = Managers.Game.Title;
        if (title != null)
        {
            title.updateMonth(CurrentMonth);
            title.updateTime(monthTimer);
            if (PlantController.Data != null)
                title.updateEnergy(PlantController.Data.CurrentEnergy);
        }
    }

    public void Update(float deltaTime)
    {
        if (IsFrozen) return;

        monthTimer += deltaTime;

        TitleUI title = Managers.Game.Title;
        if (title != null)
            title.updateTime(monthTimer);

        if (monthTimer >= MONTH_DURATION)
        {
            monthTimer = MONTH_DURATION;
            int monthIndex = CurrentMonth - 1;
            int requiredEnergy = 0;
            requiredEnergy = GameDefines.GetRequiredEnergy(CurrentMonth);

            bool isClear = (PlantController.Data != null && PlantController.Data.CurrentEnergy >= requiredEnergy);
            FreezeForEndMonth(isClear);
        }
    }

    /// <summary>
    /// 달(월)이 끝났을 때 호출. 클리어 시 ClearMonth 팝업을, 실패 시 Die 씬을 로드한다.
    /// </summary>
    public void FreezeForEndMonth(bool isClear)
    {
        if (IsFrozen) return;
        IsFrozen = true;
        Time.timeScale = 0f;

        if (isClear)
        {
            ClearMonth popup = Managers.UI.ShowPopupUI<ClearMonth>("ClearMonth");
            if (CurrentMonth >= 10)
                popup.SetInfo(11, LoadClearScene);
            else
                popup.SetInfo(CurrentMonth + 1, AdvanceToNextMonth);
        }
        else
        {
            Managers.Scene.FadeAndLoadScene("Die", 1f);
        }
    }

    /// <summary>
    /// 모든 달(10월)을 클리어했을 때 Clear 씬을 로드한다.
    /// </summary>
    public void LoadClearScene()
    {
        if (!IsFrozen) return;
        IsFrozen = false;
        Time.timeScale = 1f;

        // 클리어 시 세이브 데이터를 지우고 Clear 씬로 이동
        Managers.Data.DeleteSave();
        Managers.Scene.LoadScene("Clear");
    }

    /// <summary>
    /// ClearMonth 팝업의 NextMonthButton 클릭 시 호출. 다음 달로 진행한다.
    /// </summary>
    public void AdvanceToNextMonth()
    {
        if (!IsFrozen) return;
        IsFrozen = false;
        Time.timeScale = 1f;

        monthTimer = 0f;
        CurrentMonth++;

        Managers.TrapLogic?.ClearBugsFromMap();

        TitleUI title = Managers.Game.Title;
        if (title != null)
            title.updateMonth(CurrentMonth);

        OnMonthChanged?.Invoke(CurrentMonth);

        Managers.Data.Save();
    }

    public void RetryMonth()
    {
        if (!IsFrozen) return;
        IsFrozen = false;
        Time.timeScale = 1f;

        monthTimer = 0f;

        Managers.TrapLogic?.ClearBugsFromMap();

        SaveData saved = Managers.Data.Load();
        if (saved != null)
        {
            CurrentMonth = saved.currentMonth;
        }
        else
        {
            CurrentMonth = 3;
        }

        TitleUI title = Managers.Game.Title;
        if (title != null)
        {
            title.updateMonth(CurrentMonth);
            title.updateTime(monthTimer);
            if (PlantController.Data != null)
                title.updateEnergy(PlantController.Data.CurrentEnergy);
        }
    }
}
