using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrapLogicManager
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<GameObject> bugs { get; private set; } = new List<GameObject>();
    public List<GameObject> traps { get; private set; } = new List<GameObject>();

    void Update()
    {

    }

    /// <summary>
    /// Destroy된 벌레 오브젝트를 bugs 리스트에서 제거한다.
    /// SpawnManager가 현재 살아있는 벌레 수를 정확히 파악하기 위해 호출한다.
    /// </summary>
    public void CleanupBugs()
    {
        bugs.RemoveAll(b => b == null);
    }

    public void ClearBugsFromMap()
    {
        foreach (var bug in bugs)
        {
            if (bug != null)
                GameObject.Destroy(bug);
        }
        bugs.Clear();
    }

    public void Clear()
    {
        bugs.Clear();
        traps.Clear();
    }

    public void AddBug(GameObject bug)
    {
        if (bug == null || bugs.Contains(bug))
        {
            return;
        }

        bugs.Add(bug);
    }

    public void AddTrap(GameObject trap)
    {
        if (trap == null || traps.Contains(trap))
        {
            return;
        }

        traps.Add(trap);
        TrapController tc = trap.GetComponent<TrapController>();
        if (tc == null) return;

        tc.onBugSensed -= OnBugEnterSensor;
        tc.onBugEscaped -= OnBugExitSensor;
        tc.onBugEaten -= OnBugEaten;
        tc.onTrapClosed -= OnTrapClosed;

        tc.onBugSensed += OnBugEnterSensor;
        tc.onBugEscaped += OnBugExitSensor;
        tc.onBugEaten += OnBugEaten;
        tc.onTrapClosed += OnTrapClosed;
    }

    void OnBugEnterSensor(BugController bug)
    {

    }

    void OnBugExitSensor(BugController bug)
    {

    }

    void OnBugEaten(BugController bug)
    {
        // 벌레의 에너지를 식물에 흡수
        PlantData data = PlantController.Data;
        if (data != null)
        {
            data.CurrentEnergy += bug.EnergyValue;
            Managers.Game.Title.updateEnergy(data.CurrentEnergy);
        }

        bug.Die();
    }

    void OnTrapClosed(List<BugController> bugs)
    {
        Managers.Game.Title.updateEnergy(PlantController.Data.CurrentEnergy);
    }
}
