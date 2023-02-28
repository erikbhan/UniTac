#nullable enable
using MQTTnet;
using MQTTnet.Server;
using System.Threading.Tasks;

/// <summary>
/// MQTT-server wrapper, see <see cref="MqttServer"/>
/// </summary>
public class Server
{
    private readonly MqttServer mqttServer;

    /// <summary>
    /// Creates an MQTTserver (broker) ready to listen to localhost:1883
    /// </summary>
    /// <param name="logger">Optional: A logger may be passed to enable debugging</param>
    public Server(Logger? logger = null)
    {
        // Factory
        MqttFactory mqttFactory = logger != null ? new MqttFactory(logger) : new MqttFactory();

        // Options
        var mqttServerOptions = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .Build();

        mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);
    }

    /// <summary>
    /// Starts the server listening to localhost:1883
    /// </summary>
    /// <returns>awaitable <see cref="Task"/></returns>
    public async Task Start()
    {
        await mqttServer.StartAsync();
    }

    /// <summary>
    /// Stops the server
    /// </summary>
    /// <returns>awaitable <see cref="Task"/></returns>
    public async Task Stop() {
        await mqttServer.StopAsync();
    }

    /// <summary>
    /// Checks if the server is started
    /// </summary>
    /// <returns>true if server is started</returns>
    public bool IsStarted()
    {
        return mqttServer.IsStarted;
    }
}
