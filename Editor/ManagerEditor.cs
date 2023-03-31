using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Interface))]
public class ManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.HelpBox("This is a help box", MessageType.Info);


        Interface manager = (Interface) target;
        if (GUILayout.Button("Add a sensor"))
        {
            Instantiate(manager.transform.GetChild(0), manager.transform, false);
        }
    }
}
