using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Sample monobehaviour that spawns GameObjects that represents any entities detected by the sensor.
/// </summary>
public class SpawnOnEntity : MonoBehaviour
{
    /// <summary>
    /// GameObject to spawn representing entities.
    /// </summary>
    public GameObject SpawnPrefab = null;
    private readonly Queue<Entity> UpdateQueue = new();
    private readonly Dictionary<long, GameObject> Spawned = new();
    private Sensor Sensor = null;

    /// <summary>
    /// Initialices the spawner.
    /// </summary>
    void Start()
    {
        if (!SpawnPrefab) Debug.LogWarning("Prefab not set");
        Sensor = gameObject.GetComponent<Sensor>();
        if (!Sensor) Debug.LogError("Sensor script not found");
        Sensor.MessageReceivedEvent.AddListener(Spawn);
    }

    /// <summary>
    /// Maintains the list of active game objects.
    /// </summary>
    void Update() {
        if (UpdateQueue.Any())
        {
            Entity entity = UpdateQueue.Dequeue();
            string s = "";
            foreach (var i in entity.X) s += i + ", ";
            Debug.Log(s);
            if (Spawned.ContainsKey(entity.Id))
            {
                Spawned[entity.Id].transform.localPosition = new Vector3(entity.X[0]/10f, 0f, entity.Y[0]/10f);
            }
            else
            {
                var newBean = Instantiate(SpawnPrefab, gameObject.transform, false);
                newBean.transform.localPosition = new Vector3(entity.X[0] / 10f, 0f, entity.Y[0] / 10f);
                Spawned.Add(entity.Id, newBean);
            }
        }
    }

    /// <summary>
    /// Queues entities for spawning in next frame.
    /// </summary>
    void Spawn()
    {
        foreach (Entity entity in Sensor.Entities.Values.ToList<Entity>())
        {
            UpdateQueue.Enqueue(entity);
        }
    }
}