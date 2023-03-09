using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

#nullable enable

namespace Singleton {
// From https://csharpindepth.com/Articles/Singleton
public sealed class Client
{
    private static readonly Client instance = new Client();
    private static readonly MqttFactory mqttFactory = new MqttFactory();
    private static readonly IMqttClient mqttClient = mqttFactory.CreateMqttClient();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static Client() {}

    private Client() {}

    public static Client Instance { get { return instance; } }

    /// <summary>
    /// Connects to the server and subscribes to any position data from any sensmax TAC-B sensors connected to the server
    /// </summary>
    /// <returns>awaitable <see cref="Task"/></returns>
    public async Task Connect()
    {
        // 0.0.0.0 did not work, but 127.0.0.1 does
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("127.0.0.1")
            .Build();

        // Sets up the message handler before connecting so no messages are lost
        mqttClient.ApplicationMessageReceivedAsync += e => MessageHandler(e);

        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(
                    f =>
                    {
                        // + or # is a wildcard so that any serial is accepted
                        f.WithTopic("smx/device/+/position");
                    })
                .Build();
        
        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
    }

    /// <summary>
    /// Disconnects the client from localhost
    /// </summary>
    /// <returns>awaitable <see cref="Task"/></returns>
    public async Task Disconnect()
    {
        await mqttClient.DisconnectAsync();
    }

    /// <summary>
    /// Checks if client is connected to the server
    /// </summary>
    /// <returns>true if server is connected</returns>
    public bool IsConnected()
    {
        return mqttClient.IsConnected;
    }

    /// <summary>
    /// Temporary handler for server messages
    /// </summary>
    /// <param name="incomingEvent">Incoming event from server</param>
    /// <returns><see cref="Task.CompletedTask"/> when message is handled</returns>
    private Task MessageHandler(MqttApplicationMessageReceivedEventArgs incomingEvent)
    {
        var bytes = incomingEvent.ApplicationMessage.Payload;
        var payload = UnpackPayload(bytes);
        if (payload != null) Debug.Log(payload);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Unpacks payloads from bytes received from the server to <see cref="Payload"/>
    /// </summary>
    /// <param name="bytes">Byte array received in <see cref="MqttApplicationMessageReceivedEventArgs"/>.ApplicationMessage.Payload</param>
    /// <returns><see cref="Payload"/> Unpacked payload or <see cref="null"/> if encoding or deseralize failed</returns>
    private Payload? UnpackPayload(byte[] bytes)
    {
        var json = Encoding.UTF8.GetString(bytes);
        var payload = JsonConvert.DeserializeObject<Payload>(json);
        return payload;
    }
}}