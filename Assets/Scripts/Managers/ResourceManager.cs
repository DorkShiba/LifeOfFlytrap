using UnityEngine;
using System.Collections.Generic;
public class ResourceManager {
    private static Dictionary<string, Object> _resources = new Dictionary<string, Object>();
    public T Load<T>(string path) where T : Object {
        if (_resources.ContainsKey(path))
            return _resources[path] as T;
        
        T resource = Resources.Load<T>(path);
        if (resource == null) {
            Debug.Log($"Failed to load resource : {path}");
            return null;
        }

        _resources[path] = resource;
        return resource;
    }

    public GameObject Instantiate(string path, Transform parent = null, Vector3? position = null, Quaternion? rotation = null) {
        GameObject original = null;

        if (_resources.ContainsKey(path)) {
            original = _resources[path] as GameObject;
        }
        else {
            original = Load<GameObject>($"Prefabs/{path}");
            if (original == null) {
                Debug.Log($"Failed to load prefab : {path}");
                return null;
            }
        }

        GameObject go = Instantiate(original, parent, position, rotation);
        if (go != null && _resources.ContainsKey(path)) {
            go.name = path; // 기존 로직 유지 (캐시된 경로는 이름으로 덮어씀)
        }
        return go;
    }

    public GameObject Instantiate(GameObject original, Transform parent = null, Vector3? position = null, Quaternion? rotation = null) {
        if (original == null) {
            Debug.Log($"Failed to instantiate : original GameObject is null");
            return null;
        }

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;

        // 위치나 회전값이 명시적으로 주어졌을 때만 덮어씌움 (기본값은 프리팹 원본 유지)
        if (position != null) {
            if (parent != null) go.transform.localPosition = position.Value;
            else go.transform.position = position.Value;
        }
        
        if (rotation != null) {
            if (parent != null) go.transform.localRotation = rotation.Value;
            else go.transform.rotation = rotation.Value;
        }

        // UI(RectTransform) 객체의 경우 부모 캔버스의 스케일에 의해 로컬 스케일이 망가지는 것을 방지
        if (parent != null && go.GetComponent<RectTransform>() != null) {
            go.transform.localScale = original.transform.localScale;
        }

        return go;
    }

    public void Destroy(GameObject go) {
        if (go == null)
            return;

        Object.Destroy(go);
    }
}
