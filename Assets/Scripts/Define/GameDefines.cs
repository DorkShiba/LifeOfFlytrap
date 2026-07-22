using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDefines
{
    public enum UpgradeOptions
    {
        AddLeaf,
        StrongBite,
        StrongScent,
        DeepRoot,
        SturdyStem,
    }
    public static int MaxUpgradeLevel = UpgradeData.MAX_UPGRADE_LEVEL;

    private static UpgradeData _data;
    public static UpgradeData Data
    {
        get
        {
            if (_data == null)
            {
                // Resources/GameData/UpgradeData.asset 으로 로드 시도
                _data = Resources.Load<UpgradeData>("GameData/UpgradeData");
                if (_data == null)
                {
                    Debug.LogError("UpgradeData 에셋을 Resources/GameData/ 폴더에서 찾을 수 없습니다.");
                }
            }
            return _data;
        }
    }

    public static List<int> GetUpgradeCosts(UpgradeOptions option)
    {
        return Data != null ? Data.GetCosts(option) : new List<int>();
    }

    /// <summary>
    /// 벌레의 BaseApproachChancePercent를 기반으로 StrongScent 보너스를 적용한 최종 접근 확률을 반환합니다.
    /// </summary>
    public static float GetCurrentLandingChance(float baseChance)
    {
        int level = PlantController.GetLevel(UpgradeOptions.StrongScent);
        if (Data == null || Data.LandingChanceBonusByLevel.Count == 0) return baseChance;
        level = Mathf.Clamp(level, 0, Data.LandingChanceBonusByLevel.Count - 1);
        float bonus = Data.LandingChanceBonusByLevel[level]; // [0] = 더미, [1]~[7] = 레벨별 증가량
        return baseChance * (1f + bonus * 0.1f);   // 예) baseChance=20, bonus=8 → 20 * 1.8 = 36%
    }

    public static float GetCurrentLandingRadius()
    {
        int level = PlantController.GetLevel(UpgradeOptions.StrongScent);
        if (Data == null || Data.LandingRadiusByLevel.Count == 0) return 0.3f;
        level = Mathf.Clamp(level, 0, Data.LandingRadiusByLevel.Count - 1);
        return Data.LandingRadiusByLevel[level]; // [0] = 더미, [1]~[7] = 레벨별 값
    }

    public static int GetCurrentEnergyRegenRate()
    {
        int level = PlantController.GetLevel(UpgradeOptions.DeepRoot);
        if (Data == null || Data.EnergyRegenRateByLevel.Count == 0) return 0;
        level = Mathf.Clamp(level, 0, Data.EnergyRegenRateByLevel.Count - 1);
        return Data.EnergyRegenRateByLevel[level]; // [0] = 더미, [1]~[7] = 레벨별 값
    }

    public static float GetTrapReopenTimeMultiplier()
    {
        int level = PlantController.GetLevel(UpgradeOptions.SturdyStem);
        if (Data == null || Data.TrapReopenTimeMultiplierByLevel.Count == 0) return 1.0f;
        level = Mathf.Clamp(level, 0, Data.TrapReopenTimeMultiplierByLevel.Count - 1);
        return Data.TrapReopenTimeMultiplierByLevel[level]; // [0] = 더미, [1]~[7] = 레벨별 값
    }

    public static Sprite GetCurrentPlantSprites(int AddLeafLevel)
    {
        if (Data == null || Data.PlantSprites == null || Data.PlantSprites.Count == 0) return null;
        int index = Mathf.Clamp(AddLeafLevel, 0, Data.PlantSprites.Count - 1);
        return Data.PlantSprites[index];
    }

    /// <summary>해당 달의 목표 에너지를 반환합니다.</summary>
    public static int GetRequiredEnergy(int month)
    {
        var constraints = GameData.Instance?.ClearConstraints;
        if (constraints == null || month < 0 || month >= constraints.Count) return 0;
        return constraints[month];
    }
}
