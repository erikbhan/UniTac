using System;

/// <summary>
/// Class that represents data about a sensor session.
/// </summary>
public class Session
{
    public DateTime Start { get; } = DateTime.Now;
    public float SessionLength { get; set; }
    /// <summary>
    /// <see cref="bool"/> representing if the session was active or idle. 
    /// Active sessions have pariticipants detected by the sensor.
    /// </summary>
    public bool ActiveSession { get; }

    public Session(bool activeSession)
    {
        this.ActiveSession = activeSession;
    }
}
