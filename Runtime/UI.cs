using System;
using System.Collections;
using UnityEngine;

public class UI : MonoBehaviour
{
    Rect serverWindowRect = new(20, 20, 150, 0);
    Rect clientWindowRect = new(190, 20, 0, 0);

    void OnGUI()
    {
        serverWindowRect = GUILayout.Window(0, serverWindowRect, ServerWindow, "Server");
        clientWindowRect = GUILayout.Window(1, clientWindowRect, ClientWindow, "Client");
    }

    private void ServerWindow(int id)
    {
        GUILayout.Label("Server status: " + false.ToString());

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Start"))
        {
            // Do checks
            // Start server if all good
        }
        if (GUILayout.Button("Stop"))
        {
            // Do checks
            // Stop server if all good
        }
        GUILayout.EndHorizontal();

        GUI.DragWindow();
    }

    private void ClientWindow(int id)
    {
        if (GUILayout.Button("Connect"))
        {
            // Do checks
            // Connect to server if all good
        }

        GUI.DragWindow();
    }
}
