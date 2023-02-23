using MQTTnet;
using MQTTnet.Server;
using System.Threading.Tasks;

public class Server
{
    private readonly MqttServer mqttServer;
    public Server(Logger logger = null)
    {
        // Factory
        MqttFactory mqttFactory;
        if (logger != null) mqttFactory = new MqttFactory(logger);
        else mqttFactory = new MqttFactory();

        // Options
        var mqttServerOptions = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .Build();

        mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);
    }

    public async Task Start()
    {
        await mqttServer?.StartAsync();
    }

    public async Task Stop() {
        await mqttServer?.StopAsync();
    }

    public bool IsStarted()
    {
        return mqttServer.IsStarted;
    }
}
