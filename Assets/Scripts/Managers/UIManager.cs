using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager
{
    int _order = 10;

    Stack<Popup> _popupStack = new Stack<Popup>();

    // 사전 매핑용 딕셔너리
    private Dictionary<string, Canvas> _uiCanvasDict = new Dictionary<string, Canvas>();
    private Dictionary<string, GameObject> _uiPrefabDict = new Dictionary<string, GameObject>();

    public void Init()
    {
        // 1. 씬 내의 UIBase 수집 후 캔버스 매핑 및 파괴
        BaseUI[] sceneUIs = Object.FindObjectsByType<BaseUI>(FindObjectsSortMode.None);
        foreach (BaseUI ui in sceneUIs)
        {
            Canvas parentCanvas = ui.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                _uiCanvasDict[ui.GetType().Name] = parentCanvas;
                Debug.Log($"Register Canvas: {ui.GetType().Name}, {parentCanvas.name}");
            }
            Object.Destroy(ui.gameObject);
        }

        // 2. 프리팹 캐싱
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/UI");
        foreach (GameObject prefab in prefabs)
        {
            _uiPrefabDict[prefab.name] = prefab;
            Debug.Log($"Register Prefab: {prefab.name}");
        }
    }

    public Transform Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
            {
                root = new GameObject { name = "@UI_Root" };
                SetSceneCanvas(root, 0);
            }

            // EventSystem 자동 생성
            EventSystem eventSystem = Object.FindAnyObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject esObj = new GameObject("@EventSystem");
                esObj.AddComponent<EventSystem>();
                esObj.AddComponent<StandaloneInputModule>();
            }

            return root.transform;
        }
    }

    public T ShowPopupUI<T>(string name = null,
        float left = 0, float right = 0,
        float top = 0, float bottom = 0) where T : Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        Transform parent = _uiCanvasDict.ContainsKey(name) ? _uiCanvasDict[name].transform : Root;
        GameObject go;

        if (_uiPrefabDict.TryGetValue(name, out GameObject prefab))
        {
            go = Managers.Resource.Instantiate(prefab, parent);
            go.name = name;
        }
        else
        {
            Debug.LogError($"[UIManager] 프리팹 캐시에 {name}이(가) 없습니다. Resources/Prefabs/UI 폴더를 확인하세요.");
            return null;
        }

        RectTransform rect = go.GetComponent<RectTransform>();
        if (rect != null)
        {
            Vector2 oMin = rect.offsetMin;
            Vector2 oMax = rect.offsetMax;
            oMin.x = left;
            oMin.y = bottom;
            oMax.x = -right;
            oMax.y = -top;
            rect.offsetMin = oMin;
            rect.offsetMax = oMax;
        }

        T popup = go.GetComponent<T>();
        if (popup == null)
            popup = go.AddComponent<T>();

        _popupStack.Push(popup);

        popup.Init();

        return popup;
    }

    // ClosePopupUI 구현
    public void ClosePopupUI(Popup popup)
    {
        if (_popupStack.Count == 0) return;

        if (_popupStack.Peek() != popup)
        {
            Debug.LogWarning("닫으려는 팝업이 스택의 최상단이 아닙니다!");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0) return;

        Popup popup = _popupStack.Pop();
        popup.Close();
        Managers.Resource.Destroy(popup.gameObject);

        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
        {
            ClosePopupUI();
        }
    }

    public T InstantiateUI<T>(Transform parent = null, string name = null) where T : BaseUI
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        if (parent == null)
        {
            parent = _uiCanvasDict.ContainsKey(name) ? _uiCanvasDict[name].transform : Root;
        }
        else if (parent != Root && parent.IsChildOf(Root) == false)
            return null;

        GameObject go;
        if (_uiPrefabDict.TryGetValue(name, out GameObject prefab))
        {
            go = Managers.Resource.Instantiate(prefab, parent);
            go.name = name;
        }
        else
        {
            Debug.LogError($"[UIManager] 프리팹 캐시에 {name}이(가) 없습니다. Resources/Prefabs/UI 폴더를 확인하세요.");
            return null;
        }

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
        {
            ui.Close();
            Managers.Resource.Destroy(ui.gameObject);
        }
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
        Debug.Log($"MakeWorldSpaceUI: {name}");
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        if (parent == null)
            parent = Root;
        else if (parent != Root && parent.IsChildOf(Root) == false)
            return null;

        GameObject go;
        if (_uiPrefabDict.TryGetValue(name, out GameObject prefab))
        {
            Debug.Log($"Before Instantiate: {name}");
            go = Managers.Resource.Instantiate(prefab, parent);
            go.name = name;
        }
        else
        {
            Debug.LogError($"[UIManager] 프리팹 캐시에 {name}이(가) 없습니다. Resources/Prefabs/UI 폴더를 확인하세요.");
            return null;
        }

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

        GameObject go;
        if (_uiPrefabDict.TryGetValue(path, out GameObject prefab))
        {
            go = Managers.Resource.Instantiate(prefab, parent);
            go.name = name != null ? name : path;
        }
        else
        {
            Debug.LogError($"[UIManager] 프리팹 캐시에 {path}이(가) 없습니다. Resources/Prefabs/UI 폴더를 확인하세요.");
            return null;
        }

        T ui = go.GetComponent<T>();
        if (ui == null) ui = go.AddComponent<T>();
        ui.Init();

        return ui;
    }
}
