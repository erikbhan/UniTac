using MQTTnet;
using MQTTnet.Client;
using System.Threading;
using System.Threading.Tasks;

public class Client
{
    private readonly IMqttClient mqttClient;
    private readonly MqttClientOptions mqttClientOptions;
    private readonly MqttClientSubscribeOptions mqttSubscribeOptions;
    public bool isConnected = false;

    public Client(Logger logger)
    {
        var mqttFactory = new MqttFactory();

        this.mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("192.168.0.185")
            .Build();

        mqttClient = mqttFactory.CreateMqttClient();
        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            logger.Log(e);
            return Task.CompletedTask;
        };

        this.mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("smx/device/");
                    })
                .Build();
    }

    public async Task Connect()
    {
        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        isConnected = mqttClient.IsConnected;
    }

    public async Task Disconnect()
    {
        await mqttClient.DisconnectAsync();
        isConnected = mqttClient.IsConnected;
    }
}
