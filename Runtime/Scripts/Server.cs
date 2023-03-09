using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Server;
using UnityEngine;

namespace Singleton {
    // From https://csharpindepth.com/Articles/Singleton
    public sealed class Server
    {
        private static readonly Server instance = new Server();
        private static MqttServer mqttServer;
        private MqttFactory mqttFactory = new MqttFactory();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Server() {}

        private Server() {
            var mqttServerOptions = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .Build();

            mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);
        }

        public static Server Instance { get { return instance; } }

        public async Task StartAsync() {
            await mqttServer.StartAsync();
        }
    }
}