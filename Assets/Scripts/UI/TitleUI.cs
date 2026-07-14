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
        Managers.UI.SetPopupCanvas(gameObject);
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

    UpgradeList upgradeListPopup;

    public void OnUpgradeToggleButtonClicked()
    {
        if (upgradeListPopup != null)
        {
            Managers.UI.ClosePopupUI(upgradeListPopup);
            upgradeListPopup = null;
        }
        else
        {
            upgradeListPopup = Managers.UI.ShowPopupUI<UpgradeList>("UpgradeList");
        }
    }

    MenuList menuListPopup;
    public void OnMenuToggleButtonClicked()
    {
        if (menuListPopup != null)
        {
            Managers.UI.ClosePopupUI(menuListPopup);
            menuListPopup = null;
        }
        else
        {
            menuListPopup = Managers.UI.ShowPopupUI<MenuList>("MenuList");
        }
    }
}
