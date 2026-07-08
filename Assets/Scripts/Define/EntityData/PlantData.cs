using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
[CreateAssetMenu(fileName = "New Plant Data", menuName = "EntityData/PlantData", order = 1)]
public class PlantData : ScriptableObject
{
    [SerializeField] private float attractionRange = 2.0f;
    [SerializeField] private float attractionStrength = 5.0f;
    [SerializeField] private int  biteDamage = 1;
    [SerializeField] private int currentEnergy = 100;
    [SerializeField] private int energyRegenRate = 0;
    [SerializeField] private int energyCostPerBite = 3;
    
    public float AttractionRange {
        get => attractionRange;
        set => attractionRange = value;
    }
    public float AttractionStrength {
        get => attractionStrength;
        set => attractionStrength = value;
    }
    public int BiteDamage {
        get => biteDamage;
        set => biteDamage = value;
    }
    public int CurrentEnergy {
        get => currentEnergy;
        set => currentEnergy = value;
    }
    public int EnergyRegenRate {
        get => energyRegenRate;
        set => energyRegenRate = value;
    }
    public int EnergyCostPerBite {
        get => energyCostPerBite;
        set => energyCostPerBite = value;
    }
}
