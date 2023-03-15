using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json;

public class Interface : MonoBehaviour
{
    public LogLevel serverLogLevel = LogLevel.Warning;
    public LogLevel clientLogLevel = LogLevel.Warning;
    public bool debugMenu = false;
    public int serverPort = 1883;
    public string clientIP = "127.0.0.1";

    Logger serverLogger = new Logger();
    Logger clientLogger = new Logger();
    MqttServer server;
    IMqttClient client;

    void Start() { Init(); }

    async void Init() {
        server = CreateServer();
        client = CreateClient();
        await server.StartAsync();
        await ConnectClient();
        await SubscribeClient();
    }

    async Task ConnectClient()
    {
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(clientIP)
            .Build();
        await client.ConnectAsync(mqttClientOptions);
    }

    async Task SubscribeClient()
    {
        var mqttSubscribeOptions = (new MqttFactory()).CreateSubscribeOptionsBuilder()
            .WithTopicFilter(
                f => { f.WithTopic("smx/device/+/position"); })
            .Build();
        await client.SubscribeAsync(mqttSubscribeOptions);        
    }

    void Update()
    {
        // TODO: use events vs checking every frame
        if (clientLogger.logLevel != this.clientLogLevel)
            clientLogger.logLevel = this.clientLogLevel;
        if (serverLogger.logLevel != this.serverLogLevel)
            serverLogger.logLevel = this.serverLogLevel;
    }

    MqttServer CreateServer() {
        var mqttFactory = new MqttFactory(serverLogger);
        var MqttServerOptions = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(serverPort)
            .Build();
        return mqttFactory.CreateMqttServer(MqttServerOptions);
    }

    IMqttClient CreateClient() {
        var mqttFactory = new MqttFactory(clientLogger);
        var client = mqttFactory.CreateMqttClient();
        client.ApplicationMessageReceivedAsync += e => MessageHandler(e);
        return client;
    }

    private Task MessageHandler(MqttApplicationMessageReceivedEventArgs e)
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

    // TODO; make debug menu
    void OnGUI() {
        if (debugMenu) {
            if (GUILayout.Button("Press Me"))
                Debug.Log("Hello!");
        }
    }
}