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
    private Session CurrentSession = new(false);

    // Readonly
    public float CurrentSessionLength { get; private set; } = 0f;
    public Session LastSession { get; private set; }
    public Dictionary<long, Entity> Entities { get; private set; } = new();
    public bool IsActive { get; private set; } = false;

    /// <summary>
    /// Processes the received message data. 
    /// Updates existing dictionary of <see cref="Entity"/> entities to the new entities received in the message.
    /// Also reset the idle countdown to maintain active session.
    /// </summary>
    /// <param name="payload">Payload from the client; holds data from the sensor</param>
    public void HandleMessage(Payload payload) {
        idleTimer = secondsUntilIdle;
        var temp = new Dictionary<long, Entity>();
        foreach (Entity e in payload.Entities.Values.ToList())
        {
            temp.Add(e.Id, e);
        }
        Entities = temp;
    }

    public void Update()
    {
        // Update session length
        CurrentSessionLength += Time.deltaTime;

        // Update active status
        if (IsActive)
        {
            if (idleTimer > 0) idleTimer -= Time.deltaTime;
            else
            {
                Entities = new();
                UpdateSession();
            }
        }
        else
        {
            if (idleTimer > 0) UpdateSession();
        }
    }

    /// <summary>
    /// Updates the current and last sessions at the session end of a session
    /// </summary>
    private void UpdateSession() {
        IsActive = !IsActive;
        CurrentSession.SessionLength = CurrentSessionLength;
        LastSession = CurrentSession;
        CurrentSession = new Session(IsActive);
        CurrentSessionLength = 0f;
    }
}
