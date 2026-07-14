using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MenuButton
{
    private SaveList saveList;
    protected override void OnClicked()
    {
        if (saveList != null) return;
        saveList = Managers.UI.ShowPopupUI<SaveList>();
    }
}