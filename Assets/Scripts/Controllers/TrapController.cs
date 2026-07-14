using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour, ITrap
{
    [Header("Events")]
    public Action<BugController> onBugSensed;
    public Action<BugController> onBugEscaped;
    public Action<BugController> onBugEaten;
    public Action<List<BugController>> onTrapClosed;

    // Bite Zone에 현재 들어와 있는 벌레 목록
    [SerializeField] private List<BugController> OnBiteBugs = new List<BugController>();

    [SerializeField] private Animator anim;
    [SerializeField] private Collider2D biteZoneCollider;

    // 스냅 윈도우 최대 지속 시간 (하드 리밋)
    [SerializeField] private float snapDuration = 0.5f;
    // 연타 종료 판정: 마지막 클릭 후 이 시간 동안 입력 없으면 스냅 종료
    [SerializeField] private float comboTimeout = 0.3f;
    // 트랩 닫힘 회복 시간
    [SerializeField] private float trapClosedDuration = 5.0f;

    // ─────────────────────────────────────────────
    // ITrap 구현
    // ─────────────────────────────────────────────

    [Header("ITrap 설정")]
    [Tooltip("벨레가 착지하는 반지름(유닛)")]
    [SerializeField] private float landingRadius = 0.3f;

    public Vector3 Position => transform.position;
    public Vector2 ColliderSize
    {
        get
        {
            var col = GetComponent<BoxCollider2D>();
            return col != null ? col.size : new Vector2(3.8f, 3.4f);
        }
    }
    public float LandingRadius => PlantDefines.GetCurrentLandingRadius();
    public bool IsAvailable => closeTime <= Util.EPS;

    private float closeTime = 0.0f;
    private bool isSnapping = false;
    // 두 코루틴 중 먼저 발화하는 쪽이 스냅을 종료한다.
    private Coroutine snapDurationCoroutine = null;  // 조건①: 500ms 하드 리밋
    private Coroutine comboTimeoutCoroutine = null;  // 조건②: 연타 종료

    private float currentSnapMaxDigestionTime = 0f;

    private const string TrapBiteStateName = "TrapBite";
    private static readonly int CloseTimeHash = Animator.StringToHash("closeTime");

    void Awake()
    {
        Managers.TrapLogic.AddTrap(gameObject);
        anim = GetComponent<Animator>();
        biteZoneCollider = GetComponentInChildren<TrapZoneRelay>()?.GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        Managers.Input.OnClickPerformed -= Bite;
        Managers.Input.OnClickPerformed += Bite;
    }

    void OnDisable()
    {
        CancelSnap();
        if (Managers.Input == null) return;
        Managers.Input.OnClickPerformed -= Bite;
    }

    void OnDestroy()
    {
        CancelSnap();
        if (Managers.Input == null) return;
        Managers.Input.OnClickPerformed -= Bite;
    }

    void Update()
    {
        if (closeTime > Util.EPS)
        {
            closeTime -= Time.deltaTime;
            if (anim != null) anim.SetFloat(CloseTimeHash, closeTime);
        }
    }

    private static BugController GetBugFromCollider(Collider2D other)
    {
        if (other == null) return null;
        return other.GetComponentInParent<BugController>();
    }

    public void HandleBiteEnter(Collider2D other)
    {
        BugController bug = GetBugFromCollider(other);
        if (bug == null) return;
        if (!OnBiteBugs.Contains(bug)) OnBiteBugs.Add(bug);
        onBugSensed?.Invoke(bug);
    }

    public void HandleBiteExit(Collider2D other)
    {
        BugController bug = GetBugFromCollider(other);
        if (bug == null) return;
        if (OnBiteBugs.Contains(bug)) OnBiteBugs.Remove(bug);
        onBugEscaped?.Invoke(bug);
    }

    public void Bite(Vector2 mousePosition)
    {
        // 닫혀있고 스냅 중도 아닌 경우 물지 못함
        if (closeTime > Util.EPS && !isSnapping) return;

        // 클릭 위치가 센서 범위 내에 있는지 확인
        if (biteZoneCollider == null || !biteZoneCollider.OverlapPoint(mousePosition)) return;

        if (!isSnapping)
        {
            // 에너지 확인 및 소모
            PlantData data = PlantController.Data;
            if (data == null || data.CurrentEnergy < data.EnergyCostPerBite) return;
            data.CurrentEnergy -= data.EnergyCostPerBite;
            Managers.Game.Title.updateEnergy(data.CurrentEnergy);

            // 스냅 시작 — 하드 리밋 코루틴 (리셋 없음)
            isSnapping = true;
            currentSnapMaxDigestionTime = 0f;
            FreezeAllBiteBugs();
            snapDurationCoroutine = StartCoroutine(SnapDurationRoutine());
        }

        // 물기 애니메이션 재생 (매 클릭마다)
        if (anim != null) anim.Play(TrapBiteStateName, 0, 0f);

        // 조건② 연타 타이머 리셋 (클릭할 때마다 다시 시작)
        if (comboTimeoutCoroutine != null) StopCoroutine(comboTimeoutCoroutine);
        comboTimeoutCoroutine = StartCoroutine(ComboTimeoutRoutine());

        // 데미지 적용 (벌레가 없으면 건너뜀)
        ApplyBiteDamage();
    }

    private void ApplyBiteDamage()
    {
        BugController[] bugsToBite = OnBiteBugs.ToArray();
        foreach (var bug in bugsToBite)
        {
            if (bug == null) continue;

            bool isDead = bug.TakeDamage(PlantController.Data.BiteDamage);
            if (isDead)
            {
                currentSnapMaxDigestionTime = Mathf.Max(currentSnapMaxDigestionTime, bug.DigestionTime);
                OnBiteBugs.Remove(bug);
                onBugEaten?.Invoke(bug);
            }
        }

        // 조건③: Bite Zone 내 벌레가 모두 사라졌으면 즉시 스냅 종료
        if (isSnapping && OnBiteBugs.Count == 0)
        {
            EndSnap();
        }
    }

    // 조건①: 500ms 하드 리밋
    private IEnumerator SnapDurationRoutine()
    {
        yield return new WaitForSeconds(snapDuration);
        EndSnap();
    }

    // 조건②: 연타 종료 — 마지막 클릭 후 comboTimeout 초 경과 시 스냅 종료
    private IEnumerator ComboTimeoutRoutine()
    {
        yield return new WaitForSeconds(comboTimeout);
        EndSnap();
    }

    /// <summary>
    /// 세 조건 중 먼저 충족된 쪽이 호출. 중복 호출은 isSnapping 가드로 무시.
    /// </summary>
    private void EndSnap()
    {
        if (!isSnapping) return;
        isSnapping = false;

        // 두 코루틴 모두 정리
        if (snapDurationCoroutine != null) { StopCoroutine(snapDurationCoroutine); snapDurationCoroutine = null; }
        if (comboTimeoutCoroutine != null) { StopCoroutine(comboTimeoutCoroutine); comboTimeoutCoroutine = null; }

        // 살아남은 벌레 해동 후 탈출 처리
        foreach (var bug in OnBiteBugs.ToArray())
        {
            if (bug != null)
            {
                bug.Unfreeze();
                OnBiteBugs.Remove(bug);
                onBugEscaped?.Invoke(bug);
            }
        }

        CloseAndRecover(currentSnapMaxDigestionTime);
    }

    private void FreezeAllBiteBugs()
    {
        foreach (var bug in OnBiteBugs)
        {
            if (bug != null) bug.Freeze();
        }
    }

    /// <summary>
    /// OnDisable / OnDestroy 시 코루틴만 정리 (이벤트/닫힘 처리 없음).
    /// </summary>
    private void CancelSnap()
    {
        if (snapDurationCoroutine != null) { StopCoroutine(snapDurationCoroutine); snapDurationCoroutine = null; }
        if (comboTimeoutCoroutine != null) { StopCoroutine(comboTimeoutCoroutine); comboTimeoutCoroutine = null; }
        isSnapping = false;
    }

    private void CloseAndRecover(float maxDigestionTime)
    {
        // 먹은 벌레가 있다면 가장 긴 소화 시간을, 하나도 못 먹었다면 기본 닫힘 시간을 적용
        closeTime = maxDigestionTime > 0f ? maxDigestionTime : trapClosedDuration;
        if (anim != null) anim.SetFloat(CloseTimeHash, closeTime);
    }
}
