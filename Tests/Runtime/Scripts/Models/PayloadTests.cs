#nullable enable
using NUnit.Framework;
using Unity.Plastic.Newtonsoft.Json;
using UniTac.Models;
using System;

namespace UniTac.Tests.Models
{
    public class PayloadTests
    {
        [Test]
        public void LongPayloadDeserializesCorrectly()
        {
            var json = @"{""messagetype"":""position"",""id"":""974682a"",""collector_id"":""tac-b"",""collector_serial"":""051001572"",""total_detected_objects"":1,""object_list"":{""object0"":{""id"":76,""speed"":0.0,""x"":[-75,-93,0,0,0,0,0,0,0,0],""y"":[40,51,0,0,0,0,0,0,0,0]}}}";

            Payload? payload = JsonConvert.DeserializeObject<Payload>(json);
            Assert.IsNotNull(payload);
            Assert.AreEqual("position", payload?.MessageType);
            Assert.AreEqual("974682a", payload?.Id);
            Assert.AreEqual("tac-b", payload?.CollectorId);
            Assert.AreEqual("051001572", payload?.CollectorSerial);
            Assert.AreEqual(1, payload?.TotalDetectedEntities);
            Assert.IsNotEmpty(payload?.Entities, "Entities not deserialized correctly");
        }

        [Test]
        public void ShortPayloadDeserializesCorrectly()
        {
            var json = @"{""messagetype"":""position"",""id"":""974682a"",""collector_id"":""tac-b"",""collector_serial"":""051001572"",""total_detected_objects"":1,""object_list"":{""object0"":[""12"",""100"",""215"",""0"", ""5.6""]}}";

            PayloadShort? payload = JsonConvert.DeserializeObject<PayloadShort>(json);
            Assert.IsNotNull(payload);
            Assert.AreEqual("position", payload?.MessageType);
            Assert.AreEqual("974682a", payload?.Id);
            Assert.AreEqual("tac-b", payload?.CollectorId);
            Assert.AreEqual("051001572", payload?.CollectorSerial);
            Assert.AreEqual(1, payload?.TotalDetectedEntities);
            Assert.IsNotEmpty(payload?.Entities, "Entities not deserialized correctly");
            Assert.AreEqual(12, payload?.Entities["object0"][0]);
        }

        [Test]
        public void newPayload_withShortJson_InitializesCorrectly()
        {
            var json = @"{""messagetype"":""position"",""id"":""974682a"",""collector_id"":""tac-b"",""collector_serial"":""051001572"",""total_detected_objects"":1,""object_list"":{""object0"":[""12"",""100"",""215"",""0"", ""5.6""]}}";

            Payload payload;
            try
            {
                payload = JsonConvert.DeserializeObject<Payload>(json) ?? throw new ArgumentNullException();

            } catch (Exception)
            {
                payload = new Payload(json);
            }

            Assert.IsNotNull(payload);
            Assert.AreEqual("position", payload?.MessageType);
            Assert.AreEqual("974682a", payload?.Id);
            Assert.AreEqual("tac-b", payload?.CollectorId);
            Assert.AreEqual("051001572", payload?.CollectorSerial);
            Assert.AreEqual(1, payload?.TotalDetectedEntities);
            Assert.IsNotEmpty(payload?.Entities, "Entities not deserialized correctly");
        }
    }
}
