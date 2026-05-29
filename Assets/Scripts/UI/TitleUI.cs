using UnityEngine;
using UnityEngine.UI;
using TMPro;

enum TitleText
{
    MonthText,
    TimeText,
    EnergyText
}

public class TitleUI : BaseUI {
    private TextMeshProUGUI monthText, timeText, energyText;
    public override void Init() {
        Managers.UI.SetPopupCanvas(gameObject);
        Bind<TextMeshProUGUI>(typeof(TitleText));
        monthText = GetText(0);
        timeText = GetText(1);
        energyText = GetText(2);
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
}
