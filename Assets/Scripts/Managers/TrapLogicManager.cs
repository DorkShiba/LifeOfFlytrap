using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public class TrapLogicManager
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<GameObject> bugs { get; private set; } = new List<GameObject>();
    public List<GameObject> traps { get; private set; } =  new List<GameObject>();

    void Update()
    {

    }

    public void AddBug(GameObject bug)
    {
        if (bug == null || bugs.Contains(bug)) {
            return;
        }

        bugs.Add(bug);
    }

    public void AddTrap(GameObject trap)
    {
        if (trap == null || traps.Contains(trap)) {
            return;
        }

        traps.Add(trap);
        trap.GetComponent<TrapController>().onBugSensed -= OnBugEnterSensor;
        trap.GetComponent<TrapController>().onBugEscaped -= OnBugExitSensor;
        trap.GetComponent<TrapController>().onBugEaten -= OnBugEaten;
        trap.GetComponent<TrapController>().onTrapClosed -= OnTrapClosed;

        trap.GetComponent<TrapController>().onBugSensed += OnBugEnterSensor;
        trap.GetComponent<TrapController>().onBugEscaped += OnBugExitSensor;
        trap.GetComponent<TrapController>().onBugEaten += OnBugEaten;
        trap.GetComponent<TrapController>().onTrapClosed += OnTrapClosed;
    }

    void OnBugEnterSensor(BugController bug)
    {

    }

    void OnBugExitSensor(BugController bug)
    {

    }

    void OnBugEaten(BugController bug)
    {
        bug.Die();
    }

    void OnTrapClosed(List<BugController> bugs)
    {
        
    }
}
