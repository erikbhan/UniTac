using UnityEngine;
using UnityEditor;

namespace UniTac
{
    [CustomEditor(typeof(Sensor))]
    public class SensorEditor : Editor
    {
        public float arrowSize = 1;

        void OnSceneGUI()
        {
            Sensor t = target as Sensor;

            Handles.color = Color.blue;
            Handles.Label(t.transform.position + Vector3.up * 2,
                                 t.transform.position.ToString() + "\nFieldOfView: " +
                                 t.fieldOfView.ToString());

            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(Screen.width - 100, Screen.height - 80, 90, 50));

            if (GUILayout.Button("Reset Area"))
                t.fieldOfView = 5;

            GUILayout.EndArea();
            Handles.EndGUI();

            Handles.color = new Color(1, 1, 1, 0.2f);

            Quaternion from = Quaternion.identity;
            from.eulerAngles = new Vector3(0, 30, 0);

            Handles.DrawSolidArc(t.transform.position, t.transform.up, from * -t.transform.right,
                                    120, t.fieldOfView);

            Handles.color = Color.white;
            t.fieldOfView = Handles.ScaleValueHandle(t.fieldOfView,
                            t.transform.position + t.transform.forward * t.fieldOfView,
                            t.transform.rotation, 1, Handles.ConeHandleCap, 1);
        }
    }
}