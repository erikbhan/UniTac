#nullable enable
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public int activeSeconds { get; } = 0;
    public int idleSeconds { get; } = 0;
    public int sessionTimeoutSeconds = 60;
    public bool isActive = false;

    void Update()
    {
        
    }

    public GameObject? spawnObject = null;
    public string serial = "";

    public void HandleMessage(Payload payload) {
        
        if (spawnObject == null) return;
    }

}
