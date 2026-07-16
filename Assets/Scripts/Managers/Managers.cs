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
    DataManager _data = new DataManager();
    InputManager _input;
    // OptionsManager _options = new OptionsManager();
    PoolManager _pool;
    ResourceManager _resource = new ResourceManager();
    SceneManager _scene = new SceneManager();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();
    TrapLogicManager _trapLogic;
    GameManager _game;
    SpawnManager _spawn;

    void Awake()
    {
        Init();
    }

    void OnApplicationQuit() {
        // Managers.Data.Save(); // 게임 종료 시 자동 저장 기능 비활성화
        isQuitting = true;
    }

    public static DataManager Data { get { return isQuitting ? null : Instance._data; } }
    public static InputManager Input { get { return isQuitting ? null : Instance._input; } }
    // public static OptionsManager Options { get { return Instance._options; } }
    public static PoolManager Pool { get { return isQuitting? null: Instance._pool; } }
    public static ResourceManager Resource { get { return isQuitting? null: Instance._resource; } }
    public static SceneManager Scene { get { return isQuitting? null: Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return isQuitting? null: Instance._ui; } }
    public static TrapLogicManager TrapLogic { get { return isQuitting? null: Instance._trapLogic; } }
    public static GameManager Game { get { return isQuitting? null: Instance._game; } }
    public static SpawnManager Spawn { get { return isQuitting? null: Instance._spawn; } }
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

    static bool _isInitialized = false;

    public static void Init()
    {
        if (_instance == null && !_isInitialized)
        {
            _isInitialized = true; // 중복 호출 방지 플래그 설정

            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                // AddComponent를 호출하면 유니티가 즉시 Awake()를 실행하여 내부적으로 다시 Init()을 호출합니다.
                // _isInitialized 플래그 덕분에 Awake() 안에서의 Init() 호출은 무시됩니다.
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            _instance = go.GetComponent<Managers>();

            if (_instance._input == null) _instance._input = new InputManager();
            if (_instance._pool == null) _instance._pool = new PoolManager();
            if (_instance._trapLogic == null) _instance._trapLogic = new TrapLogicManager();
            
            // UIManager는 UI 캐싱을 위해 가장 먼저 Init 되어야 함
            _instance._ui.Init();

            // GameManager는 반드시 DataManager 다음에 초기화 (생성자에서 Load 호출)
            if (_instance._game == null) _instance._game = new GameManager();
            // SpawnManager는 GameManager 이후에 초기화 (OnMonthChanged 구독)
            if (_instance._spawn == null)
            {
                _instance._spawn = new SpawnManager();
                _instance._spawn.Init();
            }

            // Data.Init();
            _instance._scene.Init();
            // Pool.Init();
            // Options.Init();
            _instance._sound.Init();

            // Camera.Init();
        }
    }

    void FixedUpdate() {
        // UI.Update();

        // Combat.Update();
        // Camera.Update();
        Game.CurrentSession?.Update(Time.deltaTime);
        Input.Update();
    }

    public static void Clear() {
        Scene?.Clear();
        TrapLogic?.Clear();
        // Pool.Clear();
        Sound?.Clear();
        // UI.Clear();

        // Combat.Clear();
        // Camera.Clear();
    }
}
