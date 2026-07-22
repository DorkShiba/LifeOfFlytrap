using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager
{
    public void Init()
    {
    }

    public void LoadScene(string sceneName, Action callback = null)
    {
        Managers.Clear();
        Managers.StartCoroutineManager(LoadSceneCoroutine, (sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single, callback));
    }

    /// <summary>
    /// 화면을 검은색으로 페이드아웃한 뒤 씬을 로드한다. Time.timeScale이 0이어도 동작한다.
    /// </summary>
    public void FadeAndLoadScene(string sceneName, float fadeDuration = 1f, Action callback = null)
    {
        Managers.StartCoroutineManager(FadeAndLoadSceneCoroutine, (sceneName, fadeDuration, callback));
    }

    private IEnumerator FadeAndLoadSceneCoroutine((string sceneName, float fadeDuration, Action callback) args)
    {
        // 최상단 Canvas 생성
        GameObject canvasObj = new GameObject("@FadeOverlay");
        UnityEngine.Object.DontDestroyOnLoad(canvasObj);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // 검은색 Image 생성
        GameObject imageObj = new GameObject("BlackPanel");
        imageObj.transform.SetParent(canvasObj.transform, false);

        Image image = imageObj.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0f);

        RectTransform rect = imageObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // 알파 0 → 1 페이드인 (unscaledDeltaTime 사용)
        float elapsed = 0f;
        while (elapsed < args.fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(elapsed / args.fadeDuration);
            image.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        image.color = new Color(0f, 0f, 0f, 1f);

        // 씬 로드
        Managers.Clear();
        AsyncOperation asyncOper = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
            args.sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        while (!asyncOper.isDone)
        {
            yield return null;
        }

        UnityEngine.Object.Destroy(canvasObj);

        // 씬 전환 전 timeScale이 0으로 멈춰있었으므로 복원
        Time.timeScale = 1f;

        args.callback?.Invoke();
    }

    public void LoadSceneAdditive(string sceneName, Action callback = null)
    {
        Managers.StartCoroutineManager(LoadSceneCoroutine, (sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive, callback));
    }

    private IEnumerator LoadSceneCoroutine((string sceneName, UnityEngine.SceneManagement.LoadSceneMode mode, Action callback) args)
    {
        AsyncOperation asyncOper = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(args.sceneName, args.mode);
        while (!asyncOper.isDone)
        {
            yield return null;
        }

        args.callback?.Invoke();
    }

    public void UnloadScene(string sceneName, Action callback = null)
    {
        Managers.StartCoroutineManager(UnloadSceneCoroutine, (sceneName, callback));
    }

    private IEnumerator UnloadSceneCoroutine((string sceneName, Action callback) args)
    {
        AsyncOperation asyncOper = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(args.sceneName);
        while (!asyncOper.isDone)
        {
            yield return null;
        }

        args.callback?.Invoke();
    }

    public void Clear()
    {
    }
}
