using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public const float EPS = 1e-5f;
    public static float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg;
    }

    public static float BoxMuller(float m, float s, float min, float max)
    {
        float z = Mathf.Sqrt(-2.0f * Mathf.Log(Random.Range(EPS, 1.0f))) * Mathf.Cos(2.0f * Mathf.PI * Random.Range(EPS, 1.0f));
        float value = m + z * s;
        return Mathf.Clamp(value, min, max);
    }

    public static GameObject FindChild(GameObject go, string name, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform != null)
            return transform.gameObject;
        return null;
    }

    public static T FindChild<T>(GameObject go, string name, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        Transform[] transforms = recursive
            ? go.GetComponentsInChildren<Transform>(true)
            : go.GetComponentsInChildren<Transform>(false);

        foreach (Transform transform in transforms)
        {
            if (transform.name == name)
            {
                T component = transform.GetComponent<T>();
                if (component != null)
                    return component;
            }
        }

        return null;
    }
}