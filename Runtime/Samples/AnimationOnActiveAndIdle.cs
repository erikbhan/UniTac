using UnityEngine;
using UniTac;

/// <summary>
/// Sample monobehaviour that acts as a simple state machine.
/// </summary>
public class AnimationOnActiveAndIdle : MonoBehaviour
{
    /// <summary>
    /// GameObject thats is active if the sensor is detecting entities.
    /// </summary>
    public GameObject ActiveAnimation;
    /// <summary>
    /// GameObject thats is active if the sensor is not detecting entities.
    /// </summary>
    public GameObject IdleAnimation;
    /// <summary>
    /// GameObject thats is active if the sensor has not detected entities for the set time.
    /// </summary>
    public GameObject IdleAnimation2;
    /// <summary>
    /// Number of second before second idle animation is set active.
    /// </summary>
    public float IdleTimeUntilSecondAnimation = 300f;

    private Sensor Sensor;
    private bool IsActive = false;

    /// <summary>
    /// Sets up the sensor and all animation game objects.
    /// </summary>
    void Start()
    {
        Sensor = gameObject.GetComponent<Sensor>();
        if (!Sensor) Debug.LogError("Sensor script not found");
        IsActive = Sensor.IsActive;

        if (!ActiveAnimation) Debug.LogError("ActiveAnimation not set");
        ActiveAnimation = Instantiate(ActiveAnimation, gameObject.transform, false);
        ActiveAnimation.SetActive(IsActive);
        if (!IdleAnimation) Debug.LogError("IdleAnimation not set");
        IdleAnimation = Instantiate(IdleAnimation, gameObject.transform, false);
        IdleAnimation.SetActive(!IsActive);
        if (!IdleAnimation2) Debug.LogError("Second idleAnimation not set");
        IdleAnimation2 = Instantiate(IdleAnimation2, gameObject.transform, false);
        IdleAnimation2.SetActive(false);
    }

    /// <summary>
    /// Updates on state change.
    /// </summary>
    void Update()
    {
        if (Sensor.IsActive != IsActive)
        {
            IsActive = Sensor.IsActive;
            ActiveAnimation.SetActive(IsActive);
            IdleAnimation.SetActive(!IsActive);
            IdleAnimation2.SetActive(false);
        }
        if (Sensor.GetTimeSinceLastActivePeriod() > IdleTimeUntilSecondAnimation)
        {
            IdleAnimation.SetActive(false); 
            IdleAnimation2.SetActive(true);
        }
    }
}

