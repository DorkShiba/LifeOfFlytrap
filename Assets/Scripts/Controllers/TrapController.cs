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
    [SerializeField] private float snapDuration = 1.0f;
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
    // 연타 중이거나 닫혀있으면 착륙 불가
    public bool IsAvailable => closeTime <= Util.EPS && !isSnapping;

    private float closeTime = 0.0f;
    private float digestTime = 0.0f;
    private bool isSnapping = false;
    private Coroutine snapDurationCoroutine = null;

    private float currentSnapMaxDigestionTime = 0f;

    // 실제로 갇힌 벌레들만 관리하는 리스트 (연타 중 들어온 벌레 타격 방지)
    private List<BugController> trappedBugs = new List<BugController>();

    private static readonly int CloseTimeHash = Animator.StringToHash("closeTime");
    private static readonly int DigestTimeHash = Animator.StringToHash("digestTime");

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

        if (digestTime > Util.EPS)
        {
            digestTime -= Time.deltaTime;
            if (anim != null) anim.SetFloat(DigestTimeHash, digestTime);
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

        Managers.Sound.PlaySFX("EatBug");
        if (!isSnapping)
        {
            // 에너지 확인 및 소모
            PlantData data = PlantController.Data;
            if (data == null || data.CurrentEnergy < data.EnergyCostPerBite) return;

            data.CurrentEnergy -= data.EnergyCostPerBite;
            Managers.Game.Title?.updateEnergy(data.CurrentEnergy);

            // 스냅 시작
            isSnapping = true;
            currentSnapMaxDigestionTime = 0f;

            // 첫 터치 시 새로 추가된 digestTime 설정
            digestTime = snapDuration;
            if (anim != null)
            {
                anim.SetFloat(DigestTimeHash, digestTime);
                anim.Play("TrapClose", 0, 0f); // 애니메이션 딜레이 없이 즉시 닫힘 상태 진입
            }

            // 가둘 벌레들을 확정 (이 시점에 범위 내에 있던 벌레들만 타겟으로 삼음)
            trappedBugs.Clear();
            trappedBugs.AddRange(OnBiteBugs);

            FreezeAndHideAllBiteBugs();
            snapDurationCoroutine = StartCoroutine(SnapDurationRoutine());
        }
        else
        {
            // 연타 시 디버그 로그 출력
            Debug.Log("터치");
        }

        // 데미지 적용 (벌레가 없으면 건너뜀)
        ApplyBiteDamage();
    }

    private void ApplyBiteDamage()
    {
        BugController[] bugsToBite = trappedBugs.ToArray();
        foreach (var bug in bugsToBite)
        {
            if (bug == null) continue;

            bool isDead = bug.TakeDamage(PlantController.Data.BiteDamage);
            if (isDead)
            {
                currentSnapMaxDigestionTime = Mathf.Max(currentSnapMaxDigestionTime, bug.DigestionTime);
                trappedBugs.Remove(bug);
                OnBiteBugs.Remove(bug);
                onBugEaten?.Invoke(bug);
            }
        }

        // 갇힌 벌레가 모두 사라졌으면 즉시 스냅 종료
        if (isSnapping && trappedBugs.Count == 0)
        {
            EndSnap();
        }
    }

    // 제한 시간 내에 연타 가능
    private IEnumerator SnapDurationRoutine()
    {
        yield return new WaitForSeconds(snapDuration);
        EndSnap();
    }

    /// <summary>
    /// 조건이 충족되면 스냅을 종료. 중복 호출은 isSnapping 가드로 무시.
    /// </summary>
    private void EndSnap()
    {
        if (!isSnapping) return;
        isSnapping = false;

        // 코루틴 정리
        if (snapDurationCoroutine != null) { StopCoroutine(snapDurationCoroutine); snapDurationCoroutine = null; }

        // 살아남은 벌레 해동 후 탈출 처리
        foreach (var bug in trappedBugs.ToArray())
        {
            if (bug != null)
            {
                bug.Unfreeze();
                bug.Show();
                OnBiteBugs.Remove(bug);
                onBugEscaped?.Invoke(bug);
            }
        }
        trappedBugs.Clear();

        // 스냅이 끝났으므로 digestTime 초기화 후 실제 closeTime 설정
        digestTime = 0f;
        if (anim != null) anim.SetFloat(DigestTimeHash, digestTime);
        CloseAndRecover(currentSnapMaxDigestionTime);
    }

    private void FreezeAndHideAllBiteBugs()
    {
        foreach (var bug in OnBiteBugs)
        {
            if (bug != null)
            {
                bug.Freeze();
                bug.Hide();
            }
        }
    }

    /// <summary>
    /// OnDisable / OnDestroy 시 코루틴만 정리 (이벤트/닫힘 처리 없음).
    /// </summary>
    private void CancelSnap()
    {
        if (snapDurationCoroutine != null) { StopCoroutine(snapDurationCoroutine); snapDurationCoroutine = null; }
        isSnapping = false;
    }

    private void CloseAndRecover(float maxDigestionTime)
    {
        // 기본 닫힘 시간(5초) + 먹은 벌레들 중 가장 긴 소화 시간
        float baseCloseTime = trapClosedDuration + maxDigestionTime;

        // 튼튼한 줄기 업그레이드 효과 적용: 잎이 다시 열리는 시간 단축 배율 곱하기
        closeTime = baseCloseTime * PlantDefines.GetTrapReopenTimeMultiplier();

        if (anim != null) anim.SetFloat(CloseTimeHash, closeTime);
    }
}
