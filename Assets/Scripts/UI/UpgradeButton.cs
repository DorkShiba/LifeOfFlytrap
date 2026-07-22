using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum InsideTexts
{
    Cost,
    CurrentLevel,
}

public class UpgradeButton : BaseUI
{
    private bool _init = false;
    private TextMeshProUGUI costText, currentLevelText;
    private Button button;
    private GameDefines.UpgradeOptions option;
    private Func<GameDefines.UpgradeOptions, bool> onClickCallback;

    public override void Init()
    {
        if (_init) return;
        _init = true;
        Bind<TextMeshProUGUI>(typeof(InsideTexts));
        costText = Get<TextMeshProUGUI>(0);
        currentLevelText = Get<TextMeshProUGUI>(1);

        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClicked);
    }

    /// <summary>
    /// 버튼을 초기화하고 UI를 갱신합니다.
    /// </summary>
    /// <param name="upgradeOption">이 버튼이 담당하는 업그레이드</param>
    /// <param name="callback">클릭 시 실행할 콜백</param>
    public void SetInfo(GameDefines.UpgradeOptions upgradeOption, Func<GameDefines.UpgradeOptions, bool> callback)
    {
        option = upgradeOption;
        onClickCallback = callback;
        RefreshUI();
    }

    /// <summary>현재 레벨에 맞게 비용과 변화량 텍스트를 갱신합니다.</summary>
    public void RefreshUI()
    {
        int level = PlantController.GetLevel(option);
        var costs = GameDefines.GetUpgradeCosts(option);

        if (level >= GameDefines.MaxUpgradeLevel)
        {
            costText.text = "-";
            currentLevelText.text = "MAX";
            if (button != null) button.interactable = false;
        }
        else
        {
            updateCost(costs[level]);
            updateCurrentLevel(level);
            // 버튼을 항상 클릭 가능하게 두어 실패음이 들리도록 수정 (interactable = false 제거)
        }
    }

    public void updateCost(int cost)
    {
        costText.text = $"코스트: {cost}";
    }

    public void updateCurrentLevel(int level)
    {
        currentLevelText.text = $"현재 레벨 {level}";
    }

    private void OnClicked()
    {
        bool success = onClickCallback?.Invoke(option) ?? false;

        if (success)
            Managers.Sound.PlaySFX("Upgrade");
        else
            Managers.Sound.PlaySFX("ButtonClick");

        RefreshUI();
    }
}
