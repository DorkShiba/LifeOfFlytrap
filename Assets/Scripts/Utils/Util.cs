using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public const float EPS = 1e-5f;
    public static float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg;
    }

    public const float MapWidth = 6.8f * 2;  // 맵의 가로 크기
    public const float MapHeight = 4.5f * 2; // 맵의 세로
}