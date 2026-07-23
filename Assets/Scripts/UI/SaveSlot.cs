using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum TextsInSaveSlot
{
    Date, // 세이브한 날짜
    Game, // 진행 상황 정보 텍스트
}

// 버튼이 최상위 오브젝트에 있으므로 Buttons enum은 사용하지 않습니다.

public class SaveSlot : BaseUI
{
    public int slotIndex;
    private SaveList parentList;
    private TextMeshProUGUI dateText;
    private TextMeshProUGUI gameText;
    private Button button;

    private bool _init = false;

    public override void Init()
    {
        if (_init) return;
        _init = true;

        Bind<TextMeshProUGUI>(typeof(TextsInSaveSlot));

        dateText = Get<TextMeshProUGUI>((int)TextsInSaveSlot.Date);
        gameText = Get<TextMeshProUGUI>((int)TextsInSaveSlot.Game);

        // 버튼이 자기 자신(최상위 오브젝트)에 붙어 있으므로 GetComponent 사용
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClickSlot);
        }
    }

    private void Awake()
    {
        Init();
    }

    public void SetInfo(int index, SaveList parent)
    {
        Init();
        slotIndex = index;
        parentList = parent;

        RefreshUI();
    }

    public void RefreshUI()
    {
        // 해당 슬롯에 데이터가 있는지 확인하고 텍스트 갱신
        SaveData data = Managers.Data.Load(slotIndex);
        if (data != null)
        {
            // 플레이 시간 포맷팅 (예: 01:23)
            int minutes = Mathf.FloorToInt(data.monthTimer / 60F);
            int seconds = Mathf.FloorToInt(data.monthTimer - minutes * 60);
            string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

            gameText.text = $"{data.currentMonth} 월 | {timeString} | {data.currentEnergy} 에너지";

            // 저장 날짜 가져오기 (파일의 마지막 수정 날짜)
            string path = DataManager.GetSavePath(slotIndex);
            if (System.IO.File.Exists(path))
            {
                System.DateTime lastWriteTime = System.IO.File.GetLastWriteTime(path);
                dateText.text = lastWriteTime.ToString("yyyy년 MM월 dd일");
            }

            if (button != null) button.interactable = true;
        }
        else
        {
            gameText.text = "새로운 게임 시작";
            dateText.text = "기록 없음";

            if (button != null)
            {
                // 세이브 모드일 때는 빈 슬롯도 클릭 가능, 로드 모드일 때는 클릭 불가
                button.interactable = (parentList.CurrentMode == SaveListMode.Save && parentList != null);
            }
        }
    }

    private void OnClickSlot()
    {
        // 1. 클릭 시 데이터 매니저의 현재 슬롯 번호를 업데이트
        Managers.Data.CurrentSlotIndex = slotIndex;

        if (parentList.CurrentMode == SaveListMode.Save)
        {
            // 세이브 모드: 현재 진행 상태를 해당 슬롯에 저장
            Managers.Data.Save(slotIndex);
            Debug.Log($"[SaveSlot] {slotIndex}번 슬롯에 저장 완료.");

            parentList.ClosePopupUI();
        }
        else
        {
            // 로드 모드: 이미 데이터가 있다면 불러오기 후 씬 전환
            Debug.Log($"[SaveSlot] {slotIndex}번 슬롯 로드 시작.");
            parentList.ClosePopupUI();

            // 인게임 씬인 "Main" 씬으로 이동 후 게임을 시작합니다.
            Managers.Scene.LoadScene("Main", () =>
            {
                Managers.Game.StartGame();
            });
        }
    }
}