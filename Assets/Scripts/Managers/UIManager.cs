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
            SetSceneCanvas(root, 0);
            return root.transform;
        }
    }

    public T InstantiateUI<T>(Transform parent = null, string name = null) where T : BaseUI {
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

    public void DestroyUI(BaseUI ui) {
        if (ui != null)
            Managers.Resource.Destroy(ui.gameObject);
    }
    
    public T GetUIByName<T>(string name) where T : BaseUI {
        GameObject go = Util.FindChild(Root.gameObject, name, true);
        if (go == null)
            return null;

        return go.GetComponent<T>();
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
        if (parent == null)
            parent = Root;
        else if (parent != Root && parent.IsChildOf(Root) == false)
            return null;

        GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}", parent);

        Canvas canvas = go.GetComponent<Canvas>();
        if (canvas == null) {
            canvas = go.AddComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 0;

        T ui = go.GetComponent<T>();
        if (ui == null)
            ui = go.AddComponent<T>();
        ui.Init();

        return ui;
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : BaseUI {
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
