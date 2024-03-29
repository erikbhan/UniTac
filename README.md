# SensMax TAC-B Unity integration

This Unity package is an integration of the SensMax's TAC-B line of sensors. It uses the [MQTTnet](https://github.com/dotnet/MQTTnet) package to communicate with the sensor. It facilitates the use of multiple sensors in one project. This package is made for 3D projects, but is equally usable in 2D projects.

![Gif of spawn entities sample in use](https://github.com/erikbhan/UniTac/assets/42799026/6a9239cf-9510-4e96-bf74-1dcd17c3b8ef)

## Installation

To import our package into Unity, you can use Unity's built-in Package Manager. You find this under "Window" > "Package Manager" in the Unity Editor.

![Opening Package Manager](https://github.com/erikbhan/UniTac/assets/42799026/3db44466-6439-45c1-84af-aa3183f58b16)

In the Package Manager window, press the "+"-button, and then "Add package from git URL...". You can use both HTTPS and SSH to import the package.

![Adding from git](https://github.com/erikbhan/UniTac/assets/42799026/fca9382e-c42a-47cb-a244-1ee49729b7ab)

## Usage

After importing our package, you first need a manager to administrate the sensor. Click and drag our pre-made prefab into either the Unity Scene or the Unity Hierarchy from the Package folder. To troubleshoot the MQTT client or server, you can change the logging level in the manager prefab's Inspector window. You can also set a custom port if you'd like. Keep logging disabled in final product to maximize efficiency.

![Manager inspector window](https://github.com/erikbhan/UniTac/assets/42799026/cb563644-6f36-448c-9acc-9b1c875bf64b)

To handle incoming messages any sensors needs to be added. Use the "Add Sensor"-button on the manager Inspector window to add any number of sensors.

![Add sensor button](https://github.com/erikbhan/UniTac/assets/42799026/2e2a6da1-d3ec-4657-a13f-740ea732c262)

Configure each sensor via the Inspector window; you need to set the "serial" field equal to the physical sensor's serial number. Seconds until idle is simply the time from the last message until the sensor considers itself "idle". If using the sensor in a 2D project the sensor rotation must be set to X=90.

![Sensor inspector window](https://github.com/erikbhan/UniTac/assets/42799026/cd2ff6c6-67d4-4ab5-bce4-b0b6151444d7)

Finally, you need to add functionality to the sensor, either by adding a sample or your own script. Simply drag the wanted functionality onto the sensor GameObject in the Unity Hierarchy or use the "Add component"-button in the sensor inspector window.

### Sensor Events

The sensor implements the [observer pattern](https://en.wikipedia.org/wiki/Observer_pattern), utilizing [UnityEvent](https://docs.unity3d.com/ScriptReference/Events.UnityEvent.html). Because the sensor only sends a new packet every 100ms, we avoid some overhead by not checking the sensor every frame. In addition, you can add as many listeners as you want without creating unnecessary coupling in your code.

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

The manager can be set up with a username and password so that connections can be authorized. This does not mean packets cannot be intercepted, but stops connections that does not have the right credentials. Keep in mind that password is not encrypted when sent from the sensor so any password used here is compromised. To use credentials add a txt-file containing this to your project:

```JSON
{
    "username":"YOUR_USERNAME",
    "password":"SECRET_PASSWORD"
}
```

Then add the relative path to this file to the "Secret File Path"-variable in the manager inspector window. The relative path to your file can be found in Unity by right clicking it and selecting "Copy path".

![ManagerInspector-secrets](https://github.com/erikbhan/UniTac/assets/42799026/9cbc03c5-3410-48e0-ba75-e7f8cfffb850)

### Saving 5-minute-packets

The sensor automatically sends a "5-minute-data"-packet every 5 minutes. As a default the manager discards these to not waste time. The script "DataPacketClient" can be added to manager to log these messages in a separate file.

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
