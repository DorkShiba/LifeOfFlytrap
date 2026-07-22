using UnityEngine;

public class NewGameButton : MenuButton
{
    protected override void OnClicked()
    {
        base.OnClicked();

        // 1. 기존 세이브 삭제 (새 게임은 무조건 초기 상태)
        Managers.Data.DeleteSave();

        // 2. 씬 로딩을 먼저 요청하고, 씬 로딩이 완료되면 게임을 시작합니다.
        Managers.Scene.LoadScene("Main", () =>
        {
            Managers.Game.StartGame();
        });
    }
}
