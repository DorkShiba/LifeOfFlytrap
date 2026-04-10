using System.Collections.Generic;
using UnityEngine;
using System;

public class TrapController : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] public Action<BugController> onBugSensed;
    [SerializeField] public Action<BugController> onBugEscaped;
    [SerializeField] public Action<BugController> onBugEaten;

    [SerializeField] private List<BugController> OnBiteBugs = new List<BugController>();
    
    [SerializeField] private Animator anim;
    [SerializeField] private Collider2D sensorZoneCollider;

    private float closeTime = 0.0f;
    [SerializeField] private float attractionRange = 2.0f;
    [SerializeField] private float attractionStrength = 5.0f;
    public float AttractionRange => attractionRange;
    public float AttractionStrength => attractionStrength;

    private static readonly int CloseTimeHash = Animator.StringToHash("closeTime");

    public int EatenBugCount { get; private set; } //

    void Awake()
    {
        Managers.TrapLogic.AddTrap(gameObject);
        anim = GetComponent<Animator>();
        sensorZoneCollider = FindSensorZoneCollider();
    }

    void OnEnable()
    {
        Managers.Input.OnClickPerformed -= Bite;
        Managers.Input.OnClickPerformed += Bite;
    }

    void OnDisable()
    {
        if (Managers.Input == null)
        {
            return;
        }

        Managers.Input.OnClickPerformed -= Bite;
    }

    void OnDestroy()
    {
        if (Managers.Input == null)
        {
            return;
        }

        Managers.Input.OnClickPerformed -= Bite;
    }

    void Update() {
        if (closeTime > Util.EPS) {
            closeTime -= Time.deltaTime;
            anim.SetFloat(CloseTimeHash, closeTime);
        }
    }

    private static BugController GetBugFromCollider(Collider2D other)
    {
        if (other == null) {
            return null;
        }

        return other.GetComponentInParent<BugController>();
    }

    public void HandleSenseEnter(Collider2D other)
    {
        if (other == null) {
            return;
        }
        
        BugController bug = GetBugFromCollider(other);
        if (bug == null)
            return;

        onBugSensed?.Invoke(bug);
    }

    public void HandleSenseExit(Collider2D other)
    {
        BugController bug = GetBugFromCollider(other);
        if (bug == null)
            return;

        onBugEscaped?.Invoke(bug);
    }

    public void HandleBiteEnter(Collider2D other)
    {
        BugController bug = GetBugFromCollider(other);
        if (bug == null)
            return;
        if (!OnBiteBugs.Contains(bug)) OnBiteBugs.Add(bug);
    }

    public void HandleBiteExit(Collider2D other)
    {
        BugController bug = GetBugFromCollider(other);
        if (bug == null)
            return;
        if (OnBiteBugs.Contains(bug)) OnBiteBugs.Remove(bug);
    }

    Collider2D FindSensorZoneCollider()
    {
        TrapZoneRelay zoneRelays = GetComponent<TrapZoneRelay>();

        Collider2D zoneCollider = zoneRelays.GetComponent<Collider2D>();
        if (zoneCollider != null)
        {
            return zoneCollider;
        }

        return null;
    }

    public void Bite(Vector2 mousePosition)
    {
        if (closeTime > Util.EPS) {
            return;
        }
        
        sensorZoneCollider = FindSensorZoneCollider();
        if (sensorZoneCollider == null || !sensorZoneCollider.OverlapPoint(mousePosition)) return;

        if (anim != null)
        {
            closeTime = 5.0f; // Set close time to 5 seconds
            anim.SetFloat(CloseTimeHash, closeTime);
        }

        BugController[] bugsToEat = OnBiteBugs.ToArray();
        OnBiteBugs.Clear();
        foreach (var bug in bugsToEat)
        {
            if (bug != null)
            {
                onBugEaten?.Invoke(bug);
                OnBiteBugs.Remove(bug);
            }
        }
    }
}
