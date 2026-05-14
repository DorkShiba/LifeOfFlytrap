using UnityEngine;
using System.Collections.Generic;

enum UpgradeOptions
{
    AddLeaf,
    StrongBite,
    StrongScent,
    DeepRoot,
    SturdyStem,
}

public class PlantController : MonoBehaviour
{
    private List<TrapController> traps = new List<TrapController>();

    [SerializeField] private static float attractionRange = 2.0f;
    [SerializeField] private static float attractionStrength = 5.0f;
    [SerializeField] private static int  biteDamage = 1;
    [SerializeField] private static int currentHP = 100;
    [SerializeField] private static int hpRegenRate = 1;
    public static float AttractionRange => attractionRange;
    public static float AttractionStrength => attractionStrength;
    public static int BiteDamage => biteDamage;

    void Start()
    {
        CreateTraps();
        Managers.Game.OnMonthChanged -= HandleMonthChange;
        Managers.Game.OnMonthChanged += HandleMonthChange;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateTraps()
    {
        Vector3 trapPosition;
        switch (traps.Count)
        {
            case 0:
                trapPosition = new Vector3(-0.55f, 0.75f, 0);
                break;
            case 1:
                trapPosition = new Vector3(-0.55f, 0.75f, 0);
                break;
            case 2:
                trapPosition = new Vector3(-0.55f, 0.75f, 0);
                break;
            case 3:
                trapPosition = new Vector3(-0.55f, 0.75f, 0);
                break;
            case 4:
                trapPosition = new Vector3(-0.55f, 0.75f, 0);
                break;
            case 5:
                trapPosition = new Vector3(-0.55f, 0.75f, 0);
                break;
            case 6:
                trapPosition = new Vector3(-0.55f, 0.75f, 0);
                break;
            default:
                return;
        }
        GameObject trap = Managers.Resource.Instantiate("Trap", transform, trapPosition);
        traps.Add(trap.GetComponent<TrapController>());
    }

    void HandleMonthChange(int newMonth)
    {
        UpgradeOptions upgrade = GetUpgradeOptionForMonth();
        switch (upgrade)
        {
            case UpgradeOptions.AddLeaf:
                CreateTraps();
                break;
            case UpgradeOptions.StrongBite:
                biteDamage += 1;
                break;
            case UpgradeOptions.StrongScent:
                // Implement strong scent logic
                break;
            case UpgradeOptions.DeepRoot:
                // Implement deep root logic
                break;
            case UpgradeOptions.SturdyStem:
                // Implement sturdy stem logic
                break;
        }
    }

    UpgradeOptions GetUpgradeOptionForMonth()
    {
        return UpgradeOptions.AddLeaf;
    }
}
