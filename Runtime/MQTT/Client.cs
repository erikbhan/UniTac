using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;

public class Client
{
    private readonly IMqttClient mqttClient;
    private readonly MqttClientOptions mqttClientOptions;
    private readonly MqttClientSubscribeOptions mqttSubscribeOptions;
    private readonly Logger logger;
    private Payload payload;
    private DateTime lastMessageReceived;

    public Client(Logger? logger = null)
    {
        this.logger = logger;
        var mqttFactory = new MqttFactory();

        this.mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("192.168.0.185")
            .Build();
        if (logger != null) mqttClient = mqttFactory.CreateMqttClient(logger);
        else mqttClient = mqttFactory.CreateMqttClient();

        mqttClient.ApplicationMessageReceivedAsync += e => MessageHandeler(e);

        this.mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("smx/device/051001572/position");
                    })
                .Build();
    }

    public async Task Connect()
    {
        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
    }

    public async Task Disconnect()
    {
        await mqttClient.DisconnectAsync();
    }

    public bool IsConnected()
    {
        return mqttClient.IsConnected;
    }

    private Task MessageHandeler(MqttApplicationMessageReceivedEventArgs incommingEvent)
    {
        lastMessageReceived = DateTime.Now;
        var bytes = incommingEvent.ApplicationMessage.Payload;
        payload = UnpackPayload(bytes);
        if(logger != null) logger.Log(payload);
        return Task.CompletedTask;
    }

    private Payload UnpackPayload(byte[] bytes)
    {
        var json = Encoding.UTF8.GetString(bytes);
        var payload = JsonConvert.DeserializeObject<Payload>(json);
        return payload;
    }
}
