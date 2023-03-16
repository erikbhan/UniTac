using System;
class Session
{
    public DateTime start { get; } = DateTime.Now;
    public float sessionLength { get; set; }
    public bool activeSession { get; }

    public Session(bool activeSession)
    {
        this.activeSession = activeSession;
    }
}
