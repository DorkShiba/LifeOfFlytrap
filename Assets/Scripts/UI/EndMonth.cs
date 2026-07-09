using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

enum ClearText
{
    MonthText,
}

enum ClearButtons
{
    NextMonthButton,
}

public class EndMonth : Popup {
    TextMeshProUGUI monthText;
    Button nextMonthButton;
    Action onNextMonth;

    public override void Init() {
        base.Init();
        Bind<TextMeshProUGUI>(typeof(ClearText));
        monthText = Get<TextMeshProUGUI>(0);

        Bind<Button>(typeof(ClearButtons));
        nextMonthButton = Get<Button>(0);
        nextMonthButton.onClick.AddListener(OnNextMonthClicked);
    }

    /// <summary>
    /// 팝업을 초기화합니다.
    /// </summary>
    /// <param name="nextMonth">다음 달 번호</param>
    /// <param name="callback">버튼 클릭 시 실행할 콜백</param>
    public void SetInfo(int nextMonth, Action callback) {
        monthText.text = $"To month {nextMonth}";
        onNextMonth = callback;
    }

    void OnNextMonthClicked() {
        onNextMonth?.Invoke();
        Managers.UI.ClosePopupUI(this);
        Time.timeScale = 1f;
    }
}
