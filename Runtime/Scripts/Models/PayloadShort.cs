using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;

namespace UniTac.Models {
    /// <summary>
    /// Class for deserializing JSON-objects received from sensor.
    /// </summary>
    [Serializable]
    public class PayloadShort
    {
        // JSON format from sensor:
        // {"messageType":"position",
        // "id":"974682A",
        // "collector_id":"TAC-B",
        // "collector_serial":"051001572",
        // "total_detected_objects":1,
        // "object_list":{
        // "object0":["12","100","215","0", "5.6"]}

        /// <summary>
        /// Type of message from publisher.
        /// </summary>
        [JsonProperty("messagetype")]
        public string MessageType { get; set; } = string.Empty;
        /// <summary>
        /// Message Id set by publisher.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Sensor Id.
        /// </summary>
        [JsonProperty("collector_id")]
        public string CollectorId { get; set; } = string.Empty;
        /// <summary>
        /// Sensor serial number.
        /// </summary>
        [JsonProperty("collector_serial")]
        public string CollectorSerial { get; set; } = string.Empty;
        /// <summary>
        /// Total number of detected entities.
        /// </summary>
        [JsonProperty("total_detected_objects")]
        public int TotalDetectedEntities;
        /// <summary>
        /// List of entities detected by the sensor.
        /// </summary>
        [JsonProperty("object_list")]
        public Dictionary<string, float[]> Entities { get; set; } = new();
    }
}
