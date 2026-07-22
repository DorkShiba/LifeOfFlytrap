using UnityEngine;

public class ToTitleButton : MenuButton
{
    protected override void OnClicked()
    {
        base.OnClicked();

        Managers.Scene.LoadScene("Title");
    }
}
