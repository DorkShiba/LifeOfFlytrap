using UnityEngine;
using System.IO;

public class DataManager
{
    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");

    public void Save()
    {
        SaveData data = new SaveData();

        // GameManager 상태 수집
        data.currentMonth = Managers.Game.CurrentSession?.CurrentMonth ?? 3;
        data.monthTimer   = Managers.Game.CurrentSession?.MonthTimer ?? 0f;

        // PlantData 상태 수집
        PlantData plant = PlantController.Data;
        if (plant != null)
        {
            data.currentEnergy      = plant.CurrentEnergy;
            data.biteDamage         = plant.BiteDamage;
            // (향기 수치는 PlantData에서 삭제됨)
            data.energyRegenRate    = plant.EnergyRegenRate;
            data.energyCostPerBite  = plant.EnergyCostPerBite;
        }

        // 업그레이드 단계 수집
        data.addLeafLevel    = PlantController.GetLevel(PlantDefines.UpgradeOptions.AddLeaf);
        data.strongBiteLevel  = PlantController.GetLevel(PlantDefines.UpgradeOptions.StrongBite);
        data.strongScentLevel = PlantController.GetLevel(PlantDefines.UpgradeOptions.StrongScent);
        data.deepRootLevel    = PlantController.GetLevel(PlantDefines.UpgradeOptions.DeepRoot);
        data.sturdyStemLevel  = PlantController.GetLevel(PlantDefines.UpgradeOptions.SturdyStem);

        // 월 시작 상태 수집
        if (PlantController.Instance != null)
        {
            data.startMonthEnergy = PlantController.Instance.StartMonthEnergy;
            data.startMonthAddLeafLevel = PlantController.Instance.GetStartMonthLevel(PlantDefines.UpgradeOptions.AddLeaf);
            data.startMonthStrongBiteLevel = PlantController.Instance.GetStartMonthLevel(PlantDefines.UpgradeOptions.StrongBite);
            data.startMonthStrongScentLevel = PlantController.Instance.GetStartMonthLevel(PlantDefines.UpgradeOptions.StrongScent);
            data.startMonthDeepRootLevel = PlantController.Instance.GetStartMonthLevel(PlantDefines.UpgradeOptions.DeepRoot);
            data.startMonthSturdyStemLevel = PlantController.Instance.GetStartMonthLevel(PlantDefines.UpgradeOptions.SturdyStem);
        }

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[DataManager] 저장 완료 → {SavePath}");
    }

    public SaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[DataManager] 세이브 파일 없음. 새 게임으로 시작.");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log($"[DataManager] 로드 완료 → Month {data.currentMonth}");
        return data;
    }

    public bool HasSaveFile() => File.Exists(SavePath);

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("[DataManager] 세이브 파일 삭제 완료.");
        }
    }
}
