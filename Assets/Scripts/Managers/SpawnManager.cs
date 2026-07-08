using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 달(스테이지)별 벌레 스폰을 담당하는 매니저.
/// BugData의 FirstAppearMonth와 SpawnWeight를 기반으로 동적 필터링 및 가중치 랜덤 추첨을 수행한다.
/// </summary>
public class SpawnManager
{
    // Resources 하위에서 BugData를 찾을 경로 (폴더)
    private const string BUG_DATA_PATH = "EntityData";

    // 달에 따라 선형 보간되는 스폰 파라미터
    private const float SPAWN_INTERVAL_MIN = 2f;   // 마지막 스테이지 스폰 쿨타임(초)
    private const float SPAWN_INTERVAL_MAX = 7f;   // 첫 스테이지 스폰 쿨타임(초)
    private const int   MAX_BUGS_MIN       = 5;    // 첫 스테이지 동시 최대 마리수
    private const int   MAX_BUGS_MAX       = 18;   // 마지막 스테이지 동시 최대 마리수
    private const int   TOTAL_MONTHS       = 12;

    // 게임 시작 월 (3월 시작 → 2월 종료)
    // 스테이지 인덱스(0~11) = (월 - START_MONTH + 12) % 12
    private const int   START_MONTH        = 3;

    // 맵 가장자리 스폰 시 트랩과의 최소 거리
    private const float MIN_DIST_FROM_TRAP = 2f;

    private List<BugData> allBugData = new List<BugData>();
    private Coroutine spawnCoroutine;
    private int currentMonth = 1;

    // ──────────────────────────────────────────────
    // 초기화
    // ──────────────────────────────────────────────

    /// <summary>
    /// Resources 폴더에서 모든 BugData를 로드하고, GameManager 월 변경 이벤트를 구독한다.
    /// </summary>
    public void Init()
    {
        LoadAllBugData();

        Managers.Game.OnMonthChanged -= OnMonthChanged;
        Managers.Game.OnMonthChanged += OnMonthChanged;

        // 게임 시작 시 현재 달로 스폰 시작
        currentMonth = Managers.Game.CurrentMonth;
        StartSpawning(currentMonth);
    }

    private void LoadAllBugData()
    {
        BugData[] loaded = Resources.LoadAll<BugData>(BUG_DATA_PATH);
        allBugData = loaded != null ? new List<BugData>(loaded) : new List<BugData>();

        if (allBugData.Count == 0)
        {
            Debug.LogWarning("[SpawnManager] BugData를 하나도 로드하지 못했습니다. " +
                             $"Resources/{BUG_DATA_PATH} 경로를 확인하세요.");
        }
        else
        {
            Debug.Log($"[SpawnManager] BugData {allBugData.Count}개 로드 완료.");
        }
    }

    // ──────────────────────────────────────────────
    // 이벤트 핸들러
    // ──────────────────────────────────────────────

    private void OnMonthChanged(int newMonth)
    {
        currentMonth = newMonth;
        StartSpawning(currentMonth);
    }

    // ──────────────────────────────────────────────
    // 스폰 코루틴 제어
    // ──────────────────────────────────────────────

