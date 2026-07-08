using UnityEngine;
using UnityEngine.UI;
using TMPro;

enum TitleTexts
{
    MonthText,
    TimeText,
    EnergyText
}

enum TitleButtons
{
    ToggleButton,
}

public class TitleUI : BaseUI {
    private TextMeshProUGUI monthText, timeText, energyText;
    private Button upgradeToggleButton;
    public override void Init() {
        Managers.UI.SetPopupCanvas(gameObject);
        Bind<TextMeshProUGUI>(typeof(TitleTexts));
        monthText = GetText(0);
        timeText = GetText(1);
        energyText = GetText(2);

        Bind<Button>(typeof(TitleButtons));
        upgradeToggleButton = Get<Button>(0);
        upgradeToggleButton.onClick.AddListener(OnUpgradeToggleButtonClicked);
    }

    public void updateMonth(int month)
    {
        monthText.text = $"Month {month}";
    }

    public void updateTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timeText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    public void updateEnergy(int energy)
    {
        energyText.text = $"Energy: {energy}";
    }

    UpgradeList upgradeListPopup;

    public void OnUpgradeToggleButtonClicked()
    {
        if (upgradeListPopup != null)
        {
            Managers.UI.DestroyUI(upgradeListPopup);
            upgradeListPopup = null;
        }
        else
        {
            upgradeListPopup = Managers.UI.ShowPopupUI<UpgradeList>("UpgradeList");
        }
    }
}
