using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Collections;

public class SpawnOnEntity : MonoBehaviour
{
    public GameObject spawnPrefab = null;
    private Queue<Entity> updateQueue = new();
    private Dictionary<long, GameObject> spawned = new();
    private Sensor sensor = null;

    void Start()
    {
        if (!spawnPrefab) Debug.LogWarning("Prefab not set");
        sensor = gameObject.GetComponent<Sensor>();
        if (!sensor) Debug.LogError("Sensor script not found");
        sensor.messageReceivedEvent.AddListener(Spawn);
    }

    void Update() {
        if (updateQueue.Any())
        {
            Entity entity = updateQueue.Dequeue();
            string s = "";
            foreach (var i in entity.X) s += i + ", ";
            Debug.Log(s);
            if (spawned.ContainsKey(entity.Id))
            {
                spawned[entity.Id].transform.localPosition = new Vector3(entity.X[0]/10f, 0f, entity.Y[0]/10f);
            }
            else
            {
                var newBean = Instantiate(spawnPrefab, gameObject.transform, false);
                newBean.transform.localPosition = new Vector3(entity.X[0] / 10f, 0f, entity.Y[0] / 10f);
                spawned.Add(entity.Id, newBean);
            }
        }
    }

    void Spawn()
    {
        foreach (Entity entity in sensor.Entities.Values.ToList<Entity>())
        {
            updateQueue.Enqueue(entity);
        }
    }
}