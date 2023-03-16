using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Script for the sensor gameobject that handles functionality
/// </summary>
public class Sensor : MonoBehaviour
{
    // Config
    public string serial = "";
    public float secondsUntilIdle = 2f;

    
    // Private
    private float idleTimer = 0f;
    private float currentSessionLength = 0f;
    private Session currentSession = new(false);
    private Session lastSession;
    private Dictionary<long, Entity> entities = new();
    public bool isActive { get; private set; } = false;

    /// <summary>
    /// Processes the received message data
    /// </summary>
    /// <param name="payload">Payload from the client; holds data from the sensor</param>
    public void HandleMessage(Payload payload) {
        idleTimer = secondsUntilIdle;
        var temp = new Dictionary<long, Entity>();
        foreach (Entity e in payload.Entities.Values.ToList())
        {
            temp.Add(e.Id, e);
        }
        entities = temp;
    }

    public void Update()
    {
        currentSessionLength += Time.deltaTime;
        if (isActive)
        {
            if (idleTimer > 0) idleTimer -= Time.deltaTime;
            else
            {
                entities = new();
                UpdateSession();
            }
        }
        else
        {
            if (idleTimer > 0) UpdateSession();
        }
    }

    private void UpdateSession() {
        isActive = !isActive;
        currentSession.sessionLength = currentSessionLength;
        lastSession = currentSession;
        currentSession = new Session(isActive);
        currentSessionLength = 0f;
    }
}
