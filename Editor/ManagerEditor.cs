using UnityEngine;
using UnityEditor;

namespace UniTac {
[CustomEditor (typeof(Manager))]
    public class InterfaceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Manager manager = (Manager) target;

            // Button to add another sensor
            if (GUILayout.Button("Add Sensor")) {
                GameObject sensor = new("Sensor", typeof(Sensor));
                sensor.transform.SetParent(manager.transform);
                sensor.transform.SetPositionAndRotation(manager.transform.position, manager.transform.rotation);
            }
        }
    }
}
