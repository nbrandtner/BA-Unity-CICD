using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    void Start()
    {
        ApplyFrameRateSettings();
    }

    public void ApplyFrameRateSettings()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
}
