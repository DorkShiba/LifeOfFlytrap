using UnityEngine;
using UnityEngine.UI;
using TMPro;

enum UpgradeButtons
{
    AddLeafButton,
    StrongBiteButton,
    StrongScentButton,
    DeeperRootButton,
    SturdyStemButton,
}

public class UpgradeList : Popup
{
    private UpgradeButton addLeafButton, strongBiteButton,
        strongScentButton, deeperRootButton, sturdyStemButton;

    public override void Init()
    {
        base.Init();
        Bind<UpgradeButton>(typeof(UpgradeButtons));
        addLeafButton    = Get<UpgradeButton>(0);
        strongBiteButton  = Get<UpgradeButton>(1);
        strongScentButton = Get<UpgradeButton>(2);
        deeperRootButton  = Get<UpgradeButton>(3);
        sturdyStemButton  = Get<UpgradeButton>(4);

        // 각 버튼 Init() 먼저 호출 (costText 등 바인딩)
        addLeafButton   .Init();
        strongBiteButton .Init();
        strongScentButton.Init();
        deeperRootButton .Init();
        sturdyStemButton .Init();

        // 각 버튼에 업그레이드 옵션과 콜백 연결
        addLeafButton   .SetInfo(PlantDefines.UpgradeOptions.AddLeaf,    OnUpgradeClicked);
        strongBiteButton .SetInfo(PlantDefines.UpgradeOptions.StrongBite,  OnUpgradeClicked);
        strongScentButton.SetInfo(PlantDefines.UpgradeOptions.StrongScent, OnUpgradeClicked);
        deeperRootButton .SetInfo(PlantDefines.UpgradeOptions.DeepRoot,    OnUpgradeClicked);
        sturdyStemButton .SetInfo(PlantDefines.UpgradeOptions.SturdyStem,  OnUpgradeClicked);
    }

    /// <summary>
    /// 업그레이드 버튼 클릭 시 호출.
    /// PlantController에 업그레이드를 위임하고 모든 버튼 UI를 갱신합니다.
    /// </summary>
    private void OnUpgradeClicked(PlantDefines.UpgradeOptions option)
    {
        PlantController plantCtrl = FindObjectOfType<PlantController>();
        if (plantCtrl == null) return;

        bool success = plantCtrl.TryUpgrade(option);
        if (success)
        {
            // 에너지가 바뀌었으므로 모든 버튼의 활성 상태 갱신
            addLeafButton   .RefreshUI();
            strongBiteButton .RefreshUI();
            strongScentButton.RefreshUI();
            deeperRootButton .RefreshUI();
            sturdyStemButton .RefreshUI();
        }
    }
}
