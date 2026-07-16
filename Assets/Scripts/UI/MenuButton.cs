using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class MenuButton : BaseUI
{
    public Button button;
    private bool _isInitialized = false;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClicked);
    }

    protected virtual void OnClicked()
    {
        Managers.Sound.PlaySFX("ButtonClick");
        // 자식 클래스에서 오버라이드하여 각 버튼의 기능을 구현합니다.
    }
}
