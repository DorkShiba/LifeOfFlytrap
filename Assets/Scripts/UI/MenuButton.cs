using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class MenuButton : Popup
{
    private Button button;

    public override void Init()
    {
        base.Init();

        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
    }
}
