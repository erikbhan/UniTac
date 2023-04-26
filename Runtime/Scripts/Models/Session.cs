using System;

namespace UniTac.Models
{
    /// <summary>
    /// Class that represents data about a sensor session.
    /// </summary>
    public class Session
    {
        /// <summary>
        /// Start time of the session.
        /// </summary>
        public DateTime Start { get; } = DateTime.Now;
        /// <summary>
        /// Duration of the session. May not be set if session is not complete.
        /// </summary>
        public float SessionLength { get; set; }
        /// <summary>
        /// <see cref="bool"/> representing if the session was active or idle. 
        /// Active sessions have pariticipants detected by the sensor.
        /// </summary>
        public bool ActiveSession { get; }

        /// <summary>
        /// Initializes a new instance of the Session class.
        /// </summary>
        /// <param name="activeSession"><see cref="bool"/> representing if the session is active or idle</param>
        public Session(bool activeSession)
        {
            this.ActiveSession = activeSession;
        }
    }
}
