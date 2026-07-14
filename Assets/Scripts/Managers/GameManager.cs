using System;
using UnityEngine;

public class GameManager
{
    private TitleUI title;
    public TitleUI Title => title;

    // 현재 플레이 중인 세션
    public GameSession CurrentSession { get; private set; }

    public GameManager()
    {
        // GameManager 생성 시점이 Init 씬일 수 있으므로 UI 생성을 StartGame으로 미룹니다.
    }

    public void StartGame()
    {
        // 메인 씬 로딩이 완료된 후에 UI를 생성해야 파괴되지 않습니다.
        if (title == null)
        {
            title = Managers.UI.MakeWorldSpaceUI<TitleUI>(null, "TitleUI");
            if (title != null)
                title.setAnchoredPosition(title.gameObject, new Vector2(0, 0));
        }

        CurrentSession = new GameSession();
        CurrentSession.Init();

        // Title 씬이 켜지거나 게임 시작될 때 SpawnManager 구독 갱신 등 필요하다면 추가
        Managers.Spawn.Init();
    }

    public void GameOver()
    {
        CurrentSession = null;
    }
}
