using UnityEngine;
using UnityEditor;

namespace UniTac.EditorUI 
{
    /// <summary>
    /// Custom editor for <see cref="Manager"/> that adds button to add sensors.
    /// </summary>
    [CustomEditor (typeof(Manager))]
    public class ManagerEditor : Editor
    {
        /// <summary>
        /// Draws the standard inspector and with the "Add sensor"-button.
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
