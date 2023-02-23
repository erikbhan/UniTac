using MQTTnet.Diagnostics;
using System;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

#nullable enable
public class Logger : MonoBehaviour, IMqttNetLogger
{
    public bool IsEnabled => true;

    public void Publish(MqttNetLogLevel logLevel, string source, string message, object[]? parameters, Exception? exception)
    {
        if (parameters?.Length > 0) { message = string.Format(message, parameters); }
        if (exception != null) Debug.LogException(exception);
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
    }

    public void Log(object e)
    {
        var output = "NULL";
        if (e != null)
        {
            output = e.ToString();
        }
        Debug.Log(output);
    }
}
