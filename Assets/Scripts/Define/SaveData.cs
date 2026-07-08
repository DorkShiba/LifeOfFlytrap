using System;

[Serializable]
public class SaveData
{
    // GameManager 상태
    public int currentMonth;
    public float monthTimer;

    // PlantData 상태
    public int currentEnergy;
    public int biteDamage;
    public float attractionRange;
    public float attractionStrength;
    public int energyRegenRate;
    public int energyCostPerBite;

    // 업그레이드 단계
    public int addLeafLevel;
    public int strongBiteLevel;
    public int strongScentLevel;
    public int deepRootLevel;
    public int sturdyStemLevel;
}
