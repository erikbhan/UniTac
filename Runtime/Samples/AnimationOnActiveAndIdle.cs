using UnityEngine;


public class AnimationOnActiveAndIdle : MonoBehaviour
{
    public GameObject ActiveAnimation;
    public GameObject IdleAnimation;
    public GameObject IdleAnimation2;
    public float IdleTimeUntilSecondAnimation = 300f;
    private Sensor sensor;
    private bool IsActive = false;


    void Start()
    {
        sensor = gameObject.GetComponent<Sensor>();
        if (!sensor) Debug.LogError("Sensor script not found");
        IsActive = sensor.IsActive;

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

    void Update()
    {
        if (sensor.IsActive != IsActive)
        {
            IsActive = sensor.IsActive;
            ActiveAnimation.SetActive(IsActive);
            IdleAnimation.SetActive(!IsActive);
            IdleAnimation2.SetActive(false);
        }
        if (sensor.GetTimeSinceLastActivePeriod() > IdleTimeUntilSecondAnimation)
        {
            IdleAnimation.SetActive(false); 
            IdleAnimation2.SetActive(true);
        }
    }
}

