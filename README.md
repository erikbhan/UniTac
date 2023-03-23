# SensMax TAC-B Unity integration

This Unity package is an integration of the SensMax's TAC-B line of sensors. It uses the [MQTTnet](https://github.com/dotnet/MQTTnet) package to communicate with the sensor.

## Installation

To import our package into Unity, you can use Unity's built-in Package Manager. You find this under "Window" > "Package Manager" in the Unity Editor. In the Package Manager window, press the "+"-button, and then "Add package from git URL...". You can use both HTTPS and SSH to import the package.

## Usage

After importing our package, you first need a manager to administrate the sensor. Click and drag our pre-made prefab into either the Unity Scene or the Unity Hierarchy from the Package folder. To troubleshoot the MQTT client or server, you can change the logging level in the manager prefab's Inspector window. You can also set a custom port if you'd like.

Then you need to drag and drop a sensor prefab onto the manager for each physical sensor you'd like to connect to. These sensor prefabs represent the physical sensor in the Unity world. Configure each sensor prefab via the Inspector window; you need to set the "serial" field equal to the physical sensor's serial number. Seconds until idle is simply the time from the last message until the sensor considers itself "idle".

Finally, you need to add functionality to the sensor. There is sample code in the package folder under "Runtime" > "Samples". Simply drag the wanted functionality onto the sensor GameObject in the Unity Hierarchy, then configure via the Inspector again (for example, if you need to make a 3D object to use as a prefab for spawning).

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[MIT](https://choosealicense.com/licenses/mit/)