using UnityEngine;
using UnityEditor;

namespace UniTac.EditorUI
{
    /// <summary>
    /// Custom editor for <see cref="Sensor"/> that visualizes the sensor's field of view.
    /// </summary>
    [CustomEditor(typeof(Sensor))]
    public class SensorEditor : Editor
    {
        /// <summary>
        /// Draws sensor field of view in scene based on <see cref="Sensor.RangeOfView"/>.
        /// </summary>
        void OnSceneGUI()
        {
            Sensor t = target as Sensor;

            Handles.color = Color.blue;
            Handles.Label(t.transform.position + Vector3.up * 2,
                                 t.transform.position.ToString() + "\nRangeOfView: " +
                                 t.RangeOfView.ToString());

            Handles.color = new Color(1, 1, 1, 0.2f);

            Quaternion from = Quaternion.identity;
            from.eulerAngles = new Vector3(0, 30, 0);
            Handles.DrawSolidArc(t.transform.position, t.transform.up, from * -t.transform.right,
                                    120, t.RangeOfView);

            Handles.color = Color.white;
            t.RangeOfView = Handles.ScaleValueHandle(t.RangeOfView,
                            t.transform.position + t.transform.forward * t.RangeOfView,
                            t.transform.rotation, 1, Handles.ConeHandleCap, 1);
        }
    }
}