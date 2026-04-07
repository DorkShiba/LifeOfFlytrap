using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class TrapController : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent<FlyController> onFlySensed;
    [SerializeField] private UnityEvent<FlyController> onFlyEscaped;
    [SerializeField] private UnityEvent<FlyController> onFlyEaten;
    
    [SerializeField] private bool isSensed = false, onBite = false;
    [SerializeField] private Animator anim;

    private readonly HashSet<FlyController> sensedFlies = new HashSet<FlyController>();

    public int SensedFlyCount => sensedFlies.Count;
    public int EatenFlyCount { get; private set; } //

    void Start() {
        Managers.Input.OnSnapPerformed -= EatAllSensedFlies;  // 이벤트 구독 해제
        Managers.Input.OnSnapPerformed += EatAllSensedFlies;  // 이벤트 구독

        anim = GetComponent<Animator>();
    }

    public void HandleSenseEnter(Collider2D other)
    {
        FlyController fly = GetFlyFromCollider(other);
        if (fly == null)
            return;

        isSensed = true;
        if (sensedFlies.Add(fly))
        {
            Debug.Log($"[Trap] Fly sensed: {fly.name}. Total sensed: {SensedFlyCount}", fly.gameObject);
            onFlySensed?.Invoke(fly);
        }
    }

    public void HandleSenseExit(Collider2D other)
    {
        FlyController fly = GetFlyFromCollider(other);
        if (fly == null)
            return;

        if (sensedFlies.Remove(fly))
        {
            onFlyEscaped?.Invoke(fly);
        }
        isSensed = sensedFlies.Count > 0;
    }

    public void HandleBiteEnter(Collider2D other)
    {
        FlyController fly = GetFlyFromCollider(other);
        if (fly == null)
            return;

        onBite = true;
    }

    public void EatAllSensedFlies()
    {
        Debug.Log($"[Trap] Eating all sensed flies. Count: {SensedFlyCount}");
        foreach (var fly in sensedFlies)
        {
            if (fly != null)
            {
                onFlyEaten?.Invoke(fly);
                fly.gameObject.SetActive(false);
                EatenFlyCount++;
                anim.SetBool("isClose", true);
            }
        }
        sensedFlies.Clear();
    }

    private static FlyController GetFlyFromCollider(Collider2D other)
    {
        if (other == null) {
            return null;
        }

        return other.GetComponentInParent<FlyController>();
    }
}
