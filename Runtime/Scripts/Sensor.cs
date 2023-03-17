#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Script for the sensor gameobject that handles data management. Script can be added to add utility.
/// </summary>
public class Sensor : MonoBehaviour
{
    // Config
    public string Serial { get; set; } = "";
    public float SecondsUntilIdle { get; set; } = 2f;

    // Private
    private float IdleTimer = 0f;
    private Session CurrentSession = new(false);

    // Readonly
    public float CurrentSessionLength { get; private set; } = 0f;
    public Session? LastSession { get; private set; }
    public Dictionary<long, Entity> Entities { get; private set; } = new();
    public bool IsActive { get; private set; } = false;

    /// <summary>
    /// Processes the received message data. 
    /// Updates existing dictionary of <see cref="Entity"/> entities to the new entities received in the message.
    /// Also reset the idle countdown to maintain active session.
    /// </summary>
    /// <param name="payload">Payload from the client; holds data from the sensor</param>
    public void HandleMessage(Payload payload) {
        IdleTimer = SecondsUntilIdle;
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
            if (IdleTimer > 0) IdleTimer -= Time.deltaTime;
            else
            {
                Entities = new();
                UpdateSession();
            }
        }
        else
        {
            if (IdleTimer > 0) UpdateSession();
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

    /// <summary>
    /// Gets the closest entity to the sensor
    /// </summary>
    /// <returns><see cref="Entity"/> the entity closest to the sensor or null no entities are present </returns>
    public Entity? GetClosestEntity()
    {
        if (!Entities.Any()) return null;
        Entity? closestEntity = null;
        double distance = Double.PositiveInfinity;
        foreach (var e in Entities.Values)
        {
            var d = e.DistanceFromParent();
            if (d < distance)
            {
                distance = d;
                closestEntity = e;
            }
        }
        return closestEntity;
    }

    /// <summary>
    /// Gets the entity with the given id
    /// </summary>
    /// <param name="id">The id of the entity</param>
    /// <returns><see cref="Entity"/> the entity or null if not found</returns>
    public Entity? GetEntity(long id) { 
        return Entities.ContainsKey(id) ? Entities[id] : null; 
    }
}
