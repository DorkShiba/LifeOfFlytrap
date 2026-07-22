using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UpgradeCostEntry
{
    public GameDefines.UpgradeOptions Option;
    [Tooltip("레벨 1부터 7까지의 업그레이드 비용")]
    public List<int> Costs = new List<int>();
}

[CreateAssetMenu(fileName = "New Upgrade Data", menuName = "GameData/UpgradeData", order = 2)]
public class UpgradeData : ScriptableObject
{
    public const int MAX_UPGRADE_LEVEL = 7;

    [Header("업그레이드 비용 (Max Level 7)")]
    public List<UpgradeCostEntry> UpgradeCosts;

    [Header("착지 확률 증가량 (Landing Chance Bonus By Level)")]
    [Tooltip("레벨별 착지 확률 증가량. 값 N → 기본 착지 확률 * (1 + N * 0.1). 예) N=8 → 기본 20% * 1.8 = 36%. [0]은 더미값.")]
    public List<float> LandingChanceBonusByLevel;

    [Header("착지 반경 (Landing Radius By Level)")]
    [Tooltip("레벨 1부터 7까지의 착지 반경")]
    public List<float> LandingRadiusByLevel;

    [Header("에너지 회복량 (Energy Regen By Level)")]
    [Tooltip("레벨 1부터 7까지 5초당 에너지 회복량")]
    public List<int> EnergyRegenRateByLevel;

    [Header("트랩 재개방 시간 배율 (Trap Reopen Time Multiplier)")]
    [Tooltip("레벨별 트랩 소화 시간 배율. 값 0.9 → 기본 소화 시간의 90%. [0]은 더미값.")]
    public List<float> TrapReopenTimeMultiplierByLevel;

    [Header("트랩 위치 표 (Max 7개)")]
    [Tooltip("트랩의 위치표 (레벨1의 첫번째 ~ 7번째의 위치)")]
    public List<Vector3> TrapPositions;

    [Header("트랩 레벨별 스프라이트 이미지")]
    public List<Sprite> PlantSprites;

    public List<int> GetCosts(GameDefines.UpgradeOptions option)
    {
        foreach (var entry in UpgradeCosts)
        {
            if (entry.Option == option && entry.Costs.Count > 0)
                return entry.Costs;
        }

        // UpgradeCosts가 비어있을 때 기본값(레벨7까지)

        int[] defaultCosts;
        switch (option)
        {
            case GameDefines.UpgradeOptions.AddLeaf:
                defaultCosts = new int[] { 10, 20, 30, 40, 50, 60, 70 };
                break;
            case GameDefines.UpgradeOptions.StrongBite:
                defaultCosts = new int[] { 5, 10, 15, 20, 25, 30, 35 };
                break;
            case GameDefines.UpgradeOptions.StrongScent:
                defaultCosts = new int[] { 5, 10, 15, 20, 25, 30, 35 };
                break;
            case GameDefines.UpgradeOptions.DeepRoot:
                defaultCosts = new int[] { 8, 16, 24, 32, 40, 48, 56 };
                break;
            default:
                defaultCosts = new int[] { 10, 10, 10, 10, 10, 10, 10 };
                break;
        }
        return new List<int>(defaultCosts);
    }
}