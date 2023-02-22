using System;
using Unity.Plastic.Newtonsoft.Json;

/// <summary>
/// Class for objects detected by the sensor, sensor payload carries <see cref="Dictionary" />&lt;<see cref="string"/>,<see cref="Entity"/>&gt;
/// </summary>
[Serializable]
public class Entity
{
    // {
    //    "ID":76,
    //    "speed":0.0,
    //    "X":[-75,-93,0,0,0,0,0,0,0,0],
    //    "Y":[40,51,0,0,0,0,0,0,0,0]
    // }
    
    [JsonProperty("ID")]
    public long Id { get; set; }
    [JsonProperty("speed")]
    public float Speed { get; set; }
    public int[] X { get; set; } = new int[10];
    public int[] Y { get; set; } = new int[10];

    /// <summary>
    /// Returns a string that represents the current payload.
    /// </summary>
    /// <returns>
    /// <see cref="string" />
    /// A string that represents the current payload.
    /// </returns>
    public override string ToString()
    {
        string s = "Id: " + Id + "\nSpeed: " + Speed + "\n";
        s += "X: [ ";
        foreach (var x in X) { s += x + ", "; }
        s += "]\n";
        s += "Y: [ ";
        foreach (var y in Y) { s += y + ", "; }
        s += "]\n";
        return s;
    }
}
