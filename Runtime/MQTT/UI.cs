using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.VersionControl;
using UnityEngine;

public class UI : MonoBehaviour
{
    Rect serverWindowRect = new(20, 20, 150, 0);
    Rect clientWindowRect = new(190, 20, 0, 0);
    Server server;
    bool serverRunning;

    private void Start()
    {
        server = new Server(gameObject.AddComponent<Logger>());

    }

    void OnGUI()
    {
        if (server != null) serverRunning = server.IsStarted();
        serverWindowRect = GUILayout.Window(0, serverWindowRect, ServerWindow, "Server");
        clientWindowRect = GUILayout.Window(1, clientWindowRect, ClientWindow, "Client");
    }

    private async void ServerWindow(int id)
    {
        GUILayout.Label("Server running: " + serverRunning.ToString());

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Start"))
        {
            await server.Start();
        }

        if (GUILayout.Button("Stop"))
        {
            await server.Stop();
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
