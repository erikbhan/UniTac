#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UniTac.EditorUI 
{
    /// <summary>
    /// Extension to the <see cref="Manager"/>'s inspector GUI. Adds a button which instantiates sensor game objects as children to the manager.
    /// </summary>
    [CustomEditor (typeof(Manager))]
    public class ManagerEditor : Editor
    {
        /// <summary>
        /// Draws the standard inspector, and then the "Add sensor"-button.
        /// </summary>
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
#endif