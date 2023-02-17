using MQTTnet;
using MQTTnet.Diagnostics;
using MQTTnet.Server;
using System;
using System.Collections;
using UnityEngine;

public class MQTTServer : MonoBehaviour
{
    [SerializeField] bool enableLogging = false;
    [SerializeField] int port = 1883;
    [SerializeField] int connectionBacklog = 100;

    Coroutine startServerCoroutine;
    MqttServer mqttServer;

    public void Start()
    {
        if (startServerCoroutine == null && mqttServer == null)
        {
            startServerCoroutine = StartCoroutine(StartServerAsync());
        } 
        else
        {
            Debug.LogWarning("Server already starting or started");
        }
    }

    IEnumerator StartServerAsync()
    {
        var mqttServerOptions = new MqttServerOptionsBuilder()
            .WithConnectionBacklog(connectionBacklog)
            .WithDefaultEndpointPort(port)
            .Build();

        MqttFactory mqttFactory;
        if (enableLogging)
        {
            mqttFactory = new MqttFactory(new ConsoleLogger());
        }
        else
        {
            mqttFactory = new MqttFactory();
        }
        using (mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions))
        {
            mqttServer.StartAsync();
            yield return new WaitUntil(() => mqttServer.IsStarted);
        }
    }

    class ConsoleLogger : IMqttNetLogger
    {
        readonly object _consoleSyncRoot = new();
        public bool IsEnabled => true;

#nullable enable
        public void Publish(MqttNetLogLevel logLevel, string source, string message, object[]? parameters, Exception? exception)
#nullable disable
        {
            if (parameters?.Length > 0) { message = string.Format(message, parameters); }

            lock (_consoleSyncRoot)
            {
                switch (logLevel)
                {
                    case MqttNetLogLevel.Warning:
                        Debug.LogWarning(message);
                        break;

                    case MqttNetLogLevel.Error:
                        Debug.LogError(message);
                        break;

                    default:
                        Debug.Log(message);
                        break;
                }

                if (exception != null) { Debug.LogException(exception); }
            }
        }
    }
}
