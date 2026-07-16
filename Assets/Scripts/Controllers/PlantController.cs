using System.Collections.Generic;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    private List<TrapController> traps = new List<TrapController>();

    private static PlantController _instance;
    public static PlantController Instance => _instance;
    private static PlantData data;
    public static PlantData Data => data;
    [SerializeField] SpriteRenderer spriteRenderer;

    /// <summary>각 업그레이드의 현재 구매 횟수 (0 = 미구매)</summary>
    private Dictionary<PlantDefines.UpgradeOptions, int> upgradeLevels;

    // 월 시작 시점의 에너지와 업그레이드 레벨
    public int StartMonthEnergy { get; private set; }
    private Dictionary<PlantDefines.UpgradeOptions, int> startMonthUpgradeLevels;

    public int GetStartMonthLevel(PlantDefines.UpgradeOptions option)
    {
        if (startMonthUpgradeLevels == null) return 1;
        return startMonthUpgradeLevels.TryGetValue(option, out int level) ? level : 1;
    }

    public void UpdateStartMonthState()
    {
        if (data != null)
            StartMonthEnergy = data.CurrentEnergy;
        
        if (upgradeLevels != null)
        {
            foreach (var kvp in upgradeLevels)
            {
                startMonthUpgradeLevels[kvp.Key] = kvp.Value;
            }
        }
    }

    public void RestoreStartMonthState()
    {
        if (data != null)
            data.CurrentEnergy = StartMonthEnergy;
        
        if (upgradeLevels != null && startMonthUpgradeLevels != null)
        {
            var keys = new System.Collections.Generic.List<PlantDefines.UpgradeOptions>(startMonthUpgradeLevels.Keys);
            foreach (var key in keys)
            {
                upgradeLevels[key] = startMonthUpgradeLevels[key];
            }
        }

        int trapCount = upgradeLevels[PlantDefines.UpgradeOptions.AddLeaf];
        while (traps.Count > trapCount)
        {
            var trap = traps[traps.Count - 1];
            traps.RemoveAt(traps.Count - 1);
            if (Managers.TrapLogic.traps.Contains(trap.gameObject))
                Managers.TrapLogic.traps.Remove(trap.gameObject);
            Destroy(trap.gameObject);
        }

        if (spriteRenderer != null && PlantDefines.PlantSprites.Count > 0)
            spriteRenderer.sprite = PlantDefines.PlantSprites[Mathf.Clamp(traps.Count - 1, 0, PlantDefines.PlantSprites.Count - 1)];

        Managers.Game.Title?.updateEnergy(data.CurrentEnergy);
    }

    void Start()
    {
        _instance = this;
        
        // 1. 에셋 원본이 오염되지 않도록 원본을 불러와서 복제(Clone)하여 런타임용으로 사용합니다.
        PlantData originalData = Managers.Resource.Load<PlantData>("GameData/PlantData");
        data = Instantiate(originalData);

        // 업그레이드 단계 초기화 (기본값 레벨 1)
        upgradeLevels = new Dictionary<PlantDefines.UpgradeOptions, int>
        {
            { PlantDefines.UpgradeOptions.AddLeaf,     1 },
            { PlantDefines.UpgradeOptions.StrongBite,  1 },
            { PlantDefines.UpgradeOptions.StrongScent, 1 },
            { PlantDefines.UpgradeOptions.DeepRoot,    1 },
            { PlantDefines.UpgradeOptions.SturdyStem,  1 },
        };

        startMonthUpgradeLevels = new Dictionary<PlantDefines.UpgradeOptions, int>
        {
            { PlantDefines.UpgradeOptions.AddLeaf,     1 },
            { PlantDefines.UpgradeOptions.StrongBite,  1 },
            { PlantDefines.UpgradeOptions.StrongScent, 1 },
            { PlantDefines.UpgradeOptions.DeepRoot,    1 },
            { PlantDefines.UpgradeOptions.SturdyStem,  1 },
        };
        StartMonthEnergy = data.CurrentEnergy;

        // 2. 세이브 데이터 로드 시도
        SaveData saved = Managers.Data.Load();
        if (saved != null)
        {
            // 세이브가 있으면 세이브 데이터로 덮어씌움
            data.CurrentEnergy = saved.currentEnergy;
            data.BiteDamage = saved.biteDamage;
            data.EnergyRegenRate = saved.energyRegenRate;
            data.EnergyCostPerBite = saved.energyCostPerBite;

            // 하위 호환을 위해 세이브 값이 0이면 1로 보정
            upgradeLevels[PlantDefines.UpgradeOptions.AddLeaf] = Mathf.Max(1, saved.addLeafLevel);
            upgradeLevels[PlantDefines.UpgradeOptions.StrongBite] = Mathf.Max(1, saved.strongBiteLevel);
            upgradeLevels[PlantDefines.UpgradeOptions.StrongScent] = Mathf.Max(1, saved.strongScentLevel);
            upgradeLevels[PlantDefines.UpgradeOptions.DeepRoot] = Mathf.Max(1, saved.deepRootLevel);
            upgradeLevels[PlantDefines.UpgradeOptions.SturdyStem] = Mathf.Max(1, saved.sturdyStemLevel);

            // 월 시작 상태 복원
            StartMonthEnergy = saved.startMonthEnergy;
            startMonthUpgradeLevels[PlantDefines.UpgradeOptions.AddLeaf] = Mathf.Max(1, saved.startMonthAddLeafLevel);
            startMonthUpgradeLevels[PlantDefines.UpgradeOptions.StrongBite] = Mathf.Max(1, saved.startMonthStrongBiteLevel);
            startMonthUpgradeLevels[PlantDefines.UpgradeOptions.StrongScent] = Mathf.Max(1, saved.startMonthStrongScentLevel);
            startMonthUpgradeLevels[PlantDefines.UpgradeOptions.DeepRoot] = Mathf.Max(1, saved.startMonthDeepRootLevel);
            startMonthUpgradeLevels[PlantDefines.UpgradeOptions.SturdyStem] = Mathf.Max(1, saved.startMonthSturdyStemLevel);

            // 트랩 복원: AddLeaf 레벨만큼 트랩 생성
            int trapCount = upgradeLevels[PlantDefines.UpgradeOptions.AddLeaf];
            for (int i = 0; i < trapCount; i++)
                CreateTraps();
        }
        else
        {
            // 세이브가 없으면 위에서 복제해둔 원본(ScriptableObject)의 수치를 그대로 사용
            int trapCount = upgradeLevels[PlantDefines.UpgradeOptions.AddLeaf];
            for (int i = 0; i < trapCount; i++)
                CreateTraps();
        }

        // 초기화 및 세이브 복원이 끝난 직후, 로드된 에너지 값을 UI에 반영합니다.
        if (Managers.Game.Title != null)
        {
            Managers.Game.Title.updateEnergy(data.CurrentEnergy);
        }

        if (Managers.Game.CurrentSession != null)
        {
            Managers.Game.CurrentSession.OnMonthChanged -= HandleMonthChange;
            Managers.Game.CurrentSession.OnMonthChanged += HandleMonthChange;
        }

        if (spriteRenderer != null && PlantDefines.PlantSprites.Count > 0)
            spriteRenderer.sprite = PlantDefines.PlantSprites[Mathf.Clamp(traps.Count - 1, 0, PlantDefines.PlantSprites.Count - 1)];
    }

    private float regenTimer = 0f;
    void Update() 
    { 
        if (Managers.Game.CurrentSession != null && Managers.Game.CurrentSession.IsFrozen) return;

        regenTimer += Time.deltaTime;
        if (regenTimer >= 5f)
        {
            regenTimer = 0f;
            int regenAmount = PlantDefines.GetCurrentEnergyRegenRate();
            if (regenAmount > 0 && data != null)
            {
                data.CurrentEnergy += regenAmount;
                Managers.Game.Title?.updateEnergy(data.CurrentEnergy);
            }
        }
    }

    // ─────────────────────────────────────────────
    // 업그레이드 API
    // ─────────────────────────────────────────────

    /// <summary>
    /// 업그레이드를 시도합니다.
    /// 에너지가 충분하고 최대 레벨이 아닐 때만 성공합니다.
    /// </summary>
    /// <returns>업그레이드 성공 여부</returns>
    public bool TryUpgrade(PlantDefines.UpgradeOptions option)
    {
        int currentLevel = upgradeLevels[option];
        var costs = PlantDefines.GetUpgradeCosts(option);

        if (currentLevel >= costs.Count)
        {
            Debug.Log($"[PlantController] {option} 최대 레벨 도달.");
            return false;
        }

        int cost = costs[currentLevel];
        if (data.CurrentEnergy < cost)
        {
            Debug.Log($"[PlantController] 에너지 부족 ({data.CurrentEnergy} / {cost})");
            return false;
        }

        data.CurrentEnergy -= cost;
        ApplyUpgradeEffect(option);
        upgradeLevels[option]++;

        Managers.Game.Title?.updateEnergy(data.CurrentEnergy);
        Debug.Log($"[PlantController] {option} 업그레이드 → Lv{upgradeLevels[option]}");
        return true;
    }

    /// <summary>특정 업그레이드의 현재 레벨을 반환합니다.</summary>
    public static int GetLevel(PlantDefines.UpgradeOptions option)
    {
        if (_instance == null) return 0;
        return _instance.upgradeLevels.TryGetValue(option, out int level) ? level : 0;
    }

    /// <summary>특정 업그레이드가 최대 레벨인지 확인합니다.</summary>
    public static bool IsMaxLevel(PlantDefines.UpgradeOptions option)
    {
        int level = GetLevel(option);
        return level >= PlantDefines.GetUpgradeCosts(option).Count;
    }

    // ─────────────────────────────────────────────
    // 내부 로직
    // ─────────────────────────────────────────────

    private void ApplyUpgradeEffect(PlantDefines.UpgradeOptions option)
    {
        switch (option)
        {
            case PlantDefines.UpgradeOptions.AddLeaf:
                CreateTraps();
                break;
            case PlantDefines.UpgradeOptions.StrongBite:
                data.BiteDamage += 1;
                break;
            case PlantDefines.UpgradeOptions.StrongScent:
                // 향기(StrongScent) 업그레이드는 이제 PlantDefines.GetCurrentLandingChance() / Radius()를 통해 동적으로 레벨을 참조하므로 별도의 상태값 갱신이 불필요함
                break;
            case PlantDefines.UpgradeOptions.DeepRoot:
                // 에너지 회복량(EnergyRegenRate) 업그레이드는 이제 PlantDefines.GetCurrentEnergyRegenRate()를 통해 동적으로 레벨을 참조하므로 별도의 상태값 갱신이 불필요함
                break;
            case PlantDefines.UpgradeOptions.SturdyStem:
                // 잎이 다시 열리는 시간을 줄이는 효과는 TrapController에서 동적으로 레벨을 참조하여 적용합니다.
                break;
        }
    }

    public void CreateTraps()
    {
        Vector3 trapPosition = Vector3.zero;
        if (PlantDefines.Data != null && traps.Count < PlantDefines.Data.TrapPositions.Count)
        {
            trapPosition = PlantDefines.Data.TrapPositions[traps.Count];
        }
        else
        {
            Debug.LogWarning("트랩 위치 데이터가 더 이상 없거나 UpgradeData를 로드하지 못했습니다.");
            return;
        }

        GameObject trap = Managers.Resource.Instantiate("Plant/Trap", transform, trapPosition);
        if (spriteRenderer != null && PlantDefines.PlantSprites.Count > traps.Count)
            spriteRenderer.sprite = PlantDefines.PlantSprites[traps.Count];

        TrapController tc = trap.GetComponent<TrapController>();
        if (tc != null) traps.Add(tc);
        // Awake에서 Managers.TrapLogic.AddTrap이 호출되므로 여기서 중복 등록하지 않는다.
    }

    void HandleMonthChange(int newMonth)
    {
        // 월 변경 시 업그레이드 효과 예약 (추후 구현)
        UpdateStartMonthState();
    }
}
