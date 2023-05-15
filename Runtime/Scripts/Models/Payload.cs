using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UniTac.Models {
    /// <summary>
    /// Class for deserializing JSON-objects received from sensor.
    /// </summary>
    [Serializable]
    public class Payload
    {
        // JSON format from sensor:
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
        public Dictionary<string, Entity> Entities { get; set; } = new();

        /// <summary>
        /// Returns a string that represents the current payload.
        /// </summary>
        /// <returns>
        /// <see cref="string" /> that represents the current payload.
        /// </returns>
        public override string ToString()
        {
            string s = "Message type: " + MessageType + ", Message id: " + Id + ", Collector id: " 
                        + CollectorId + ", Collector serialnumber: " + CollectorSerial 
                        + ", Total detected entities: " + TotalDetectedEntities + "\nEntities:\n";
            foreach (var entity in Entities)
            {
                s += entity.Key + ": " + entity.Value.ToString() + "\n";
            }
            return s;
        }

        public Payload() { }

        /// <summary>
        /// Constructor using short MQTT-packet.
        /// </summary>
        /// <param name="json">JSON-string of short payload</param>
        public Payload(string json)
        {
            var miniPayload = JsonConvert.DeserializeObject<PayloadShort>(json);
            MessageType = miniPayload.MessageType;
            Id = miniPayload.Id;
            CollectorId = miniPayload.CollectorId;
            CollectorSerial = miniPayload.CollectorSerial;
            TotalDetectedEntities = miniPayload.TotalDetectedEntities;
            foreach (var entity in miniPayload.Entities)
            {
                Entities.Add(entity.Key, new Entity(entity.Value));
            }
        }

        /// <summary>
        /// Class for deserializing short JSON-objects received from sensor.
        /// </summary>
        [Serializable]
        private class PayloadShort
        {
            // JSON format from sensor:
            // {"messageType":"position",
            // "id":"974682A",
            // "collector_id":"TAC-B",
            // "collector_serial":"051001572",
            // "total_detected_objects":1,
            // "object_list":{
            // "object0":["12","100","215","0", "5.6"]}
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
            public Dictionary<string, float[]> Entities { get; set; } = new();
        }
    }
}
