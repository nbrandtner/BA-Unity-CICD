using UnityEngine;
using UnityEngine.Events;

public class LaserReceiver : MonoBehaviour
{
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    private bool isHit;

    public void OnLaserHit()
    {
        isHit = true;
    }

    private void LateUpdate()
    {
        if (isHit)
        {
            OnActivate?.Invoke();
        }
        else
        {
            OnDeactivate?.Invoke();
        }

        isHit = false;
    }
}
