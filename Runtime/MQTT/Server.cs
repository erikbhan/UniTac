using MQTTnet;
using MQTTnet.Server;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Server
{
    private Logger logger;
    private MqttServer mqttServer;
    public Server(Logger logger = null)
    {
        this.logger = logger;
    }

    public async Task Start()
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
        await mqttServer.StartAsync();
    }

    public async Task Stop() {
        await mqttServer.StopAsync();
    }
}
