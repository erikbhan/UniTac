using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using MQTTnet.Protocol;

namespace UniTac {
    /// <summary>
    /// Script that manages the sensors.
    /// </summary>
    public class Manager : MonoBehaviour
    {
        /// <summary>
        /// The port the project should use. 
        /// 1883 is the default for MQTT, 8883 is the default for MQTTS.
        /// </summary>
        public int ServerPort = 8883;

        /// <summary>
        /// Boolean variable toggled in the Unity Inspector GUI; enables logging to the console if true.
        /// </summary>
        public bool EnableLogging = false;
        /// <summary>
        /// Boolean variable toggled in the Unity Inspector GUI; enables TLS (encrypted) communication.
        /// Remember to set up certificates correctly.
        /// </summary>
        public bool EnableTLS = false;

        /// <summary>
        /// The minimum log level a message from the server needs before it is printed in console.
        /// </summary>
        public LogLevel ServerLogLevel = LogLevel.None;
        /// <summary>
        /// The minimum log level a message from the client needs before it is printed in console.
        /// </summary>
        public LogLevel ClientLogLevel = LogLevel.None;
        /// <summary>
        /// The path to file with username and password for MQTT-protocol. If left empty no username or password will be sett.
        /// </summary>
        public string SecretsFilePath = string.Empty;
        /// <summary>
        /// The MQTT server.
        /// </summary>
        internal MqttServer Server { get; private set; }
        private IMqttClient Client;
        private readonly Dictionary<string, Sensor> Sensors = new();

        public string CertificatePath = "";

        /// <summary>
        /// Initializes the manager when starting Play mode or running the application.
        /// </summary>
        public void Start() {
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
                if (Sensors.ContainsKey(child.GetComponent<Sensor>().Serial)) 
                {
                    Debug.LogWarning("Two or more sensors with the same serial number");
                    continue;
                }
                Sensors.Add(child.GetComponent<Sensor>().Serial, child.GetComponent<Sensor>());
            }
            Server = CreateServer(username, password);
            Client = CreateClient();
            _ = Server.StartAsync();
            ConnectClient(username, password);
        }

        /// <summary>
        /// Creates an MQTT server object enabling communication between the sensor and the client.
        /// </summary>
        /// <returns>The server object</returns>
        MqttServer CreateServer(string username, string password) {
            var mqttFactory = new MqttFactory();

            if (EnableLogging && ServerLogLevel != LogLevel.None)
            {
                Logger logger = new(ServerLogLevel);
                mqttFactory = new MqttFactory(logger);
            }

            MqttServerOptions MqttServerOptions = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(ServerPort)
                .Build();

            //TODO: Combine TLS and user/pass
            // See here to create a CA + client cert and server cert
            if (EnableTLS && File.Exists(CertificatePath + "server/server.pfx"))
            {
                X509Certificate2 serverCrt = new(CertificatePath + "server/server.pfx", "server", X509KeyStorageFlags.Exportable);

                X509Chain ch = new();
                ch.Build (serverCrt);
                foreach (var status in ch.ChainStatus) {
                    Debug.LogWarning(status.Status);
                }

                MqttServerOptions = new MqttServerOptionsBuilder()
                    .WithoutDefaultEndpoint()
                    .WithEncryptedEndpoint()
                    .WithEncryptedEndpointPort(ServerPort)
                    .WithEncryptionCertificate(serverCrt)
                    .Build();
                return mqttFactory.CreateMqttServer(MqttServerOptions);
            }
            var mqttServer = mqttFactory.CreateMqttServer(MqttServerOptions);
            if (username != "" || password != "")
            {
                mqttServer.ValidatingConnectionAsync += e =>
                {
                    if (e.UserName != username) e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    if (e.Password != password) e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    return Task.CompletedTask;
                };
            }
            return mqttServer;
        }

        /// <summary>
        /// Creates an MQTT client that receives the sensor data from the MQTT server.
        /// </summary>
        /// <returns>The client object</returns>
        IMqttClient CreateClient() {
            var mqttFactory = new MqttFactory();

            if (EnableLogging && ClientLogLevel != LogLevel.None) 
            {
                Logger logger = new(ClientLogLevel);
                mqttFactory = new MqttFactory(logger);
            }

            var client = mqttFactory.CreateMqttClient();
            client.ApplicationMessageReceivedAsync += e => HandleMessage(e);
            return client;
        }

        /// <summary>
        /// Connects the client to the server.
        /// </summary>
        /// <returns>awaitable <see cref="Task"/></returns>
        async void ConnectClient(string username, string password)
        {
            List<X509Certificate> certs = new()
            {
                new(CertificatePath + "client/client.pfx", "client", X509KeyStorageFlags.Exportable)

        };
            //TODO: Combine TLS and user/pass
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1")
                .WithTls(
                    o =>
                    {
                        o.Certificates = certs;
                        o.CertificateValidationHandler = eventArgs =>
                        {
                            Debug.LogWarning("Chain Policy Revocation Mode: " + eventArgs.Chain.ChainPolicy.RevocationMode);
                            Debug.LogWarning("Chain Status................: " + eventArgs.Chain.ChainStatus.ToString());
                            Debug.LogWarning("SSL Policy Errors...........:" + eventArgs.SslPolicyErrors);
                            return true;
                        };
                    }
                )
                // .WithCredentials(username, password)
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