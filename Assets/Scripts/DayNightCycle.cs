using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time in Game")]
    public float realMinutesPerGameHour = 1f;
    private float timeMultiplier;
    private float timeOfDay;

    [Header("Setting Light")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    private void Start()
    {
        float totalRealSecondsPerDay = realMinutesPerGameHour * 24f * 60f;
        timeMultiplier = 1f / totalRealSecondsPerDay;
    }

    private void Update()
    {
        timeOfDay += Time.deltaTime * timeMultiplier;
        if (timeOfDay >= 1f) timeOfDay = 0f;

        UpdateSun();
    }

    void UpdateSun()
    {
        if (sun != null)
        {
            float sunAngle = timeOfDay * 360f - 90f;
            sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);
            sun.intensity = sunIntensity.Evaluate(timeOfDay);
            sun.color = sunColor.Evaluate(timeOfDay);
        }
    }
}
