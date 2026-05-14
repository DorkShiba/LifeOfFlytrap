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
        GameObject go;

        if (_resources.ContainsKey(path)) {
            go = Object.Instantiate(_resources[path] as GameObject, parent);
            go.name = path;
        }
        else {
            GameObject original = Load<GameObject>($"Prefabs/{path}");
            if (original == null) {
                Debug.Log($"Failed to load prefab : {path}");
                return null;
            }

            go = Object.Instantiate(original, parent);
            go.name = original.name;
        }
        if (parent != null) {
            go.transform.localPosition = position ?? Vector3.zero;
            go.transform.localRotation = rotation ?? Quaternion.identity;
        }
        else {
            go.transform.position = position ?? Vector3.zero;
            go.transform.rotation = rotation ?? Quaternion.identity;
        }

        return go;
    }

    public void Destroy(GameObject go) {
        if (go == null)
            return;

        Object.Destroy(go);
    }
}
