using System;
using MQTTnet.Diagnostics;
using UnityEngine;

#nullable enable

namespace Singleton {
// From https://csharpindepth.com/Articles/Singleton
    public sealed class Logger : IMqttNetLogger
    {
        private static readonly Logger instance = new Logger();
        private LogLevel logLevel = LogLevel.All;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Logger() {}

        private Logger() {}

        public static Logger Instance { get { return instance; } }

        public bool IsEnabled => true;

        public void SetLogLevel(LogLevel logLevel) { this.logLevel = logLevel; }

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
                    // TODO: find a better way to compare enums
                    if (this.logLevel == LogLevel.Error) break;
                    if (this.logLevel == LogLevel.Warning) break;
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
}