    private void StartSpawning(int month)
    {
        if (spawnCoroutine != null)
            Managers.StopCoroutineManager(spawnCoroutine);

        spawnCoroutine = Managers.StartCoroutineManager(SpawnLoop);
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (!Managers.Game.IsFrozen)
            {
                // 월 번호 → 스테이지 인덱스(0~11)로 변환하여 난이도 계산
                float t = Mathf.Clamp01((float)MonthToStageIndex(currentMonth) / (TOTAL_MONTHS - 1));
                int   maxBugs       = Mathf.RoundToInt(Mathf.Lerp(MAX_BUGS_MIN, MAX_BUGS_MAX, t));
                float spawnInterval = Mathf.Lerp(SPAWN_INTERVAL_MAX, SPAWN_INTERVAL_MIN, t);

                // null 오브젝트 정리 후 현재 맵 벌레 수 확인
                Managers.TrapLogic.CleanupBugs();
                int currentBugCount = Managers.TrapLogic.bugs.Count;

                if (currentBugCount < maxBugs)
                {
                    TrySpawnBug();
                }

                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                // 게임이 멈춰 있으면 잠시 대기
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    // ──────────────────────────────────────────────
    // 스폰 로직
    // ──────────────────────────────────────────────

    private void TrySpawnBug()
    {
        // 스테이지 인덱스 기준으로 등장 가능한 벌레 필터링
        // (월 번호 직접 비교 시 1월<12월이 되어 연도 경계에서 역전되는 문제 방지)
        int currentStageIndex = MonthToStageIndex(currentMonth);
        List<BugData> eligible = allBugData
            .Where(d => d != null
                     && MonthToStageIndex(d.FirstAppearMonth) <= currentStageIndex
                     && !string.IsNullOrEmpty(d.PrefabName))
            .ToList();

        if (eligible.Count == 0)
        {
            Debug.LogWarning($"[SpawnManager] {currentMonth}달에 스폰 가능한 BugData가 없습니다.");
            return;
        }

        // 가중치 기반 랜덤 추첨
        BugData selected = WeightedRandom(eligible);
        if (selected == null) return;

        // 맵 가장자리 랜덤 위치 선택
        Vector2 spawnPos = GetSpawnPosition();

        GameObject go = Managers.Resource.Instantiate(selected.PrefabName, null, spawnPos);
        BugController bc = go.GetComponent<BugController>();
        if (bc != null)
        {
            float halfW = Util.MapWidth * 0.5f;
            float halfH = Util.MapHeight * 0.5f;
            bc.Init(new Vector2(-halfW, -halfH), new Vector2(halfW, halfH));
        }
    }

    /// <summary>
    /// SpawnWeight를 기반으로 가중치 랜덤 추첨을 수행한다.
    /// </summary>
    private BugData WeightedRandom(List<BugData> candidates)
    {
        float totalWeight = candidates.Sum(d => Mathf.Max(0f, d.SpawnWeight));
        if (totalWeight <= 0f) return candidates[Random.Range(0, candidates.Count)];

        float roll       = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (BugData data in candidates)
        {
            cumulative += Mathf.Max(0f, data.SpawnWeight);
            if (roll <= cumulative)
                return data;
        }

        return candidates[candidates.Count - 1];
    }

    /// <summary>
    /// 실제 월 번호(1~12)를 스테이지 인덱스(0~11)로 변환한다.
    /// 게임은 3월(인덱스 0)에 시작해 2월(인덱스 11)에 끝난다.
    /// </summary>
    private int MonthToStageIndex(int month)
    {
        return (month - START_MONTH + 12) % 12;
    }

    /// <summary>
    /// 맵 가장자리 4방향 중 하나를 선택하고, 트랩과 최소 거리 이상인 위치를 반환한다.
    /// 조건을 만족하는 위치를 못 찾으면 최후의 후보를 반환한다.
    /// </summary>
    private Vector2 GetSpawnPosition()
    {
        float halfW = Util.MapWidth  * 0.5f;
        float halfH = Util.MapHeight * 0.5f;

        const int MAX_ATTEMPTS = 10;
        Vector2 last = PickEdgePoint(halfW, halfH);

        for (int i = 0; i < MAX_ATTEMPTS; i++)
        {
            Vector2 candidate = PickEdgePoint(halfW, halfH);
            last = candidate;

            bool tooClose = false;
            if (Managers.TrapLogic.traps != null)
            {
                foreach (var trap in Managers.TrapLogic.traps)
                {
                    if (trap == null) continue;
                    if (Vector2.Distance(candidate, (Vector2)trap.transform.position) < MIN_DIST_FROM_TRAP)
                    {
                        tooClose = true;
                        break;
                    }
                }
            }

            if (!tooClose) return candidate;
        }

        return last;
    }

    /// <summary>
    /// 맵의 상/하/좌/우 가장자리 중 하나에서 랜덤 위치를 반환한다.
    /// </summary>
    private Vector2 PickEdgePoint(float halfW, float halfH)
    {
        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0:  return new Vector2(Random.Range(-halfW, halfW),  halfH);
            case 1:  return new Vector2(Random.Range(-halfW, halfW), -halfH);
            case 2:  return new Vector2(-halfW, Random.Range(-halfH, halfH));
            default: return new Vector2( halfW, Random.Range(-halfH, halfH));
        }
    }
}
