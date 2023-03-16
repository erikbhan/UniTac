using System.Collections.Generic;
using System.Linq;
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
        UpdateBeans(payload.Entities.Values.ToList());
    }

    Dictionary<long, GameObject> beans = new();
    List<Entity> incomingBeans = new();
    List<long> longs= new();

    private Vector3 GetVector3FromCoordinates(int X, int Y) {
        Vector3 vector = new Vector3(X, 0f, Y);
        if (X != 0) vector.x = X/10.0f;
        if (X != 0) vector.y = Y/10.0f;
        return vector;
    }

    public void Update()
    {
        foreach (Entity e in incomingBeans)
        {
            if (beans.ContainsKey(e.Id))
            {
                beans[e.Id].transform.position = new Vector3(e.X[0]/10.0f, 0, e.Y[0] / 10.0f);
            } 
            else
            {
                Debug.Log("Created new bean");
                beans.Add(e.Id, Instantiate(spawnObject, GetVector3FromCoordinates(e.X[0], e.Y[0]), Quaternion.identity));
            }
        }
        foreach (long id in beans.Keys)
        {
            if (!longs.Contains(id)) {
                Destroy(beans[id]);
                beans.Remove(id);
            }
        }
    }

    public void UpdateBeans(List<Entity> list)
    {
        longs = new();

        this.incomingBeans = list;
        foreach (Entity e in list)
        {
            longs.Add(e.Id);
        }
    }
}
