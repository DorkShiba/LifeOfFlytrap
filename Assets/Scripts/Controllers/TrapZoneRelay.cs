using UnityEngine;

public enum TrapZoneType
{
    Sense,
    Bite
}

public class TrapZoneRelay : MonoBehaviour
{
    [SerializeField] private TrapController trap;
    [SerializeField] private TrapZoneType zoneType;

    public TrapZoneType ZoneType => zoneType;

    private void Awake()
    {
        if (trap == null)
            trap = GetComponentInParent<TrapController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (trap == null)
            return;

        switch (zoneType)
        {
            case TrapZoneType.Sense:
                trap.HandleSenseEnter(other);
                break;
            case TrapZoneType.Bite:
                trap.HandleBiteEnter(other);
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (trap == null)
            return;

        if (zoneType == TrapZoneType.Sense)
            trap.HandleSenseExit(other);
        if (zoneType == TrapZoneType.Bite)
            trap.HandleBiteExit(other);
    }
}
