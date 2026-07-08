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

    public static Dictionary<UpgradeOptions, List<int>> UpgradeCosts = new Dictionary<UpgradeOptions, List<int>>()
    {
        { UpgradeOptions.AddLeaf, new List<int> { 10, 20, 30 } },
        { UpgradeOptions.StrongBite, new List<int> { 15, 30, 45 } },
        { UpgradeOptions.StrongScent, new List<int> { 20, 40, 60 } },
        { UpgradeOptions.DeepRoot, new List<int> { 25, 50, 75 } },
        { UpgradeOptions.SturdyStem, new List<int> { 30, 60, 90 } },
    };

    public static List<float> ScentUpgradeByLevel = new List<float>() {
        5.0f, 6.0f, 7.0f, 8.0f, 9.0f,
    };

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
