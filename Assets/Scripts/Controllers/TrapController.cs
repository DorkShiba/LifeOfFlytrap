using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrapController : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] public Action<BugController> onBugSensed;
    [SerializeField] public Action<BugController> onBugEscaped;
    [SerializeField] public Action<BugController> onBugEaten;
    [SerializeField] public Action<List<BugController>> onTrapClosed;

    [SerializeField] private List<BugController> OnBiteBugs = new List<BugController>();
    [SerializeField] private List<BugController> Preys = new List<BugController>();
    
    [SerializeField] private Animator anim;
    [SerializeField] private Collider2D sensorZoneCollider;
    [SerializeField] private float biteComboTimeout = 1.0f;

    private float closeTime = 0.0f;
    private Coroutine biteComboTimerCoroutine;

    private const string TrapBiteStateName = "TrapBite";
    private static readonly int CloseTimeHash = Animator.StringToHash("closeTime");

    void Awake()
    {
        Managers.TrapLogic.AddTrap(gameObject);
        anim = GetComponent<Animator>();
        sensorZoneCollider = FindSensorZoneCollider();
    }

    void OnEnable()
    {
        Managers.Input.OnClickPerformed -= Bite;
        Managers.Input.OnClickPerformed += Bite;
    }

    void OnDisable()
    {
        if (biteComboTimerCoroutine != null)
        {
            StopCoroutine(biteComboTimerCoroutine);
            biteComboTimerCoroutine = null;
        }

        if (Managers.Input == null) return;

        Managers.Input.OnClickPerformed -= Bite;
    }

    void OnDestroy()
    {
        if (biteComboTimerCoroutine != null)
        {
            StopCoroutine(biteComboTimerCoroutine);
            biteComboTimerCoroutine = null;
        }

        if (Managers.Input == null) return;

        Managers.Input.OnClickPerformed -= Bite;
    }

    void Update() {
        if (closeTime > Util.EPS) {
            closeTime -= Time.deltaTime;
            if (anim != null) anim.SetFloat(CloseTimeHash, closeTime);
        }
    }

    private static BugController GetBugFromCollider(Collider2D other)
    {
        if (other == null) return null;

        return other.GetComponentInParent<BugController>();
    }

    public void HandleSenseEnter(Collider2D other)
    {
        if (other == null) return;
        
        BugController bug = GetBugFromCollider(other);
        if (bug == null) return;

        onBugSensed?.Invoke(bug);
    }

    public void HandleSenseExit(Collider2D other)
    {
        BugController bug = GetBugFromCollider(other);
        if (bug == null) return;

        onBugEscaped?.Invoke(bug);
    }

    public void HandleBiteEnter(Collider2D other)
    {
        BugController bug = GetBugFromCollider(other);
        if (bug == null) return;
        if (!OnBiteBugs.Contains(bug)) OnBiteBugs.Add(bug);
    }

    public void HandleBiteExit(Collider2D other)
    {
        BugController bug = GetBugFromCollider(other);
        if (bug == null) return;

        if (OnBiteBugs.Contains(bug)) OnBiteBugs.Remove(bug);
    }

    Collider2D FindSensorZoneCollider()
    {
        TrapZoneRelay zoneRelays = GetComponent<TrapZoneRelay>();

        Collider2D zoneCollider = zoneRelays.GetComponent<Collider2D>();
        if (zoneCollider != null) return zoneCollider;

        return null;
    }

    private IEnumerator BiteComboTimer()
    {
        yield return new WaitForSeconds(biteComboTimeout);

        biteComboTimerCoroutine = null;
        closeTime = 5.0f;

        BugController[] eaten = Preys.ToArray();
        foreach (var bug in eaten)
        {
            if (bug != null)
            {
                Preys.Remove(bug);
                onBugEaten?.Invoke(bug);
            }
        }

        if (anim != null) anim.SetFloat(CloseTimeHash, closeTime);
    }

    public void Bite(Vector2 mousePosition)
    {
        // 닫혀있는 경우 물지 못함
        if (closeTime > Util.EPS) return;
        
        // 클릭 위치가 센서 범위 내에 있는지 확인
        sensorZoneCollider = FindSensorZoneCollider();
        if (sensorZoneCollider == null || !sensorZoneCollider.OverlapPoint(mousePosition)) return;

        // 물기 애니메이션 재생
        if (anim != null) anim.Play(TrapBiteStateName, 0, 0f);

        // 연타 타이머 코루틴 초기화
        if (biteComboTimerCoroutine != null) StopCoroutine(biteComboTimerCoroutine);
        biteComboTimerCoroutine = StartCoroutine(BiteComboTimer());

        BugController[] bugsToBite = OnBiteBugs.ToArray();
        foreach (var bug in bugsToBite)
        {
            if (bug != null)
            {
                bool isDead = bug.TakeDamage(PlantController.Data.BiteDamage);
                if (isDead)
                {
                    OnBiteBugs.Remove(bug);
                    Preys.Add(bug);
                }
            }
        }
    }
}
