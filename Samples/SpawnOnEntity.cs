using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniTac;
using UniTac.Models;

/// <summary>
/// Sample monobehaviour that spawns GameObjects that represents any entities 
/// detected by the sensor.
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
    /// List of colors for spawned objects.
    /// </summary>
    public List<Color> colors = new(){
        Color.blue,
        Color.green,
        Color.red,
        Color.yellow,
    };
    private bool newMessageRecieved = false;

    /// <summary>
    /// Initialices the spawner.
    /// </summary>
    void Start()
    {
        if (!SpawnPrefab) Debug.LogWarning("Prefab not set");
        Sensor = gameObject.GetComponent<Sensor>();
        if (!Sensor) Debug.LogError("Sensor script not found");
        Sensor.MessageReceivedEvent.AddListener(Spawn);
        Sensor.StatusChangedEvent.AddListener(UpdateStatus);
    }

    /// <summary>
    /// Maintains the list of active game objects.
    /// </summary>
    void Update()
    {
        if (newMessageRecieved)
        {
            newMessageRecieved = false;
            TransformOrInstantiateEntities(UpdateQueue);
        }
    }
    
    /// <summary>
    /// Updates the list of gameobjects.
    /// </summary>
    /// <param name="entities">Queue of active entities.</param>
    void TransformOrInstantiateEntities(Queue<Entity> entities)
    {
        foreach (var spawn in Spawned)
        {
            spawn.Value.SetActive(false);
        }
        while (entities.Count > 0)
        {
            var entity = UpdateQueue.Dequeue();
            if (Spawned.ContainsKey(entity.Id))
            {
                // Entity is spawned on localPosition with the incoming X and Y coordinates from the sensor.
                // The sensor sends X and Y as centimeters, so accurate conversion to Unity's units would be x/100,
                // this sample uses x/10 for easy visualization.
                Spawned[entity.Id].transform.localPosition = new Vector3(entity.X[0] / 10f, 0f, entity.Y[0] / 10f);
                Spawned[entity.Id].SetActive(true);
            }
            else
            {
                var newBean = Instantiate(SpawnPrefab, gameObject.transform, false);
                newBean.transform.localPosition = new Vector3(entity.X[0] / 10f, 0f, entity.Y[0] / 10f);
                if (colors.Any()) newBean.GetComponent<MeshRenderer>().material.color = colors[(int)entity.Id % colors.Count];
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
        newMessageRecieved = true;
    }

    /// <summary>
    /// Updates local active status on event.
    /// </summary>
    void UpdateStatus()
    {
        foreach (var spawn in Spawned.Values)
        {
            Destroy(spawn);
        }
        Spawned.Clear();
    }
}