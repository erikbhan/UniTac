using System.Collections.Generic;
using UnityEngine;
using MQTTnet.Server;
using MQTTnet.Client;
using MQTTnet;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using MQTTnet.Protocol;
using System.IO;
using UniTac.Models;
using System;

namespace UniTac {
    /// <summary>
    /// Script that manages the sensors.
    /// </summary>
    public class Manager : MonoBehaviour
    {
        /// <summary>
        /// The port the project should use. 
        /// </summary>
        [Tooltip("Server port for the MQTT-broker.")]
        public int ServerPort = 1883;

        [Header("Debug settings")]
        /// <summary>
        /// Boolean variable toggled in the Inspector GUI; enables logging to 
        /// the console if true.
        /// </summary>
        [Tooltip("Turns logging to debug console on or off completely. Turning this off increases performance.")]
        public bool EnableLogging = false;
        /// <summary>
        /// The minimum log level a message from the server needs before it 
        /// is printed in console.
        /// </summary>
        [Tooltip("The minimum log level a message from the server needs before it is printed in console.")]
        public LogLevel ServerLogLevel = LogLevel.None;
        /// <summary>
        /// The minimum log level a message from the client needs before it 
        /// is printed in console.
        /// </summary>
        [Tooltip("The minimum log level a message from the client needs before it is printed in console.")]
        public LogLevel ClientLogLevel = LogLevel.None;

        [Header("Credentials")]
        /// <summary>
        /// The relative path to file with username and password for MQTT-protocol. 
        /// If left empty no username or password will be sett.
        /// </summary>
        [Tooltip("The relative path to a txt-file containing {\"username\":\"YOUR_USERNAME\",\"password\":\"SECRET_PASSWORD\"}")]
        public string SecretsFilePath = string.Empty;
        /// <summary>
        /// The MQTT server.
        /// </summary>
        internal MqttServer Server { get; private set; }
        private IMqttClient Client;
        private readonly Dictionary<string, Sensor> Sensors = new();
        public List<GameObject> SensorList = new();

        /// <summary>
        /// Start is called before the first frame update.
        /// Initializes the manager, starting the server and connecting the client.
        /// </summary>
        void Start() {
            var username = "";
            var password = "";
            if (SecretsFilePath != "" && File.Exists(SecretsFilePath))
            {
                var json = File.ReadAllText(SecretsFilePath);
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                username = data["username"];
                password = data["password"];
            }
            foreach (Transform child in transform)
            {
                var sensor = child.GetComponent<Sensor>();
                Sensors[sensor.Serial] = sensor;
                SensorList.Add(child.gameObject);
            }
            Server = CreateServer(username, password);
            Client = CreateClient();
            _ = Server.StartAsync();
            ConnectClient(username, password);
        }

        /// <summary>
        /// Creates an MQTT server object enabling communication between the 
        /// sensor and the client.
        /// </summary>
        /// <returns>The server object.</returns>
        private MqttServer CreateServer(string username, string password) {
            var mqttFactory = new MqttFactory();
#if UNITY_EDITOR
            if (EnableLogging && ServerLogLevel != LogLevel.None)
            {
                Logger logger = new(ServerLogLevel);
                mqttFactory = new MqttFactory(logger);
            }
#endif
            var MqttServerOptions = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(ServerPort)
                .Build();
            var mqttServer = mqttFactory.CreateMqttServer(MqttServerOptions);
            if (username != "" || password != "")
            {
                mqttServer.ValidatingConnectionAsync += e =>
                {
                    if (e.UserName != username) 
                        e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    if (e.Password != password) 
                        e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    return Task.CompletedTask;
                };
            }
            return mqttServer;
        }

        /// <summary>
        /// Creates an MQTT client that receives the sensor data from the MQTT server.
        /// </summary>
        /// <returns>The client object.</returns>
        private IMqttClient CreateClient() {
            var mqttFactory = new MqttFactory();
#if UNITY_EDITOR
            if (EnableLogging && ClientLogLevel != LogLevel.None) 
            {
                Logger logger = new(ClientLogLevel);
                mqttFactory = new MqttFactory(logger);
            }
#endif
            var client = mqttFactory.CreateMqttClient();
            client.ApplicationMessageReceivedAsync += e => HandleMessage(e);
            return client;
        }

        /// <summary>
        /// Connects the client to the server.
        /// </summary>
        /// <returns>awaitable <see cref="Task"/>.</returns>
        private async void ConnectClient(string username, string password)
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", ServerPort)
                .WithCredentials(username, password)
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
        /// <param name="e">The incoming message event.</param>
        /// <returns>awaitable <see cref="Task"/>.</returns>
        private Task HandleMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            var json = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            Payload payload;
            try
            {
                payload = JsonConvert.DeserializeObject<Payload>(json) ?? throw new ArgumentNullException();
            }
            catch (Exception)
            {
                payload = new Payload(json);
            }

            // Pass message to correct sensor
            var serial = payload.CollectorSerial;
            if (Sensors.ContainsKey(serial))
            {
                Sensors[serial].HandleMessage(payload);
            }
            else
            {
                Debug.LogWarning("Received message from untracked sensor with serial: " + serial);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gracefully shut down and dispose client and server when exiting play 
        /// mode or quitting the application.
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