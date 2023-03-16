using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnOnEntity : MonoBehaviour
{
    public GameObject prefab = null;
    private List<GameObject> spawned = new();
    private Sensor sensor;
    private bool hasWork;

    void Start()
    {
        sensor = gameObject.GetComponent<Sensor>();
        sensor.messageReceivedEvent.AddListener(Spawn);
    }

    void Update() {
        if (hasWork) {
            hasWork = false;
            foreach (GameObject obj in spawned) Destroy(obj);
            spawned = new List<GameObject>();
            foreach (Entity e in sensor.Entities.Values.ToList()) {
                var obj = Instantiate(prefab, new Vector3(e.X[0]/10f, 0, e.Y[0]/10), Quaternion.identity);
                obj.transform.SetParent(gameObject.transform);
                spawned.Add(obj);
            }
        }
    }

    void Spawn()
    {
        if (prefab == null) {
            Debug.LogWarning("No prefab set, nothing will spawn");
            return;
        }
        hasWork = true;
    }
}
