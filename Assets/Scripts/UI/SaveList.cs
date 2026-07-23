using UnityEngine;
using UnityEngine.UI;

enum SaveSlotNames
{
    SaveSlot1,
    SaveSlot2,
    SaveSlot3,
}

public enum SaveListMode
{
    Save,
    Load
}

public class SaveList : Popup
{
    public SaveListMode CurrentMode { get; private set; }
    private SaveSlot saveSlot1, saveSlot2, saveSlot3;

    public void SetMode(SaveListMode mode)
    {
        CurrentMode = mode;
        // 슬롯들의 UI 갱신 (모드에 따라 interactable 상태가 달라질 수 있음)
        if (saveSlot1 != null) saveSlot1.RefreshUI();
        if (saveSlot2 != null) saveSlot2.RefreshUI();
        if (saveSlot3 != null) saveSlot3.RefreshUI();
    }

    public override void Init()
    {
        base.Init();
        Managers.UI.SetPopupCanvas(gameObject);

        // 팝업 외부(배경) 클릭 시 닫히도록 투명한 블로커(Blocker) 생성
        CreateBackgroundBlocker();

        Bind<SaveSlot>(typeof(SaveSlotNames));
        saveSlot1 = Get<SaveSlot>(0);
        saveSlot2 = Get<SaveSlot>(1);
        saveSlot3 = Get<SaveSlot>(2);

        // 각 슬롯에 인덱스와 SaveList(this) 참조를 전달하여 초기화
        saveSlot1.SetInfo(1, this);
        saveSlot2.SetInfo(2, this);
        saveSlot3.SetInfo(3, this);
    }

    private GameObject _blocker;

    private void CreateBackgroundBlocker()
    {
        _blocker = new GameObject("Blocker");
        RectTransform blockerRect = _blocker.AddComponent<RectTransform>();
        
        // SaveList와 동일한 부모(예: UI_Root)에 형제로 배치
        blockerRect.SetParent(transform.parent, false);
        blockerRect.SetSiblingIndex(transform.GetSiblingIndex());

        // 부모(Root 캔버스)의 크기에 맞춰 꽉 채움 (화면 전체 덮기)
        blockerRect.anchorMin = Vector2.zero;
        blockerRect.anchorMax = Vector2.one;
        blockerRect.offsetMin = Vector2.zero;
        blockerRect.offsetMax = Vector2.zero;
        blockerRect.localScale = Vector3.one;

        // SaveList 캔버스보다 1단계 아래의 sortingOrder를 주어 정확히 팝업 뒤에만 위치하게 설정
        Canvas myCanvas = GetComponent<Canvas>();
        if (myCanvas != null)
        {
            Canvas blockerCanvas = _blocker.AddComponent<Canvas>();
            blockerCanvas.overrideSorting = true;
            blockerCanvas.sortingOrder = myCanvas.sortingOrder - 1;
            _blocker.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        // 반투명한 어두운 배경 추가
        UnityEngine.UI.Image blockerImage = _blocker.AddComponent<UnityEngine.UI.Image>();
        blockerImage.color = new Color(0, 0, 0, 0.5f); 

        // 버튼 추가 및 클릭 이벤트에 닫기 함수 연결
        UnityEngine.UI.Button blockerButton = _blocker.AddComponent<UnityEngine.UI.Button>();
        blockerButton.onClick.AddListener(() => {
            Debug.Log("[SaveList] 팝업 외부 클릭됨. 팝업을 닫습니다.");
            ClosePopupUI();
        });
    }

    private void OnDestroy()
    {
        // 팝업이 파괴될 때 형제로 생성된 블로커도 함께 파괴
        if (_blocker != null)
        {
            Destroy(_blocker);
        }
    }
}