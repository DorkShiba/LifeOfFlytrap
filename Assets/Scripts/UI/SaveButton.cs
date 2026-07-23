using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveButton : MenuButton
{
    private SaveList saveList;
    protected override void OnClicked()
    {
        base.OnClicked();
        if (saveList != null) return;
        saveList = Managers.UI.ShowPopupUI<SaveList>();
        saveList.SetMode(SaveListMode.Save);
    }
}
