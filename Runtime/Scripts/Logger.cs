using System;
using MQTTnet.Diagnostics;
using UnityEngine;

#nullable enable

/// <summary>
/// Enum for keeping track of log levels
/// </summary>
public enum LogLevel { None, Error, Warning, All }

/// <summary>
/// Class that logs to console for <see cref="Client"/> and <see cref="Server"/> 
/// </summary>
public class Logger : IMqttNetLogger
{
    public LogLevel logLevel {get; set;} = LogLevel.Warning;
    bool IMqttNetLogger.IsEnabled => true;

    /// <summary>
    /// Logs a messages from parent
    /// </summary>
    /// <param name="logLevel">Type of log received</param>
    /// <param name="source">Source of massage</param>
    /// <param name="message">Message content</param>
    /// <param name="parameters">Optional parameters of message</param>
    /// <param name="exception">Optional exception</param>
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
