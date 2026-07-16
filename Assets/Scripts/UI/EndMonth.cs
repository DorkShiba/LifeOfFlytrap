using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum EndText
{
    EndingText,
    ButtonText,
}

enum EndButton
{
    EndingButton,
}

public class EndMonth : Popup
{
    TextMeshProUGUI endingText, buttonText;
    Button endingButton;
    Action onNextMonth;

    public override void Init()
    {
        base.Init();
        Bind<TextMeshProUGUI>(typeof(EndText));
        endingText = Get<TextMeshProUGUI>(0);
        buttonText = Get<TextMeshProUGUI>(1);

        Bind<Button>(typeof(EndButton));
        endingButton = Get<Button>(0);
        endingButton.onClick.AddListener(OnEndingClicked);
    }

    /// <summary>
    /// 팝업을 초기화합니다.
    /// </summary>
    /// <param name="isClear">클리어 여부</param>
    /// <param name="month">해당 달 번호</param>
    /// <param name="callback">버튼 클릭 시 실행할 콜백</param>
    public void SetInfo(bool isClear, int month, Action callback)
    {
        if (isClear)
        {
            Managers.Sound.PlaySFX("NewMonth");
            if (month > 10)
            {
                endingText.text = "모든 달을\n무사히 넘겼다!";
                buttonText.text = "타이틀로";
            }
            else
            {
                endingText.text = "이번 달을\n무사히 넘겼다!";
                buttonText.text = "다음 달로";
            }
        }
        else
        {
            endingText.text = "이번 달을\n넘기지 못했어...";
            buttonText.text = "이번 달 재시작";
        }
        onNextMonth = callback;
    }

    void OnEndingClicked()
    {
        onNextMonth?.Invoke();
        Managers.UI.ClosePopupUI(this);
        Time.timeScale = 1f;
    }
}
