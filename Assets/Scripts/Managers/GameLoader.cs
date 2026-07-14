using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class GameLoader : MonoBehaviour
{
    void Awake()
    {
        // 글로벌 환경 설정
        Application.targetFrameRate = 60;
        
        // 싱글턴 초기화 진입점
        // Managers 프리팹이나 객체가 씬에 없더라도 접근 시 자동 생성되도록 유도
        Managers.Init(); 
    }

    IEnumerator Start()
    {
        // 1프레임 대기 후 씬 전환 (안정성 확보)
        yield return null;
        
        // Init 씬에서 초기화가 끝났으므로 Title 씬으로 전환
        Managers.Scene.LoadScene("Title", () => {
            Debug.Log("Title 씬 로드 완료");
        });
    }

    void Update()
    {
        CheckButtonClick();
    }

    // UI 클릭 사운드 중앙 처리
    private GameObject lastSelected;
    private float lastPlayTime;
    private const float interval = 0.05f;
    private readonly List<RaycastResult> results = new List<RaycastResult>();

    private void CheckButtonClick()
    {
        EventSystem es = EventSystem.current;
        if (es == null)
            return;

        results.Clear();
        bool isPointerDownThisFrame = false;
        Vector2 screenPosition = new Vector2(-1, -1);

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            screenPosition = Mouse.current.position.ReadValue();
            isPointerDownThisFrame = true;
        }
        else if (Touchscreen.current != null)
        {
            foreach (TouchControl touch in Touchscreen.current.touches)
            {
                if (touch.press.wasPressedThisFrame)
                {
                    screenPosition = touch.position.ReadValue();
                    isPointerDownThisFrame = true;
                    break;
                }
            }
        }

        GameObject current = es.currentSelectedGameObject;
        if (current == null || current == lastSelected || !isPointerDownThisFrame)
            return;

        if (Time.unscaledTime - lastPlayTime < interval)
            return;

        // Button 또는 Click 핸들러가 있는 오브젝트인지 확인
        GameObject eventHandlerObj = ExecuteEvents.GetEventHandler<UnityEngine.UI.Button>(current);
        if (!eventHandlerObj)
            return;

        lastPlayTime = Time.unscaledTime;
        lastSelected = current;

        // 임시로 디버그 로그, 향후 사운드 매니저 연결
        // Managers.Sound.Play("ui_clicked");
        Debug.Log($"UI Clicked: {current.name}");
    }
}
