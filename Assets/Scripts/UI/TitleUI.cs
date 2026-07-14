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

    public void updateMonth(int month)
    {
        monthText.text = $"{month}월";
    }

    public void updateTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timeText.text = $"{minutes:00}:{seconds:00}";
    }

    public void updateEnergy(int energy)
    {
        energyText.text = $"에너지: {energy}";
    }

    UpgradeList upgradeList;

    public void OnUpgradeToggleButtonClicked()
    {
        if (upgradeList != null)
        {
            Managers.UI.DestroyUI(upgradeList);
            upgradeList = null;
        }
        else
        {
            upgradeList = Managers.UI.InstantiateUI<UpgradeList>();
        }
    }

    MenuList menuList;
    public void OnMenuToggleButtonClicked()
    {
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
