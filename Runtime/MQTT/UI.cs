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
    Client client;
    bool serverRunning;

    private void Start()
    {
        Logger logger = gameObject.AddComponent<Logger>();
        server = new Server(logger);
        client = new Client(logger);
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

    private async void ClientWindow(int id)
    {
        GUILayout.Label("Client connected: " + client.IsConnected().ToString());

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Connect"))
        {
            await client.Connect();
        }

        if (GUILayout.Button("Disconnect"))
        {
            await client.Disconnect();
        }
        GUILayout.EndHorizontal();

        GUI.DragWindow();
    }
}
