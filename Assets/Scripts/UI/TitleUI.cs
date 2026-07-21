using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum TitleTexts
{
    MonthText,
    TimeText,
    EnergyText
}

enum TitleButtons
{
    UpgradeToggleButton,
    MenuToggleButton,
}

public class TitleUI : BaseUI
{
    private TextMeshProUGUI monthText, timeText, energyText;
    private Button upgradeToggleButton, menuToggleButton;
    public override void Init()
    {
        Managers.UI.SetSceneCanvas(gameObject, 0);
        Bind<TextMeshProUGUI>(typeof(TitleTexts));
        monthText = GetText(0);
        timeText = GetText(1);
        energyText = GetText(2);

        Bind<Button>(typeof(TitleButtons));
        upgradeToggleButton = Get<Button>(0);
        upgradeToggleButton.onClick.AddListener(OnUpgradeToggleButtonClicked);
        menuToggleButton = Get<Button>(1);
        menuToggleButton.onClick.AddListener(OnMenuToggleButtonClicked);
    }

    private int _currentMonth = 3;

    public void updateMonth(int month)
    {
        _currentMonth = month;
        monthText.text = $"{month}월";
        
        // 월이 바뀌었을 때 목표 에너지 수치도 갱신되도록 다시 호출
        if (PlantController.Data != null)
            updateEnergy(PlantController.Data.CurrentEnergy);
    }

    public void updateTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timeText.text = $"{minutes:00}:{seconds:00}";
    }

    public void updateEnergy(int energy)
    {
        int requiredEnergy = 0;
        int monthIndex = _currentMonth - 1;
        if (GameData.Instance != null && monthIndex >= 0 && monthIndex < GameData.Instance.ClearConstraints.Count)
        {
            requiredEnergy = GameData.Instance.ClearConstraints[monthIndex];
        }

        energyText.text = $"에너지: {energy} / {requiredEnergy}";
    }

    UpgradeTable upgradeTable;

    public void OnUpgradeToggleButtonClicked()
    {
        Managers.Sound.PlaySFX("ButtonClick");
        if (upgradeTable != null)
        {
            Managers.UI.DestroyUI(upgradeTable);
            upgradeTable = null;
        }
        else
        {
            upgradeTable = Managers.UI.InstantiateUI<UpgradeTable>();
        }
    }

    MenuList menuList;
    public void OnMenuToggleButtonClicked()
    {
        Managers.Sound.PlaySFX("ButtonClick");
        if (menuList != null)
        {
            Managers.UI.DestroyUI(menuList);
            menuList = null;
        }
        else
        {
            menuList = Managers.UI.InstantiateUI<MenuList>();
        }
    }
}
