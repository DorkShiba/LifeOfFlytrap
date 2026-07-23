using UnityEngine;

public class ToTitleButton : MenuButton
{
    protected override void OnClicked()
    {
        base.OnClicked();

        // 타이틀로 돌아갈 때는 게임 세션을 초기화하여 Update 루프를 멈춥니다.
        Managers.Game.GameOver();

        Managers.Scene.LoadScene("Title");
    }
}
