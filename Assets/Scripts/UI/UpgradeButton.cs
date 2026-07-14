using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum InsideTexts
{
    Cost,
    UpgradeChange,
}

public class UpgradeButton : Popup
{
    private TextMeshProUGUI costText, upgradeChangeText;
    private Button button;
    private PlantDefines.UpgradeOptions option;
    private Action<PlantDefines.UpgradeOptions> onClickCallback;

    public override void Init()
    {
        base.Init();
        Bind<TextMeshProUGUI>(typeof(InsideTexts));
        costText = Get<TextMeshProUGUI>(0);
        upgradeChangeText = Get<TextMeshProUGUI>(1);

        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClicked);
    }

    /// <summary>
    /// 버튼을 초기화하고 UI를 갱신합니다.
    /// </summary>
    /// <param name="upgradeOption">이 버튼이 담당하는 업그레이드</param>
    /// <param name="callback">클릭 시 실행할 콜백</param>
    public void SetInfo(PlantDefines.UpgradeOptions upgradeOption, Action<PlantDefines.UpgradeOptions> callback)
    {
        option = upgradeOption;
        onClickCallback = callback;
        RefreshUI();
    }

    /// <summary>현재 레벨에 맞게 비용과 변화량 텍스트를 갱신합니다.</summary>
    public void RefreshUI()
    {
        int level = PlantController.GetLevel(option);
        var costs = PlantDefines.GetUpgradeCosts(option);

        if (level >= costs.Count)
        {
            costText.text = "MAX";
            upgradeChangeText.text = "";
            if (button != null) button.interactable = false;
        }
        else
        {
            updateCost(costs[level]);
            if (button != null)
                button.interactable = PlantController.Data != null &&
                                      PlantController.Data.CurrentEnergy >= costs[level];
        }
    }

    public void updateCost(int cost)
    {
        costText.text = $"소모에너지: {cost}";
    }

    public void updateUpgradeChange(int prev, int next)
    {
        upgradeChangeText.text = $"레벨: {prev} -> {next}";
    }

    private void OnClicked()
    {
        onClickCallback?.Invoke(option);
        RefreshUI();
    }
}
