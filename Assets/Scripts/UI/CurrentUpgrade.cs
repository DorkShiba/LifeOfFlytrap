using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum TextsInCurrentUpgrade
{
    CurrentLevel,
}

public class CurrentUpgrade : BaseUI
{
    private bool _init = false;
    private TextMeshProUGUI currentLevelText;
    private PlantDefines.UpgradeOptions option;

    public override void Init()
    {
        if (_init) return;
        _init = true;
        Bind<TextMeshProUGUI>(typeof(TextsInCurrentUpgrade));
        currentLevelText = Get<TextMeshProUGUI>(0);
    }

    /// <param name="upgradeOption">담당하는 업그레이드</param>
    public void SetInfo(PlantDefines.UpgradeOptions upgradeOption)
    {
        option = upgradeOption;
        RefreshUI();
    }

    /// <summary>현재 레벨에 맞게 비용과 변화량 텍스트를 갱신합니다.</summary>
    public void RefreshUI()
    {
        int level = PlantController.GetLevel(option);

        if (level >= PlantDefines.MaxUpgradeLevel)
        {
            currentLevelText.text = "MAX";
            return;
        }
        
        currentLevelText.text = $"현재 레벨 {level}";
    }
}
