using UnityEngine;

public class FlyRotate : MonoBehaviour
{
    [SerializeField] private float speed = 90f; // 초당 회전 각도

    void Update()
    {
        transform.Rotate(0f, 0f, speed * Time.unscaledDeltaTime);
    }
}
