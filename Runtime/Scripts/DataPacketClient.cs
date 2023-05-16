using MQTTnet.Client;
using MQTTnet;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UniTac
{
    /// <summary>
    /// MQTT-client that saves 5-minute-messages from the sensor to a log file.
    /// </summary>
    public class DataPacketClient : MonoBehaviour
    {
        /// <summary>
        /// Relative path to output file, a file wil be automatically generated with the name and filetype in the set path.
        /// </summary>
        [Tooltip("Relative path to output file, a file wil be automatically generated with the name and filetype in the set path.")]
        public string Path = "./Assets/5-minute-message-log.txt";
        /// <summary>
        /// The minimum log level a message from the client needs before it 
        /// is printed in Unity's debug console.
        /// </summary>
        [Tooltip("The minimum log level a message from the client needs before it is printed in Unity's debug console. Does not effect function of the script.")]
        public LogLevel LogLevel = LogLevel.None;
        private IMqttClient Client;
        private Manager Manager;

        /// <summary>
        /// Start is called before the first frame update.
        /// Gets the parent <see cref="Manager"/> and creates the file if necessary.
        /// </summary>
        void Start()
        {
            Manager = gameObject.GetComponent<Manager>();
            if (!Manager)
            {
                Debug.LogError("Manager not found");
                Application.Quit();
            }
            if (!File.Exists(Path))
            {
                File.Create(Path);
            }
            if (!File.Exists(Path))
            {
                Debug.LogError("Could not create file!");
            }
            Client = CreateClient();
            StartCoroutine(ConnectClient());
        }

        /// <summary>
        /// Creates an MQTT client that receives the sensor data from the MQTT server.
        /// </summary>
        /// <returns>The client object</returns>
        private IMqttClient CreateClient()
        {
            var mqttFactory = new MqttFactory();
#if UNITY_EDITOR
            if (LogLevel != LogLevel.None)
            {
                Logger logger = new(LogLevel);
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
        private IEnumerator ConnectClient()
        {
            yield return new WaitUntil(() => Manager.Server != null && Manager.Server.IsStarted);
            var username = "";
            var password = "";
            if (Manager.SecretsFilePath != "" && File.Exists(Manager.SecretsFilePath))
            {
                var json = File.ReadAllText(Manager.SecretsFilePath);
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                username = data["username"];
                password = data["password"];
            }
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1")
                .WithCredentials(username, password)
                .Build();
            Client.ConnectAsync(mqttClientOptions, System.Threading.CancellationToken.None);

            yield return new WaitUntil(() => Client.IsConnected);
            var mqttFactory = new MqttFactory();
            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f => { f.WithTopic("smx/device/+/5_min_data/#"); })
                .Build();
            Client.SubscribeAsync(mqttSubscribeOptions, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// A method to process message events from TAC-B sensors.
        /// </summary>
        /// <param name="e">The incoming message event</param>
        /// <returns>awaitable <see cref="Task"/></returns>
        private Task HandleMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            var json = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            using FileStream fs = new(Path, FileMode.Append, FileAccess.Write);
            using StreamWriter sw = new(fs);
            sw.WriteLine(json + ",");
            return Task.CompletedTask;
        }
    }
}
