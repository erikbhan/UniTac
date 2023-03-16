using System;
public class Session
{
    public DateTime Start { get; } = DateTime.Now;
    public float SessionLength { get; set; }
    public bool ActiveSession { get; }

    public Session(bool activeSession)
    {
        this.ActiveSession = activeSession;
    }
}
