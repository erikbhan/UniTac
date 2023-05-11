# SensMax TAC-B Unity integration

This Unity package is an integration of the SensMax's TAC-B line of sensors. It uses the [MQTTnet](https://github.com/dotnet/MQTTnet) package to communicate with the sensor. It facilitates the use of multiple sensors in one project.

![Gif of spawn entities sample in use]()

## Installation

To import our package into Unity, you can use Unity's built-in Package Manager. You find this under "Window" > "Package Manager" in the Unity Editor.

![Opening Package Manager]()

In the Package Manager window, press the "+"-button, and then "Add package from git URL...". You can use both HTTPS and SSH to import the package.

![Adding from git]()

## Usage

After importing our package, you first need a manager to administrate the sensor. Click and drag our pre-made prefab into either the Unity Scene or the Unity Hierarchy from the Package folder. To troubleshoot the MQTT client or server, you can change the logging level in the manager prefab's Inspector window. You can also set a custom port if you'd like.

![Manager inspector window]()

To handle incoming messages any sensors needs to be added. Use the "Add Sensor"-button on the manager Inspector window to add any number of sensors.

![Add sensor button]()

Configure each sensor via the Inspector window; you need to set the "serial" field equal to the physical sensor's serial number. Seconds until idle is simply the time from the last message until the sensor considers itself "idle".

![Sensor inspector window]()

Finally, you need to add functionality to the sensor, either by adding a sample or your own script. Simply drag the wanted functionality onto the sensor GameObject in the Unity Hierarchy, then configure via the Inspector again (for example, if you need to make a 3D object to use as a prefab for spawning).

### Sensor Events

The sensor implements events to facilitate observer pattern scripts. The sensor has a low update rate so it's recommended to utilize events to update only when new data is available.

| UnityEvent | Functionality |
| --- | --- |
| MessageReceivedEvent | This event is invoked when the sensor-object receives a new update from the physical sensor. |
| StatusChangedEvent | This event is invoked when the sensor changes active/idle-status. |

### Samples

| Sample | Functionality |
| --- | --- |
| SpawnOnEntity | Spawns and maintains the local position of GameObject for each object detected by the sensor. Needs a GameObject to spawn |
| LogSessionData | Logs the session data in a txt-file in JSON-format. Session data includes: active/idle-status, start time and session length. May be given a path to generate file in desired location. Default path: ./Assets/SensorSerial-Log.txt |
| StateMachine | Switches between each gameObject based on the active/idle-status of the sensor. Needs three individual GameObjects. |

### Using credentials to authorize MQTT-connections

The manager can be set up with a username and password so that connections can be authorized. This does not mean packets cannot be intercepted, but stops connections that does not have the right credentials. To use credentials add a txt-file containing this to your project:

```JSON
{
    "username":"YOUR_USERNAME",
    "password":"SECRET_PASSWORD"
}
```

Then add the relative path to this file to the "Secret File Path"-variable in the manager inspector window. The relative path to your file can be found in Unity by right clicking it and selecting "Copy path".

### Saving 5-minute-packets

The sensor automatically sends a "5-minute-data"-packet every 5 minutes. As a default the manager discards these to not waste time. The script "DataPacketClient" can be added to manager to log these messages in a separate file. After adding the script the path variable may be left empty to automatically save the data
in "Assets/5-minute-message-log.txt" or set to another path to automatically save the data there.

### Documentation

For concrete examples on how to use our library, please see the `Samples` folder.

For API documentation, please see the [wiki](https://erikbhan.github.io/UniTac/).

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## Project status

This project is a bachelor thesis project. This means that after the project period ends further development from the original team is unlikely.

## License

[MIT](https://choosealicense.com/licenses/mit/)
