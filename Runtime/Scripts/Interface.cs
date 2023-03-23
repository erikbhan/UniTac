using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// Script that interfaces between the game world and the sensor API.
/// </summary>
public class Interface : MonoBehaviour
{
    /// <summary>
    /// Log level of the sensor. 
    /// Default log level: Waring.
    /// </summary>
    public LogLevel ServerLogLevel = LogLevel.Warning;
    /// <summary>
    /// Log level of the client.
    /// Default log level: Waring.
    /// </summary>
    public LogLevel ClientLogLevel = LogLevel.Warning;
    /// <summary>
    /// Server port for the server and client use.
    /// </summary>
    public int ServerPort = 1883;

    private readonly Logger ServerLogger = new();
    private readonly Logger ClientLogger = new();
    private MqttServer Server;
    private IMqttClient Client;
    private readonly Dictionary<string, Sensor> Sensors = new();

    /// <summary>
    /// Initializes the manager interface.
    /// </summary>
    void Start()
    {
        ClientLogger.logLevel = this.ClientLogLevel;
        ServerLogger.logLevel = this.ServerLogLevel;
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
    /// Gracefully shut down and dispose client and server when exiting play mode/shutting down application.
    /// </summary>
    async void OnApplicationQuit()
    {
        await Client.DisconnectAsync();
        await Server.StopAsync();
        Client.Dispose();
        Server.Dispose();
    }

    /// <summary>
    /// Creates an MQTT server for the sensor to connect to
    /// </summary>
    /// <returns>The server object</returns>
    MqttServer CreateServer() {
        var mqttFactory = new MqttFactory(ServerLogger);
        var MqttServerOptions = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(ServerPort)
            .Build();
        return mqttFactory.CreateMqttServer(MqttServerOptions);
    }

    /// <summary>
    /// Creates an MQTT client to receive data from the server
    /// </summary>
    /// <returns>The client object</returns>
    IMqttClient CreateClient() {
        var mqttFactory = new MqttFactory(ClientLogger);
        var client = mqttFactory.CreateMqttClient();
        client.ApplicationMessageReceivedAsync += e => HandleMessage(e);
        return client;
    }

    /// <summary>
    /// Connects the client to the server
    /// </summary>
    /// <returns>awaitable <see cref="Task"/></returns>
    async void ConnectClient()
    {
        // Client options
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("127.0.0.1")
            .Build();
        
        // Client subscription options
        var mqttFactory = new MqttFactory();
        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(
                f => { f.WithTopic("smx/device/+/position"); })
            .Build();

        await Client.ConnectAsync(mqttClientOptions, System.Threading.CancellationToken.None);
        await Client.SubscribeAsync(mqttSubscribeOptions, System.Threading.CancellationToken.None);    
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
        Sensors[serial].HandleMessage(payload);
        return Task.CompletedTask;
    }
}