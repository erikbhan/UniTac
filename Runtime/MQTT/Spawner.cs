using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    Dictionary<long, GameObject> beans = new();
    List<Entity> incomingBeans = new();
    List<long> longs= new();


    public void Update()
    {
        Debug.Log(incomingBeans.Count);
        foreach (Entity e in incomingBeans)
        {
            if (beans.ContainsKey(e.Id))
            {
                beans[e.Id].transform.position = new Vector3(e.X[0]/10.0f, 0, e.Y[0] / 10.0f);
            } 
            else
            {
                Debug.Log("Created new bean");
                beans.Add(e.Id, Instantiate(prefab, new Vector3(e.X[0] / 10.0f, 0, e.Y[0] / 00.0f), Quaternion.identity));
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
