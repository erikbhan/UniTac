using UnityEngine;

namespace Singleton {
    public enum LogLevel { None, Error, Warning, All }

    public class Interface : MonoBehaviour
    {
        public GameObject spawnObject = null;
        public LogLevel logLevel = LogLevel.Warning;
        public bool debugMenu = false;

        Singleton.Logger logger = Singleton.Logger.Instance;
        Singleton.Server server = Singleton.Server.Instance;
        Singleton.Client client = Singleton.Client.Instance;

        // Start is called before the first frame update
        void Start()
        {
            logger.SetLogLevel(this.logLevel);
            server.StartAsync();
            client.Connect();
        }

        void Update()
        {
            
        }

        void OnGUI() {
            if (debugMenu) {
                if (GUILayout.Button("Press Me"))
                    Debug.Log("Hello!");
            }
        }
    }
}