using MQTTnet;
using MQTTnet.Diagnostics;
using MQTTnet.Server;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class MQTTServer : MonoBehaviour
{
    private void Start()
    {
        Run_Server_With_Logging();
    }

    public async void Run_Server_With_Logging()
    {
        var mqttFactory = new MqttFactory(new ConsoleLogger());
        var mqttServerOptions = new MqttServerOptionsBuilder().WithDefaultEndpoint().Build();
        var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);
        await mqttServer.StartAsync();
        await Task.Yield();
    }

    class ConsoleLogger : IMqttNetLogger
    {
        readonly object _consoleSyncRoot = new();

        public bool IsEnabled => true;

#nullable enable
        public void Publish(MqttNetLogLevel logLevel, string source, string message, object[]? parameters, Exception? exception)
        {
            if (parameters?.Length > 0) { message = string.Format(message, parameters); }

            lock (_consoleSyncRoot)
            {
                Debug.Log(message);
                if (exception != null) { Debug.LogError(exception); }
            }
        }
#nullable disable
    }
}
