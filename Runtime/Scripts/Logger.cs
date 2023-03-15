using System;
using MQTTnet.Diagnostics;
using UnityEngine;

#nullable enable

public enum LogLevel { None, Error, Warning, All }

public class Logger : IMqttNetLogger
{
    public LogLevel logLevel {get; set;} = LogLevel.Warning;
    public bool IsEnabled => true;

    public void Publish(MqttNetLogLevel logLevel, string source, string message, object[]? parameters, Exception? exception)
    {
        if (this.logLevel == LogLevel.None) return;

        if (parameters?.Length > 0) { message = string.Format(message, parameters); }
        if (exception != null) Debug.LogException(exception);

        switch (logLevel)
        {
            case MqttNetLogLevel.Error:
                Debug.LogError(message);
                break;
            case MqttNetLogLevel.Warning:
                if (this.logLevel == LogLevel.Error) break;
                Debug.LogWarning(message);
                break;
            default:
                if (this.logLevel != LogLevel.All) break;
                Debug.Log(message);
                break;
        }
    }
}

