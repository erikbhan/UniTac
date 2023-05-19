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
    [Tooltip("Spawnable prefab")]
    public GameObject SpawnPrefab;
    /// <summary>
    /// List of colors for spawned objects. Colors are assigned based on Id in this sample.
    /// </summary>
    [Tooltip("Colors for spawned objects. Colors are assigned based on Id in this sample.")]
    public List<Color> colors = new(){
        Color.blue,
        Color.green,
        Color.red,
        Color.yellow,
    };
   
    // Private
    private bool newMessageReceived = false;
    private readonly Queue<Entity> UpdateQueue = new();
    private readonly Dictionary<long, GameObject> Spawned = new();
    private List<GameObject> Entities;
    private Sensor Sensor;

    /// <summary>
    /// Start is called before the first frame update.
    /// Gets the parent <see cref="Sensor"/> and initiates the event listeners.
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
    /// Update is called once per frame.
    /// Maintains the list of active game objects.
    /// </summary>
    void Update()
    {
        if (newMessageReceived)
        {
            newMessageReceived = false;
            TransformOrInstantiateEntities(UpdateQueue);
        }
    }
    
    /// <summary>
    /// Updates the list of gameobjects.
    /// </summary>
    /// <param name="entities">Queue of active entities.</param>
    private void TransformOrInstantiateEntities(Queue<Entity> entities)
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
                Entities.Add(Spawned[entity.Id]);
            }
        }
    }

    /// <summary>
    /// Queues entities for spawning in next frame.
    /// </summary>
    private void Spawn()
    {
        foreach (Entity entity in Sensor.Entities)
        {
            UpdateQueue.Enqueue(entity);
        }
        newMessageReceived = true;
    }

    /// <summary>
    /// Updates local active status on event.
    /// </summary>
    private void UpdateStatus()
    {
        foreach (var spawn in Spawned.Values)
        {
            Destroy(spawn);
        }
        Spawned.Clear();
        Entities.Clear();
    }
}