using System;
using UnityEngine;

[Serializable]
public class ObjectPosition
{
// {
//    "ID":76,
//    "speed":0.0,
//    "X":[-75,-93,0,0,0,0,0,0,0,0],
//    "Y":[40,51,0,0,0,0,0,0,0,0]
// }
    public long ID { get; set; }
    public float speed { get; set; }
    public int[] X { get; set; } = new int[10];
    public int[] Y { get; set; } = new int[10];
}
