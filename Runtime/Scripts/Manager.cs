using System.Collections.Generic;
using UnityEngine;
using MQTTnet.Server;
using MQTTnet.Client;
using MQTTnet;
using System.Threading.Tasks;
using System.Text;
using Unity.Plastic.Newtonsoft.Json;

namespace UniTac {
    /// <summary>
    /// Script that manages the sensors.
    /// </summary>
    public class Manager : MonoBehaviour
    {
        /// <summary>
        /// The port the project should use. 
        /// </summary>
        public int ServerPort = 1883;

        /// <summary>
        /// Boolean variable toggled in the Inspector GUI; enables logging to the console if true.
        /// </summary>
        public bool EnableLogging = false;

        /// <summary>
        /// The minimum log level a message from the server needs before it is printed in console.
        /// </summary>
        public LogLevel ServerLogLevel = LogLevel.None;

        /// <summary>
        /// The minimum log level a message from the client needs before it is printed in console.
        /// </summary>
        public LogLevel ClientLogLevel = LogLevel.None;

        /// <summary>
        /// The logger, if enabled. Else; null.
        /// </summary>
        private Logger Logger;

        /// <summary>
        /// The MQTT server.
        /// </summary>
        private MqttServer Server;

        /// <summary>
        /// The MQTT client.
        /// </summary>
        private IMqttClient Client;

        /// <summary>
        /// A dictionary that keeps track of all the sensors.
        /// </summary>
        private readonly Dictionary<string, Sensor> Sensors = new();

        /// <summary>
        /// Initializes the manager when starting Play mode or running the application.
        /// </summary>
        public void Start() {
            if (ClientLogLevel == LogLevel.None && ServerLogLevel == LogLevel.None)
                EnableLogging = false;
            if (EnableLogging) Logger = new();
            foreach (Transform child in transform)
            {
                if (Sensors.ContainsKey(child.GetComponent<Sensor>().Serial)) continue;
                Sensors.Add(child.GetComponent<Sensor>().Serial, child.GetComponent<Sensor>());
            }
            Server = CreateServer();
            Client = CreateClient();
            _ = Server.StartAsync();
            ConnectClient();
        }

        /// <summary>
        /// Creates an MQTT server object enabling communication between the sensor and the client.
        /// </summary>
        /// <returns>The server object</returns>
        MqttServer CreateServer() {
            var mqttFactory = new MqttFactory();
            if (EnableLogging)
                mqttFactory = new MqttFactory(Logger);
            
            var MqttServerOptions = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(ServerPort)
                .Build();
            return mqttFactory.CreateMqttServer(MqttServerOptions);
        }

        /// <summary>
        /// Creates an MQTT client that receives the sensor data from the MQTT server.
        /// </summary>
        /// <returns>The client object</returns>
        IMqttClient CreateClient() {
            var mqttFactory = new MqttFactory();
            if (EnableLogging)
                mqttFactory = new MqttFactory(Logger);
            var client = mqttFactory.CreateMqttClient();
            client.ApplicationMessageReceivedAsync += e => HandleMessage(e);
            return client;
        }

        /// <summary>
        /// Connects the client to the server.
        /// </summary>
        /// <returns>awaitable <see cref="Task"/></returns>
        async void ConnectClient()
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1")
                .Build();
            
            var mqttFactory = new MqttFactory();
            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f => { f.WithTopic("smx/device/+/position"); })
                .Build();

            await Client.ConnectAsync(mqttClientOptions, System.Threading.CancellationToken.None);
            await Client.SubscribeAsync(mqttSubscribeOptions, System.Threading.CancellationToken.None);    
        }

        /// <summary>
        /// A method to process message events from TAC-B sensors.
        /// </summary>
        /// <param name="e">The incoming message event</param>
        /// <returns>awaitable <see cref="Task"/></returns>
        private Task HandleMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            var json = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            var payload = JsonConvert.DeserializeObject<Payload>(json);
            var serial = payload.CollectorSerial;
            Sensors[serial].HandleMessage(payload);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gracefully shut down and dispose client and server when exiting play mode/shutting down application.
        /// </summary>
        async void OnApplicationQuit()
        {
            await Client.DisconnectAsync();
            await Server.StopAsync();
            Client.Dispose();
            Server.Dispose();
        }
    }
}