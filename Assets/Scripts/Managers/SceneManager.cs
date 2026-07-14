using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
