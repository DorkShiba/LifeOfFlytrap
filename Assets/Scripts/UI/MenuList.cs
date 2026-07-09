using UnityEngine;
using UnityEngine.UI;
using TMPro;

enum MenuButtons
{
    SaveButton,
    LoadButton,
}

public class MenuList : Popup
{
    private MenuButton saveButton, loadButton;

    public override void Init()
    {
        base.Init();
        Bind<MenuButton>(typeof(MenuButtons));
        saveButton    = Get<MenuButton>(0);
        loadButton  = Get<MenuButton>(1);

        // 각 버튼 Init() 먼저 호출 (costText 등 바인딩)
        saveButton   .Init();
        loadButton .Init();
    }
}
