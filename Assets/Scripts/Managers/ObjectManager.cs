using UnityEngine;

public class ObjectManager
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] flies { get; private set; }
    public GameObject[] traps { get; private set; }

    void Update()
    {
            flies = GameObject.FindGameObjectsWithTag("Fly");
            traps = GameObject.FindGameObjectsWithTag("Trap");
    }
}
