using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json;
using System.Collections.Generic;

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
    Dictionary<string, Sensor> sensors = new();

    void Start()
    {
        clientLogger.logLevel = this.clientLogLevel;
        serverLogger.logLevel = this.serverLogLevel;
        foreach (Transform child in transform)
        {
            if (sensors.ContainsKey(child.GetComponent<Sensor>().Serial)) continue;
            sensors.Add(child.GetComponent<Sensor>().Serial, child.GetComponent<Sensor>());
        }
        server = CreateServer();
        client = CreateClient();
        _ = server.StartAsync();
        ConnectClient();
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

        await client.ConnectAsync(mqttClientOptions, System.Threading.CancellationToken.None);
        await client.SubscribeAsync(mqttSubscribeOptions, System.Threading.CancellationToken.None);    
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
        sensors[serial].HandleMessage(payload);
        return Task.CompletedTask;
    }
}