using UnityEngine;
using UnityEngine.UI;
using TMPro;

enum CurrentUpgrades
{
    CurrentUpgrade_1,
    CurrentUpgrade_2,
    CurrentUpgrade_3,
    CurrentUpgrade_4,
    CurrentUpgrade_5,
}

public class UpgradeTable : Popup
{
    private bool _init = false;
    void Start()
    {
        Init();
    }
    private CurrentUpgrade cu_addLeaf, cu_strongBite, cu_strongScent,
        cu_deeperRoot, cu_sturdyStem;

    public override void Init()
    {
        if (_init) return;
        _init = true;
        Bind<CurrentUpgrade>(typeof(CurrentUpgrades));
        cu_addLeaf    = Get<CurrentUpgrade>(0);
        cu_strongBite  = Get<CurrentUpgrade>(1);
        cu_strongScent = Get<CurrentUpgrade>(2);
        cu_deeperRoot  = Get<CurrentUpgrade>(3);
        cu_sturdyStem  = Get<CurrentUpgrade>(4);

        // 각 버튼 Init() 먼저 호출 (costText 등 바인딩)
        cu_addLeaf   .Init();
        cu_strongBite .Init();
        cu_strongScent.Init();
        cu_deeperRoot .Init();
        cu_sturdyStem .Init();

        // 각 버튼에 업그레이드 옵션과 콜백 연결
        cu_addLeaf   .SetInfo(PlantDefines.UpgradeOptions.AddLeaf);
        cu_strongBite .SetInfo(PlantDefines.UpgradeOptions.StrongBite);
        cu_strongScent.SetInfo(PlantDefines.UpgradeOptions.StrongScent);
        cu_deeperRoot .SetInfo(PlantDefines.UpgradeOptions.DeepRoot);
        cu_sturdyStem .SetInfo(PlantDefines.UpgradeOptions.SturdyStem);
    }
}
