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

    private static PlantData data;
    public static PlantData Data => data;
    [SerializeField] SpriteRenderer spriteRenderer;

    void Start()
    {
        CreateTraps();
        Managers.Game.OnMonthChanged -= HandleMonthChange;
        Managers.Game.OnMonthChanged += HandleMonthChange;
        data = Managers.Resource.Load<PlantData>("EntityData/PlantData");
        if (spriteRenderer != null && PlantDefines.PlantSprites.Count > 0)
        {
            spriteRenderer.sprite = PlantDefines.PlantSprites[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateTraps()
    {
        Vector3 trapPosition;
        switch (traps.Count)
        {
            case 0:
                trapPosition = new Vector3(-1.28f, 1.58f, 0);
                break;
            case 1:
                trapPosition = new Vector3(2.9f, -1.25f, 0);
                break;
            case 2:
                trapPosition = new Vector3(-1.65f, -2.6f, 0);
                break;
            case 3:
                trapPosition = new Vector3(-3.8f, 0.23f, 0);
                break;
            case 4:
                trapPosition = new Vector3(1.95f, 1.6f, 0);
                break;
            case 5:
                trapPosition = new Vector3(-0.26f, -4.00f, 0);
                break;
            case 6:
                trapPosition = new Vector3(-5.2f, -3.5f, 0);
                break;
            default:
                return;
        }
        GameObject trap = Managers.Resource.Instantiate("Trap", transform, trapPosition);
        spriteRenderer.sprite = PlantDefines.PlantSprites[traps.Count];
        traps.Add(trap.GetComponent<TrapController>());
        Managers.TrapLogic.AddTrap(trap);
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
                data.BiteDamage += 1;
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
