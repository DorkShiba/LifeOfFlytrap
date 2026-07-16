using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum EndText
{
    EndingText,
}

enum EndButton
{
    EndingButton,
}

public class EndMonth : Popup
{
    TextMeshProUGUI endingText;
    Button endingButton;
    Action onNextMonth;

    public override void Init()
    {
        base.Init();
        Bind<TextMeshProUGUI>(typeof(EndText));
        endingText = Get<TextMeshProUGUI>(0);

        Bind<Button>(typeof(EndButton));
        endingButton = Get<Button>(0);
        endingButton.onClick.AddListener(OnEndingClicked);
    }

    /// <summary>
    /// 팝업을 초기화합니다.
    /// </summary>
    /// <param name="nextMonth">다음 달 번호</param>
    /// <param name="callback">버튼 클릭 시 실행할 콜백</param>
    public void SetInfo(int nextMonth, Action callback)
    {
        monthText.text = $"To month {nextMonth}";
        onNextMonth = callback;
    }

    void OnNextMonthClicked()
    {
        onNextMonth?.Invoke();
        Managers.UI.ClosePopupUI(this);
        Time.timeScale = 1f;
    }
}
