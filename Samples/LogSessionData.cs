using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UniTac;

/// <summary>
/// Sample monobehaviour that saves sensor session data to a separate log file.
/// </summary>
public class LogSessionData : MonoBehaviour
{
    /// <summary>
    /// Relative path to log file. Default path is set to "./Assets/SerialNumber-Log.txt".
    /// </summary>
    [Tooltip("Relative path to log file. Default path is set to \"./Assets/SerialNumber-Log.txt\".")]
    public string Path = "";
    private Sensor Sensor = null;

    /// <summary>
    /// Start is called before the first frame update.
    /// Gets the sensor and creates a log file if it does not already exist.
    /// </summary>
    void Start()
    {
        Sensor = gameObject.GetComponent<Sensor>();
        if (!Sensor)
        {
            Debug.LogError("Sensor not found");
            Application.Quit();
        }
        Sensor.StatusChangedEvent.AddListener(LogStatus);

        if (Path == "") Path = "./Assets/" + Sensor.Serial + "-Log.txt";
        if (!File.Exists(Path))
        {
            File.Create(Path);
        }
        if (!File.Exists(Path))
        {
            Debug.LogError("Could not create file!");
        }
    }

    /// <summary>
    /// Writes the session data to the file.
    /// </summary>
    private void LogStatus()
    {
        var data = JsonConvert.SerializeObject(Sensor.LastSession);
        using FileStream fs = new(Path, FileMode.Append, FileAccess.Write);
        using StreamWriter sw = new(fs);
        sw.WriteLine(data + ",");
    }
}
