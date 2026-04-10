using System;
using System.Collections;
using UnityEngine;

public class Managers : MonoBehaviour {
    static Managers _instance;
    private static Managers Instance { get { Init(); return _instance; } }
    private static bool isQuitting = false;

    // #region Content
    // CameraManager _camera = new CameraManager();
    // CombatManager _combat = new CombatManager();

    // public static CameraManager Camera { get { return Instance._camera; } }
    // public static CombatManager Combat { get { return Instance._combat; } }
    // #endregion

    // #region Core
    // DataManager _data = new DataManager();
    InputManager _input;
    // OptionsManager _options = new OptionsManager();
    PoolManager _pool;
    ResourceManager _resource = new ResourceManager();
    // SceneManagerEx _scene = new SceneManagerEx();
    // SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();
    TrapLogicManager _trapLogic;

    void Awake()
    {
        Init();
    }

    void OnApplicationQuit() {
        isQuitting = true;
    }

    // public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return isQuitting ? null : Instance._input; } }
    // public static OptionsManager Options { get { return Instance._options; } }
    public static PoolManager Pool { get { return isQuitting? null: Instance._pool; } }
    public static ResourceManager Resource { get { return isQuitting? null: Instance._resource; } }
    // public static SceneManagerEx Scene { get { return Instance._scene; } }
    // public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return isQuitting? null: Instance._ui; } }
    public static TrapLogicManager TrapLogic { get { return isQuitting? null: Instance._trapLogic; } }
    // #endregion

    public static Coroutine StartCoroutineManager(Func<IEnumerator> func) {
        return Instance.StartCoroutine(func());
    }
    public static Coroutine StartCoroutineManager<T>(Func<T, IEnumerator> func, T t) {
        return Instance.StartCoroutine(func(t));
    }
    public static void StopCoroutineManager(Coroutine coroutine) {
        Instance.StopCoroutine(coroutine);
    }

    static void Init() {
        if (_instance == null) {
            GameObject go = GameObject.Find("@Managers");
            if (go == null) {
                go = new GameObject("@Managers");
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            _instance = go.GetComponent<Managers>();

            // Awake에서 호출되므로 여기서는 실제 초기화만 진행
            if (_instance._input == null) _instance._input = new InputManager();
            if (_instance._pool == null) _instance._pool = new PoolManager();
            if (_instance._trapLogic == null) _instance._trapLogic = new TrapLogicManager();

            // Data.Init();
            // Scene.Init();
            // Pool.Init();
            // Options.Init();
            // Sound.Init();

            // Camera.Init();
        }
    }

    void FixedUpdate() {
        // UI.Update();

        // Combat.Update();
        // Camera.Update();
    }

    public static void Clear() {
        // Scene.Clear();
        // Pool.Clear();
        // Sound.Clear();
        // UI.Clear();

        // Combat.Clear();
        // Camera.Clear();
    }
}
