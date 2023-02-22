using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;

/// <summary>
/// Class for deserializing JSON-objects received from sensor
/// </summary>
[Serializable]
public class Payload
{
    // JSON from sensor:
    // {"messageType":"position",
    // "id":"974682A",
    // "collector_id":"TAC-B",
    // "collector_serial":"051001572",
    // "total_detected_objects":1,
    // "object_list":{
    // "object0":{
    // "ID":76,
    // "speed":0.0,
    // "X":[-75,-93,0,0,0,0,0,0,0,0],
    // "Y":[40,51,0,0,0,0,0,0,0,0]}}}

    [JsonProperty("messagetype")]
    public string MessageType { get; set; } = string.Empty;
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    [JsonProperty("collector_id")] 
    public string CollectorId { get; set; } = string.Empty;
    [JsonProperty("collector_serial")] 
    public string CollectorSerial { get; set; } = string.Empty;
    [JsonProperty("total_detected_objects")]
    public int TotalDetectedEntities;
    [JsonProperty("object_list")]
    public Dictionary<string,Entity> Entities { get; set; } = new Dictionary<string,Entity>();

    /// <summary>
    /// Returns a string that represents the current payload.
    /// </summary>
    /// <returns>
    /// <see cref="string" />
    /// A string that represents the current payload.
    /// </returns>
    public override string ToString()
    {
        string s = "Message type: " + MessageType + ", Message id: " + Id + ", Collector id: " + CollectorId + ", Collector serialnumber: " + CollectorSerial + ", Total detected entities: " + TotalDetectedEntities + "\nEntities:\n";
        foreach (var entity in Entities)
        {
            s += entity.Key + ": " + entity.Value.ToString() + "\n";
        }
        return s;
    }
}
