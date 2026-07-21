using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum EndButton
{
    EndingButton,
}

public class ClearMonth : Popup
{
    TextMeshProUGUI endingText;
    Button endingButton;
    Action onNextMonth;

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(EndButton));
        endingButton = Get<Button>(0);
        endingButton.onClick.AddListener(OnEndingClicked);
    }

    /// <summary>
    /// 팝업을 초기화합니다.
    /// </summary>
    /// <param name="month">해당 달 번호</param>
    /// <param name="callback">버튼 클릭 시 실행할 콜백</param>
    public void SetInfo(int month, Action callback)
    {
        Managers.Sound.PlaySFX("ClearMonth");
        onNextMonth = callback;
    }

    void OnEndingClicked()
    {
        onNextMonth?.Invoke();
        Managers.UI.ClosePopupUI(this);
        Time.timeScale = 1f;
    }
}
