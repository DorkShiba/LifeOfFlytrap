using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantDefines
{
    public enum UpgradeOptions
    {
        AddLeaf,
        StrongBite,
        StrongScent,
        DeepRoot,
        SturdyStem,
    }

    private static UpgradeData _data;
    public static UpgradeData Data
    {
        get
        {
            if (_data == null)
            {
                // Resources/GameData/UpgradeData.asset 으로 저장한다고 가정
                _data = Resources.Load<UpgradeData>("GameData/UpgradeData");
                if (_data == null)
                {
                    Debug.LogError("UpgradeData 에셋을 Resources/GameData/ 폴더에서 찾을 수 없습니다!");
                }
            }
            return _data;
        }
    }

    public static List<int> GetUpgradeCosts(UpgradeOptions option)
    {
        return Data != null ? Data.GetCosts(option) : new List<int>();
    }

    public static float GetCurrentLandingChance()
    {
        int level = PlantController.GetLevel(UpgradeOptions.StrongScent);
        if (Data == null || Data.LandingChanceByLevel.Count == 0) return 30f;
        level = Mathf.Clamp(level, 0, Data.LandingChanceByLevel.Count - 1);
        return Data.LandingChanceByLevel[level];
    }

    public static float GetCurrentLandingRadius()
    {
        int level = PlantController.GetLevel(UpgradeOptions.StrongScent);
        if (Data == null || Data.LandingRadiusByLevel.Count == 0) return 0.3f;
        level = Mathf.Clamp(level, 0, Data.LandingRadiusByLevel.Count - 1);
        return Data.LandingRadiusByLevel[level];
    }

    public static int GetCurrentEnergyRegenRate()
    {
        int level = PlantController.GetLevel(UpgradeOptions.DeepRoot);
        if (Data == null || Data.EnergyRegenRateByLevel.Count == 0) return 0;
        level = Mathf.Clamp(level - 1, 0, Data.EnergyRegenRateByLevel.Count - 1);
        return Data.EnergyRegenRateByLevel[level];
    }

    public static float GetTrapReopenTimeMultiplier()
    {
        int level = PlantController.GetLevel(UpgradeOptions.SturdyStem);
        // 레벨당 10%씩 열리는 시간 단축 (기본 1.0, 2레벨 0.9, ... 최소 0.2)
        float multiplier = 1.0f - (level - 1) * 0.1f;
        return Mathf.Clamp(multiplier, 0.2f, 1.0f);
    }

    public static List<Sprite> PlantSprites = new List<Sprite>()
    {
        Resources.Load<Sprite>("Images/Plant/Body1"),
        Resources.Load<Sprite>("Images/Plant/Body2"),
        Resources.Load<Sprite>("Images/Plant/Body3"),
        Resources.Load<Sprite>("Images/Plant/Body4"),
        Resources.Load<Sprite>("Images/Plant/Body5"),
        Resources.Load<Sprite>("Images/Plant/Body6"),
        Resources.Load<Sprite>("Images/Plant/Body7"),
    };
}
