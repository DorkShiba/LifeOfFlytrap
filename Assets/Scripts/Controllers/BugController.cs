using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 벌레(Fly, Dragonfly 등)의 공통 AI 베이스 클래스.
/// WANDER -> APPROACH -> LANDED 상태 머신 + 스티어링 기반 이동을 담당한다.
/// 종별 스탯(속도, 지터, 접근 확률, 착지 시간)만 하위 클래스에서 정의하면 된다.
/// </summary>
public abstract class BugController : MonoBehaviour
{

    // (이전 ITrapAttractionProvider 참조 코드 삭제됨)

    [Header("맵 범위 (스포너가 Init으로 세팅)")]
    protected Vector2 mapMin;
    protected Vector2 mapMax;

    protected ITrap targetTrap;

    // 종별로 반드시 정의해야 하는 스탯들
    protected abstract float MoveSpeed { get; }
    protected abstract float BaseApproachChancePercent { get; }

    /// <summary>잡혔을 때 식물에 주는 에너지량. 하위 클래스에서 Data로부터 읽어온다.</summary>
    public abstract int EnergyValue { get; }
    
    /// <summary>트랩에 잡혔을 때 소화에 걸리는 추가 시간. 하위 클래스에서 Data로부터 읽어온다.</summary>
    public abstract float DigestionTime { get; }

    // (이전 TrapAttractionMultiplier 삭제됨)

    // HP 및 Freeze 상태
    private int hp = 1;
    private bool isFrozen = false;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>스포너가 벌레를 생성할 때 맵 경계를 넘겨주며 호출.</summary>
    public virtual void Init(Vector2 min, Vector2 max)
    {
        mapMin = min;
        mapMax = max;
    }

    protected abstract void OnFixedUpdate(float dt);

    protected virtual void FixedUpdate()
    {
        if (isFrozen) return;  // Freeze 중에는 AI 동작 정지
        OnFixedUpdate(Time.fixedDeltaTime);
    }


    /// <summary>오브젝트를 바라볼 방향으로 회전한다.</summary>
    protected void LookAt(Vector3 target)
    {
        Vector2 dir = ((Vector2)(target - transform.position)).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    /// <summary>
    /// 접근할 트랩을 고르는 로직. 기본은 "가장 가까운 사용 가능한 트랩".
    /// 벌레 종류마다 타겟팅 방식이 다르다면(예: 잠자리는 더 먼 트랩도 노림) override.
    /// </summary>
    protected virtual ITrap SelectTrap()
    {
        ITrap nearest = null;
        float best = float.MaxValue;
        Vector2 pos = transform.position;
        var globalTraps = Managers.TrapLogic.traps;
        for (int i = 0; i < globalTraps.Count; i++)
        {
            if (globalTraps[i] == null) continue;
            ITrap t = globalTraps[i].GetComponent<ITrap>();
            if (t == null || !t.IsAvailable) continue;
            float d = ((Vector2)t.Position - pos).sqrMagnitude;
            if (d < best) { best = d; nearest = t; }
        }
        return nearest;
    }

    /// <summary>플레이어가 트랩을 닫아 벌레를 잡았을 때 트랩 스크립트가 호출.</summary>
    public virtual void OnCaught()
    {
        Destroy(gameObject);
    }

    // ─────────────────────────────────────────────
    // HP / 피해 / 상태 API (TrapController에서 호출)
    // ─────────────────────────────────────────────

    /// <summary>HP를 초기화한다. 하위 클래스 Awake에서 Data.HP를 넘겨 호출.</summary>
    protected void InitializeHP(int maxHp)
    {
        hp = maxHp;
    }

    /// <summary>
    /// 피해를 입힌다. HP가 0 이하로 떨어지면 true를 반환한다.
    /// TrapController.ApplyBiteDamage에서 호출한다.
    /// </summary>
    public bool TakeDamage(int damage)
    {
        hp -= damage;
        return hp <= 0;
    }

    /// <summary>벌레를 제자리에 고정한다. 스냅 구간 진입 시 TrapController가 호출.</summary>
    public void Freeze()
    {
        isFrozen = true;
    }

    /// <summary>고정을 해제한다. 스냅 실패(탈출) 시 TrapController가 호출.</summary>
    public void Unfreeze()
    {
        isFrozen = false;
    }

    /// <summary>
    /// 벌레가 먹혔을 때 TrapLogicManager가 호출.
    /// 에너지 수집 후 오브젝트를 제거한다.
    /// </summary>
    public virtual void Die()
    {
        Destroy(gameObject);
    }


}