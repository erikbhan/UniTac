using UnityEngine;

/// <summary>
/// Script for the sensor gameobject that handles functionality
/// </summary>
public class Sensor : MonoBehaviour
{
    public string serial = "";
    public GameObject spawnObject = null;

    /// <summary>
    /// Processes the received message data
    /// </summary>
    /// <param name="payload">Payload from the client; holds data from the sensor</param>
    public void HandleMessage(Payload payload) {
        if (spawnObject == null) return;
    }

}
