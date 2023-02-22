using MQTTnet;
using MQTTnet.Diagnostics;
using MQTTnet.Server;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Server : MonoBehaviour
{
    [SerializeField] bool enableLogging = false;
    [SerializeField] int port = 1883;
    [SerializeField] int connectionBacklog = 100;

    Coroutine startServerCoroutine;
    Coroutine stopServerCoroutine;
    MqttServer mqttServer;

    string isStarted = "lorem ipsum";
    
    void OnGUI()
    {
        if (GUILayout.Button("Start"))
        {
            if (mqttServer != null && mqttServer.IsStarted)
            {
                Debug.Log("Server already started");
                return;
            }

            if (stopServerCoroutine != null) StopCoroutine(stopServerCoroutine);
            startServerCoroutine = StartCoroutine(StartServerAsync());
        }

        if (GUILayout.Button("Stop"))
        {
            if (mqttServer == null || !mqttServer.IsStarted)
            {
                Debug.Log("Server already stopped");
                return;
            }

            if (startServerCoroutine != null) StopCoroutine(startServerCoroutine);
            stopServerCoroutine = StartCoroutine(StopServerAsync());
        }

        GUILayout.Label("isStarted: " + isStarted);
    }

    void Update()
    {
        if (mqttServer != null) isStarted = mqttServer.IsStarted.ToString();
        else isStarted = "null";
    }

    /// <summary>
    /// A Unity coroutine that creates and asynchronously starts the MQTT server
    /// </summary>
    IEnumerator StartServerAsync()
    {
        var mqttFactory = new MqttFactory();
        if (enableLogging) mqttFactory = new MqttFactory(new ConsoleLogger());

        var mqttServerOptions = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(port)
            .WithConnectionBacklog(connectionBacklog)
            .Build();

        mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);
        var t = Task.Run(async () => await mqttServer.StartAsync());
        yield return new WaitUntil(() => t.IsCompleted);
    }

    /// <summary>
    /// Unity coroutine that asynchronously stops the MQTT server
    /// </summary>
    IEnumerator StopServerAsync()
    {
        var t = Task.Run(async () => await mqttServer?.StopAsync());
        yield return new WaitWhile(() => t.IsCompleted);
    }

    /// <summary>
    /// A logger that communicates with the MQTT server
    /// </summary>
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
