using System.IO;
using UnityEngine;

public class DataManager
{
    public int CurrentSlotIndex { get; set; } = 1;

    public static string GetSavePath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, $"save_{slotIndex}.json");
    }

    public void Save() => Save(CurrentSlotIndex);

    public void Save(int slotIndex)
    {
        SaveData data = new SaveData();

        // GameManager 상태 수집
        data.currentMonth = Managers.Game.CurrentSession?.CurrentMonth ?? 3;
        data.monthTimer = Managers.Game.CurrentSession?.MonthTimer ?? 0f;

        // PlantData 상태 수집
        PlantData plant = PlantController.Data;
        if (plant != null)
        {
            data.currentEnergy = plant.CurrentEnergy;
            data.biteDamage = plant.BiteDamage;
            // (향기 수치는 PlantData에서 삭제됨)
            data.energyRegenRate = plant.EnergyRegenRate;
            data.energyCostPerBite = plant.EnergyCostPerBite;
        }

        // 업그레이드 단계 수집
        data.addLeafLevel = PlantController.GetLevel(GameDefines.UpgradeOptions.AddLeaf);
        data.strongBiteLevel = PlantController.GetLevel(GameDefines.UpgradeOptions.StrongBite);
        data.strongScentLevel = PlantController.GetLevel(GameDefines.UpgradeOptions.StrongScent);
        data.deepRootLevel = PlantController.GetLevel(GameDefines.UpgradeOptions.DeepRoot);
        data.sturdyStemLevel = PlantController.GetLevel(GameDefines.UpgradeOptions.SturdyStem);

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        string savePath = GetSavePath(slotIndex);
        File.WriteAllText(savePath, json);
        Debug.Log($"[DataManager] 저장 완료 → {savePath}");
    }

    public SaveData Load() => Load(CurrentSlotIndex);

    public SaveData Load(int slotIndex)
    {
        string savePath = GetSavePath(slotIndex);
        if (!File.Exists(savePath))
        {
            Debug.Log($"[DataManager] 세이브 파일 없음 ({savePath}). 새 게임으로 시작.");
            return null;
        }

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log($"[DataManager] 로드 완료 → Month {data.currentMonth}");
        return data;
    }

    public bool HasSaveFile() => HasSaveFile(CurrentSlotIndex);

    public bool HasSaveFile(int slotIndex) => File.Exists(GetSavePath(slotIndex));

    public void DeleteSave() => DeleteSave(CurrentSlotIndex);

    public void DeleteSave(int slotIndex)
    {
        string savePath = GetSavePath(slotIndex);
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log($"[DataManager] 세이브 파일 삭제 완료 ({savePath}).");
        }
    }
}
