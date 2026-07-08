using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    int _order = 10;

    Stack<Popup> _popupStack = new Stack<Popup>();

    public Transform Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            SetSceneCanvas(root, 0);
            return root.transform;
        }
    }

    public T ShowPopupUI<T>(string name,
        float left = 0, float right = 0,
        float top = 0, float bottom = 0) where T : Popup
    {
        GameObject go = InstantiateUI<T>(Root, $"Popup/{name}").gameObject;

        if (!string.IsNullOrEmpty(name))
            go.name = name;

        RectTransform rect = go.GetComponent<RectTransform>();
        Vector2 oMin = rect.offsetMin;
        Vector2 oMax = rect.offsetMax;
        oMin.x = left;
        oMin.y = bottom;
        oMax.x = -right;
        oMax.y = -top;
        rect.offsetMin = oMin;
        rect.offsetMax = oMax;

        T popup = go.GetComponent<T>();
        if (popup == null)
            popup = go.AddComponent<T>();
        _popupStack.Push(popup);

        return popup;
    }

    public T InstantiateUI<T>(Transform parent = null, string name = null) where T : BaseUI
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        if (parent == null)
            parent = Root;
        else if (parent != Root && parent.IsChildOf(Root) == false)
            return null;

        GameObject go = Managers.Resource.Instantiate($"UI/{name}", parent);

        SetSceneCanvas(go, 0);

        T ui = go.GetComponent<T>();
        if (ui == null)
            ui = go.AddComponent<T>();
        ui.Init();

        return ui;
    }

    public void DestroyUI(BaseUI ui)
    {
        if (ui != null)
            Managers.Resource.Destroy(ui.gameObject);
    }

    public T GetUIByName<T>(string name) where T : BaseUI
    {
        GameObject go = Util.FindChild(Root.gameObject, name, true);
        if (go == null)
            return null;

        return go.GetComponent<T>();
    }

    public void SetSceneCanvas(GameObject go, int order)
    {
        Canvas canvas = go.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = go.AddComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = order;

        // 해상도에 관계없이 UI가 동일하게 보이도록 CanvasScaler 설정
        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        if (scaler == null)
            scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        GraphicRaycaster raycaster = go.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
            go.AddComponent<GraphicRaycaster>();
    }

    public void SetPopupCanvas(GameObject go)
    {
        Canvas canvas = go.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = go.AddComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = _order;
        _order++;

        GraphicRaycaster raycaster = go.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
            go.AddComponent<GraphicRaycaster>();
    }

    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : BaseUI
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        if (parent == null)
            parent = Root;
        else if (parent != Root && parent.IsChildOf(Root) == false)
            return null;

        GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}", parent);

        Canvas canvas = go.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = go.AddComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 0;

        GraphicRaycaster raycaster = go.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
            go.AddComponent<GraphicRaycaster>();

        T ui = go.GetComponent<T>();
        if (ui == null)
            ui = go.AddComponent<T>();
        ui.Init();

        return ui;
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : BaseUI
    {
        string path = typeof(T).Name;
        if (parent == null)
            parent = Root;
        else if (parent != Root && parent.IsChildOf(Root) == false)
            return null;

        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{path}", parent);

        if (!string.IsNullOrEmpty(name))
            go.name = name;

        T ui = go.GetComponent<T>();
        if (ui == null) ui = go.AddComponent<T>();
        ui.Init();

        return ui;
    }
}
