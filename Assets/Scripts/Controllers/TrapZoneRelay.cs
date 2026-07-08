using UnityEngine;

public enum TrapZoneType
{
    Bite
}

public class TrapZoneRelay : MonoBehaviour
{
    [SerializeField] private TrapController trap;

    private void Awake()
    {
        if (trap == null)
            trap = GetComponentInParent<TrapController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (trap == null) return;
        trap.HandleBiteEnter(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (trap == null) return;
        trap.HandleBiteExit(other);
    }
}
