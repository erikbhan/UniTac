using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace UniTac {
    /// <summary>
    /// Script for the sensor gameobject that handles functionality.
    /// </summary>
    public class Sensor : MonoBehaviour
    {
        /// <summary>
        /// Serial of the sensor to handle events from.
        /// </summary>
        public string Serial = "";
        /// <summary>
        /// Seconds without messages from sensor before the session set to idle.
        /// </summary>
        public float SecondsUntilIdle = 2f;
        /// <summary>
        /// Event that is invoked when the a message is received.
        /// </summary>
        public UnityEvent MessageReceivedEvent { get; } = new();
        /// <summary>
        /// Event that is envoked when the sensor changes active/idle status.
        /// </summary>
        public UnityEvent StatusChangedEvent { get; } = new();
        /// <summary>
        /// Running lenght of the current session.
        /// </summary>
        public float CurrentSessionLength { get; private set; } = 0f;
        /// <summary>
        /// Data from the session before the current session. 
        /// Null if current session is the first session.
        /// </summary>
        #nullable enable
        public Session? LastSession { get; private set; }
        #nullable disable
        /// <summary>
        /// List of entities currently detected by the sensor.
        /// </summary>
        public Dictionary<long, Entity> Entities { get; private set; } = new();
        /// <summary>
        /// Bool representing if the sensor is currently active. 
        /// The sensor is active if it has received a message in less time 
        /// than the time set by <see cref="SecondsUntilIdle"/>.
        /// </summary>
        public bool IsActive { get; private set; } = false;
        /// <summary>
        /// Radius of the field of view of the sensor. SensMax TAC-B sensors have a 10m radius.
        /// </summary>
        public float fieldOfView = 100;

        // Private
        private float IdleTimer = 0f;
        private Session CurrentSession = new(false);

        /// <summary>
        /// Processes the received message data. 
        /// Updates existing dictionary of <see cref="Entity"/> 
        /// entities to the new entities received in the message.
        /// Also reset the idle countdown to maintain active session.
        /// </summary>
        /// <param name="payload"><see cref="Payload"/> from the client; holds data from the sensor.</param>
        public void HandleMessage(Payload payload) {
            IdleTimer = SecondsUntilIdle;
            var temp = new Dictionary<long, Entity>();
            foreach (Entity e in payload.Entities.Values.ToList())
            {
                temp.Add(e.Id, e);
            }
            Entities = temp;
            MessageReceivedEvent.Invoke();
        }

        /// <summary>
        /// Checks if session needs updating and updates when needed.
        /// </summary>
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
        /// Updates the current and last sessions at the session end of a session.
        /// </summary>
        private void UpdateSession() {
            IsActive = !IsActive;
            CurrentSession.SessionLength = CurrentSessionLength;
            LastSession = CurrentSession;
            CurrentSession = new Session(IsActive);
            CurrentSessionLength = 0f;
            StatusChangedEvent.Invoke();
        }

        #nullable enable
        /// <summary>
        /// Gets the closest entity to the sensor.
        /// </summary>
        /// <returns><see cref="Entity"/> the entity closest to the sensor or null no entities are present. </returns>
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
        /// Gets the entity with the given id.
        /// </summary>
        /// <param name="id">The id of the entity</param>
        /// <returns><see cref="Entity"/> the entity or null if not found.</returns>
        public Entity? GetEntity(long id) { 
            return Entities.ContainsKey(id) ? Entities[id] : null; 
        }
        #nullable disable

        /// <summary>
        /// Returns the time since last active session.
        /// </summary>
        /// <returns><see cref="float"/> number of seconds since last active session.</returns>
        public float GetTimeSinceLastActivePeriod()
        {
            return CurrentSession.ActiveSession ? 0 : CurrentSessionLength;
        }
    }
}
