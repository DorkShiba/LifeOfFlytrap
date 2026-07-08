using System.Collections.Generic;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    private List<TrapController> traps = new List<TrapController>();

    private static PlantController _instance;
    private static PlantData data;
    public static PlantData Data => data;
    [SerializeField] SpriteRenderer spriteRenderer;

    /// <summary>각 업그레이드의 현재 구매 횟수 (0 = 미구매)</summary>
    private Dictionary<PlantDefines.UpgradeOptions, int> upgradeLevels;

    void Start()
    {
        _instance = this;
        data = Managers.Resource.Load<PlantData>("EntityData/PlantData");

        // 업그레이드 단계 초기화
        upgradeLevels = new Dictionary<PlantDefines.UpgradeOptions, int>
        {
            { PlantDefines.UpgradeOptions.AddLeaf,    0 },
            { PlantDefines.UpgradeOptions.StrongBite,  0 },
            { PlantDefines.UpgradeOptions.StrongScent, 0 },
            { PlantDefines.UpgradeOptions.DeepRoot,    0 },
            { PlantDefines.UpgradeOptions.SturdyStem,  0 },
        };

        // 세이브 데이터 복원
        SaveData saved = Managers.Data.Load();
        if (saved != null)
        {
            // PlantData 스탯 복원
            data.CurrentEnergy = saved.currentEnergy;
            data.BiteDamage = saved.biteDamage;
            data.AttractionRange = saved.attractionRange;
            data.AttractionStrength = saved.attractionStrength;
            data.EnergyRegenRate = saved.energyRegenRate;
            data.EnergyCostPerBite = saved.energyCostPerBite;

            // 업그레이드 단계 복원
            upgradeLevels[PlantDefines.UpgradeOptions.AddLeaf] = saved.addLeafLevel;
            upgradeLevels[PlantDefines.UpgradeOptions.StrongBite] = saved.strongBiteLevel;
            upgradeLevels[PlantDefines.UpgradeOptions.StrongScent] = saved.strongScentLevel;
            upgradeLevels[PlantDefines.UpgradeOptions.DeepRoot] = saved.deepRootLevel;
            upgradeLevels[PlantDefines.UpgradeOptions.SturdyStem] = saved.sturdyStemLevel;

            // 트랩 복원: 기본 1개 + AddLeaf 구매 횟수
            int trapCount = 1 + saved.addLeafLevel;
            for (int i = 0; i < trapCount; i++)
                CreateTraps();
        }
        else
        {
            // 신규 게임: 트랩 1개로 시작
            CreateTraps();
        }

        Managers.Game.OnMonthChanged -= HandleMonthChange;
        Managers.Game.OnMonthChanged += HandleMonthChange;

        if (spriteRenderer != null && PlantDefines.PlantSprites.Count > 0)
            spriteRenderer.sprite = PlantDefines.PlantSprites[Mathf.Clamp(traps.Count - 1, 0, PlantDefines.PlantSprites.Count - 1)];
    }

    void Update() { }

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
        var costs = PlantDefines.UpgradeCosts[option];

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

        Managers.Game.Title.updateEnergy(data.CurrentEnergy);
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
        return level >= PlantDefines.UpgradeCosts[option].Count;
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
                data.AttractionRange += 1.0f;
                data.AttractionStrength += 2.0f;
                break;
            case PlantDefines.UpgradeOptions.DeepRoot:
                data.EnergyRegenRate += 1;
                break;
            case PlantDefines.UpgradeOptions.SturdyStem:
                data.EnergyCostPerBite = Mathf.Max(0, data.EnergyCostPerBite - 1);
                break;
        }
    }

    public void CreateTraps()
    {
        Vector3 trapPosition;
        switch (traps.Count)
        {
            case 0: trapPosition = new Vector3(-1.28f, 1.58f, 0); break;
            case 1: trapPosition = new Vector3(2.9f, -1.25f, 0); break;
            case 2: trapPosition = new Vector3(-1.65f, -2.6f, 0); break;
            case 3: trapPosition = new Vector3(-3.8f, 0.23f, 0); break;
            case 4: trapPosition = new Vector3(1.95f, 1.6f, 0); break;
            case 5: trapPosition = new Vector3(-0.26f, -4.00f, 0); break;
            case 6: trapPosition = new Vector3(-5.2f, -3.5f, 0); break;
            default: return;
        }

        GameObject trap = Managers.Resource.Instantiate("Trap", transform, trapPosition);
        if (spriteRenderer != null && PlantDefines.PlantSprites.Count > traps.Count)
            spriteRenderer.sprite = PlantDefines.PlantSprites[traps.Count];

        TrapController tc = trap.GetComponent<TrapController>();
        if (tc != null) traps.Add(tc);
        // TrapController.OnEnable에서 BugController.RegisterTrap이 호출되고,
        // Awake에서 Managers.TrapLogic.AddTrap이 호출되므로 여기서 중복 등록하지 않는다.
    }

    void HandleMonthChange(int newMonth)
    {
        // 월 변경 시 업그레이드 효과 예약 (추후 구현)
    }
}
