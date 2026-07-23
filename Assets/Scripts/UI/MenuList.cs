using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum MenuButtons
{
    SaveButton,
    ToTitleButton,
    GameOverButton,
}

public class MenuList : BaseUI
{
    private bool _init = false;
    void Start()
    {
        Init();
    }
    private MenuButton saveButton, toTitleButton, gameOverButton;

    public override void Init()
    {
        if (_init) return;
        _init = true;
        Bind<MenuButton>(typeof(MenuButtons));
        saveButton = Get<MenuButton>(0);
        toTitleButton = Get<MenuButton>(1);
        gameOverButton = Get<MenuButton>(2);

        // 각 버튼 Init() 먼저 호출 (costText 등 바인딩)
        saveButton.Init();
        toTitleButton.Init();
        gameOverButton.Init();

        // 굳이 새 스크립트를 만들지 않고, 여기서 람다 함수로 이벤트를 바로 연결합니다.
        if (gameOverButton.button != null)
        {
            gameOverButton.button.onClick.AddListener(() =>
            {
                Debug.Log("게임 종료!");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        }
    }
}
