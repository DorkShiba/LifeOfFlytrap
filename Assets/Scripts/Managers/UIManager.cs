using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager {
    int _order = 10;

    Stack<Popup> _popupStack = new Stack<Popup>();

    public Transform Root {
        get {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root.transform;
        }
    }

    public void SetSceneCanvas(GameObject go, int order) {
        Canvas canvas = go.GetComponent<Canvas>();
        if (canvas == null) {
            canvas = go.AddComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        canvas.sortingOrder = order;
    }

    public void SetPopupCanvas(GameObject go) {
        Canvas canvas = go.GetComponent<Canvas>();
        if (canvas == null) {
            canvas = go.AddComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = _order;
        _order++;
    }

    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : BaseUI {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}", parent);

        Canvas canvas = go.GetComponent<Canvas>();
        if (canvas == null) {
            canvas = go.AddComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        T ui = go.GetComponent<T>();
        if (ui == null)
            ui = go.AddComponent<T>();
        ui.Init();

        return ui;
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : BaseUI {
        string path = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{path}", parent);

        if (!string.IsNullOrEmpty(name))
            go.name = name;

        T ui = go.GetComponent<T>();
        if (ui == null) ui = go.AddComponent<T>();
        ui.Init();

        return ui;
    }

    // public T ShowSceneUI<T>(string name = null) where T : SceneUI {
    //     if (string.IsNullOrEmpty(name))
    //         name = typeof(T).Name;

    //     GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}", Root);

    //     T ui = go.GetComponent<T>();
    //     if (ui == null) ui = go.AddComponent<T>();
    //     ui.Init();

    //     return ui;
    // }

    public T ShowPopupUI<T>(string name = null) where T : Popup {
        string path = typeof(T).Name;

        // Managers.Input.EnableMove(false);
        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{path}", Root);

        if (!string.IsNullOrEmpty(name))
            go.name = name;

        T popup = go.GetComponent<T>();
        if (popup == null) popup = go.AddComponent<T>();
        popup.Init();
        _popupStack.Push(popup);

        return popup;
    }

    public void ClosePopupUI(Popup popup) {
        if (_popupStack.Count == 0)
            return;

        if (_popupStack.Peek() != popup) {
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI() {
        if (_popupStack.Count == 0)
            return;

        Popup popup = _popupStack.Pop();

        Managers.Resource.Destroy(popup.gameObject);

        if (_popupStack.Count == 0)
            // Managers.Input.EnableMove(true);

        _order--;
    }

    public void CloseAllPopupUI() {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void Update() {
        // if (Managers.Input.CheckInput(Define.Input.Menu, true)) {
        //     if (_popupStack.Count > 0) {
        //         ClosePopupUI();
        //         return;
        //     }
        //     // ToDo : ���� �߿� escŰ ������ �޴� ����
        // }
    }

    public void Clear() {
        CloseAllPopupUI();
    }
}
