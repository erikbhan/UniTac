# SensMax TAC-B Unity integration

This Unity package is an integration of the SensMax's TAC-B line of sensors. It uses the [MQTTnet](https://github.com/dotnet/MQTTnet) package to communicate with the sensor.

## Installation

To import our package into Unity, you can use Unity's built-in Package Manager. You find this under "Window" > "Package Manager" in the Unity Editor. In the Package Manager window, press the "+"-button, and then "Add package from git URL...". You can use both HTTPS and SSH to import the package.

## Usage

After importing our package, you first need a manager to administrate the sensor. Click and drag our pre-made prefab into either the Unity Scene or the Unity Hierarchy from the Package folder. To troubleshoot the MQTT client or server, you can change the logging level in the manager prefab's Inspector window. You can also set a custom port if you'd like.

To handle incoming messages any sensors needs to be added. Use the "Add Sensor"-button on the manager Inspector window to add any number of sensors. Configure each sensor via the Inspector window; you need to set the "serial" field equal to the physical sensor's serial number. Seconds until idle is simply the time from the last message until the sensor considers itself "idle".

Finally, you need to add functionality to the sensor, either by adding a sample or your own script. Simply drag the wanted functionality onto the sensor GameObject in the Unity Hierarchy, then configure via the Inspector again (for example, if you need to make a 3D object to use as a prefab for spawning).

### Samples

| Sample | Functionality |
| --- | --- |
| SpawnOnEntity | Spawns the given gameObject for each object detected by the sensor. |
| LogSessionData | Logs the session data in a txt-file. Session data includes: active/idle-status, start time and session lenght. |
| StateMachine | Switches between each gameObject based on the active/idle-status of the sensor. |

### Using credentials to authorize MQTT-connections

The manager can be set up with a username and password so that connections can be authorized. This does not mean packets cannot be intercepted, but stops connections that does not have the right credetials. To use credentials add this file to the project:

```JSON
{
    "username":"YOUR_USERNAME",
    "password":"SECRET_PASSWORD"
}
```

Then add the path to this file to the "Secret File Path"-variable in the manager inspector window.

### Logging 5-minute-packets

The sensor automaticly sends a "5-minute-data"-packet every 5 minutes. As a default the manager discards these to not waste time. The script "DataPacketClient" can be added to manager to log these messages in a separate file.

### Documentation

For concrete examples on how to use our library, please see the `Samples` folder.

For API documentation, please see the [wiki](https://erikbhan.github.io/bachelor/).

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[MIT](https://choosealicense.com/licenses/mit/)
