#nullable enable
using MQTTnet;
using MQTTnet.Client;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;

/// <summary>
/// MQTT-client wrapper, see <see cref="MqttClient"/>
/// </summary>
public class Client
{
    private readonly IMqttClient mqttClient;
    private readonly MqttClientOptions mqttClientOptions;
    private readonly MqttClientSubscribeOptions mqttSubscribeOptions;
    private readonly Logger? logger;
    Spawner spawner;

    /// <summary>
    /// Constructor sets up the client with options to connect to the server and subscribe to any position data from any sensmax TAC-B sensors connected to the server
    /// </summary>
    /// <param name="logger">Optional: A logger may be passed to enable debugging </param>
    public Client(Spawner spawner, Logger? logger = null)
    {
        this.spawner = spawner;
        this.logger = logger;
        var mqttFactory = new MqttFactory();

        // 0.0.0.0 did not work, but 127.0.0.1 does
        this.mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("127.0.0.1")
            .Build();
        if (logger != null) mqttClient = mqttFactory.CreateMqttClient(logger);
        else mqttClient = mqttFactory.CreateMqttClient();

        // Sets up the message handler before connecting so no messages are lost
        mqttClient.ApplicationMessageReceivedAsync += e => MessageHandler(e);

        this.mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        // + or # is a wildcard so that any serial is accepted
                        f.WithTopic("smx/device/+/position");
                    })
                .Build();
    }

    /// <summary>
    /// Connects to the server and subscribes to any position data from any sensmax TAC-B sensors connected to the server
    /// </summary>
    /// <returns>awaitable <see cref="Task"/></returns>
    public async Task Connect()
    {
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
        if (logger != null)
        {
            if (payload != null) this.logger.Log(payload);
            else this.logger.LogError("Could not unpack payload" + bytes);
        }

        if (spawner != null)
        {
            spawner.UpdateBeans(payload?.Entities.Values.ToList());
        }
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
}
