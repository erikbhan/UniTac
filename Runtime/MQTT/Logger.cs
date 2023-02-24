#nullable enable
using MQTTnet.Diagnostics;
using System;
using UnityEngine;

/// <summary>
/// Class that handles the Debug consol for <see cref="Client"/> and <see cref="Server"/> 
/// </summary>
public class Logger : MonoBehaviour, IMqttNetLogger
{
    public bool IsEnabled => true;

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

    /// <summary>
    /// <inheritdoc cref="Debug.Log(object)"/>
    /// </summary>
    /// <param name="input">Object to log using <see cref="object.ToString"/></param>
    public void Log(object? input)
    {
        Debug.Log(input != null ? input.ToString() : "null");
    }

    /// <summary>
    /// <inheritdoc cref="Debug.LogError(object)"/>
    /// </summary>
    /// <param name="input">Object to log error using <see cref="object.ToString"/></param>
    public void LogError(object input) 
    { 
        Debug.LogError(input.ToString()); 
    }

    /// <summary>
    /// <inheritdoc cref="Debug.LogWarning(object)"/>
    /// </summary>
    /// <param name="input">Object to log warning using <see cref="object.ToString"/></param>
    public void LogWarning(object input)
    {
        Debug.LogWarning(input.ToString());
    }
}
