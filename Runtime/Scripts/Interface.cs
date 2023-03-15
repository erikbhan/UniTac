using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json;

/// <summary>
/// Script that interfaces between the game world and the sensor API
/// </summary>
public class Interface : MonoBehaviour
{
    public LogLevel serverLogLevel = LogLevel.Warning;
    public LogLevel clientLogLevel = LogLevel.Warning;
    public int serverPort = 1883;

    Logger serverLogger = new Logger();
    Logger clientLogger = new Logger();
    MqttServer server;
    IMqttClient client;

    void Start() { Init(); }

    void Update()
    {
        // TODO: use events vs checking every frame
        if (clientLogger.logLevel != this.clientLogLevel)
            clientLogger.logLevel = this.clientLogLevel;
        if (serverLogger.logLevel != this.serverLogLevel)
            serverLogger.logLevel = this.serverLogLevel;
    }

    /// <summary>
    /// Gracefully shut down and dispose client and server when exiting play mode/shutting down application.
    /// </summary>
    async void OnApplicationQuit()
    {
        await client.DisconnectAsync();
        await server.StopAsync();
        client.Dispose();
        server.Dispose();
    }

    /// <summary>
    /// Initialization method for the MQTT server and client.
    /// Create client and server, starts server, connects client to server and subscribes
    /// to the correct topic.
    /// </summary>
    async void Init() {
        server = CreateServer();
        client = CreateClient();
        await server.StartAsync();
        await ConnectClient();
        await SubscribeClient();
    }

    /// <summary>
    /// Creates an MQTT server for the sensor to connect to
    /// </summary>
    /// <returns>The server object</returns>
    MqttServer CreateServer() {
        var mqttFactory = new MqttFactory(serverLogger);
        var MqttServerOptions = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(serverPort)
            .Build();
        return mqttFactory.CreateMqttServer(MqttServerOptions);
    }

    /// <summary>
    /// Creates an MQTT client to receive data from the server
    /// </summary>
    /// <returns>The client object</returns>
    IMqttClient CreateClient() {
        var mqttFactory = new MqttFactory(clientLogger);
        var client = mqttFactory.CreateMqttClient();
        client.ApplicationMessageReceivedAsync += e => HandleMessage(e);
        return client;
    }

    /// <summary>
    /// Connects the client to the server
    /// </summary>
    /// <returns>awaitable <see cref="Task"/></returns>
    async Task ConnectClient()
    {
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("127.0.0.1")
            .Build();
        await client.ConnectAsync(mqttClientOptions);
    }

    /// <summary>
    /// Subscribes the client to the SensMax sensor topic
    /// </summary>
    /// <returns>awaitable <see cref="Task"/></returns>
    async Task SubscribeClient()
    {
        var mqttSubscribeOptions = (new MqttFactory()).CreateSubscribeOptionsBuilder()
            .WithTopicFilter(
                f => { f.WithTopic("smx/device/+/position"); })
            .Build();
        await client.SubscribeAsync(mqttSubscribeOptions);        
    }

    /// <summary>
    /// A method to process message events from TAC-B sensors
    /// </summary>
    /// <param name="e">The incoming message event</param>
    /// <returns>awaitable <see cref="Task"/></returns>
    private Task HandleMessage(MqttApplicationMessageReceivedEventArgs e)
    {
        var json = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        var payload = JsonConvert.DeserializeObject<Payload>(json);
        var serial = payload.CollectorSerial;
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Sensor>().serial == serial) {
                child.GetComponent<Sensor>().HandleMessage(payload);
                break; // break out of loop, because serial numbers are unique
            }
        }
        return Task.CompletedTask;
    }
}