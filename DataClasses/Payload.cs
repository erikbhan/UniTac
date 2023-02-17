using System.Collections.Generic;

public class Payload
{
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
            // "Y":[40,51,0,0,0,0,0,0,0,0]
            // }
        // }
    // }
    public string messageType { get; set; } = string.Empty;
    public string id { get; set; } = string.Empty;
    public string collector_id { get; set; } = string.Empty;
    public string collector_serial { get; set; } = string.Empty;
    public int total_detected_objects { get; set; }
    public Dictionary<string,ObjectPosition> object_list { get; set; } = new Dictionary<string,ObjectPosition>();
}
