using System.Collections.Generic;
using UnityEngine;
using System;

public class TrapController : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] public Action<BugController> onBugSensed;
    [SerializeField] public Action<BugController> onBugEscaped;
    [SerializeField] public Action<BugController> onBugEaten;
    [SerializeField] public Action<List<BugController>> onTrapClosed;

    [SerializeField] private List<BugController> OnBiteBugs = new List<BugController>();
    
    [SerializeField] private Animator anim;
    [SerializeField] private Collider2D sensorZoneCollider;
    [SerializeField] private float biteComboTimeout = 0.2f;

    private float closeTime = 0.0f;
    private float biteComboTimer = 0.0f;
    private bool waitingToClose = false;

    private static readonly int BiteTriggerHash = Animator.StringToHash("bite");
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
            if (anim != null)
            {
                anim.SetFloat(CloseTimeHash, closeTime);
            }
            return;
        }

        if (!waitingToClose)
        {
            return;
        }

        biteComboTimer += Time.deltaTime;
        if (biteComboTimer < biteComboTimeout)
        {
            return;
        }

        waitingToClose = false;
        biteComboTimer = 0.0f;

        if (anim != null)
        {
            closeTime = 5.0f;
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
            anim.ResetTrigger(BiteTriggerHash);
            anim.SetTrigger(BiteTriggerHash);
        }

        waitingToClose = true;
        biteComboTimer = 0.0f;

        BugController[] bugsToBite = OnBiteBugs.ToArray();
        foreach (var bug in bugsToBite)
        {
            if (bug != null)
            {
                bool isDead = bug.TakeDamage(PlantController.BiteDamage);
                if (isDead)
                {
                    EatenBugCount++;
                    OnBiteBugs.Remove(bug);
                    onBugEaten?.Invoke(bug);
                }
            }
        }
    }
}
