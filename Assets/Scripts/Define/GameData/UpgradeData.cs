using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UpgradeCostEntry
{
    public PlantDefines.UpgradeOptions Option;
    [Tooltip("레벨 1부터 7까지의 업그레이드 비용")]
    public List<int> Costs = new List<int>();
}

[CreateAssetMenu(fileName = "New Upgrade Data", menuName = "GameData/UpgradeData", order = 2)]
public class UpgradeData : ScriptableObject
{
    [Header("업그레이드 비용 (Max Level 7)")]
    public List<UpgradeCostEntry> UpgradeCosts = new List<UpgradeCostEntry>();

    [Header("랜딩 확률 (Landing Chance % By Level)")]
    [Tooltip("레벨 1부터 7까지의 랜딩 확률")]
    public List<float> LandingChanceByLevel = new List<float>() { 30f, 40f, 50f, 60f, 70f, 80f, 90f };

    [Header("착지 반경 (Landing Radius By Level)")]
    [Tooltip("레벨 1부터 7까지의 착지 반경")]
    public List<float> LandingRadiusByLevel = new List<float>() { 0.3f, 0.35f, 0.4f, 0.45f, 0.5f, 0.55f, 0.6f };

    [Header("트랩 생성 좌표 (Max 7개)")]
    [Tooltip("트랩이 생성될 좌표 (순서대로 1번 트랩 ~ 7번 트랩)")]
    public List<Vector3> TrapPositions = new List<Vector3>()
    {
        new Vector3(-1.28f, 1.58f, 0),
        new Vector3(2.9f, -1.25f, 0),
        new Vector3(-1.65f, -2.6f, 0),
        new Vector3(-3.8f, 0.23f, 0),
        new Vector3(1.95f, 1.6f, 0),
        new Vector3(-0.26f, -4.00f, 0),
        new Vector3(-5.2f, -3.5f, 0)
    };

    public List<int> GetCosts(PlantDefines.UpgradeOptions option)
    {
        foreach (var entry in UpgradeCosts)
        {
            if (entry.Option == option && entry.Costs.Count > 0)
                return entry.Costs;
        }

        // 에디터에서 세팅하지 않았을 경우를 위한 기본값 (각 7단계)

        int[] defaultCosts;
        switch (option)
        {
            case PlantDefines.UpgradeOptions.AddLeaf:
                defaultCosts = new int[] { 10, 20, 30, 40, 50, 60, 70 };
                break;
            case PlantDefines.UpgradeOptions.StrongBite:
                defaultCosts = new int[] { 5, 10, 15, 20, 25, 30, 35 };
                break;
            case PlantDefines.UpgradeOptions.StrongScent:
                defaultCosts = new int[] { 5, 10, 15, 20, 25, 30, 35 };
                break;
            case PlantDefines.UpgradeOptions.DeepRoot:
                defaultCosts = new int[] { 8, 16, 24, 32, 40, 48, 56 };
                break;
            default:
                defaultCosts = new int[] { 10, 10, 10, 10, 10, 10, 10 };
                break;
        }
        return new List<int>(defaultCosts);
    }
}
