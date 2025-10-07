using UnityEngine;

public class LaserButton : MonoBehaviour
{
    [SerializeField] private Door door;
    private bool isHit = false;

    // Called by Reflection when the laser hits this button
    public void OnLaserHit()
    {
        isHit = true;
    }

    private void LateUpdate()
    {
        // Update door based on whether button was hit this frame
        door?.SetOpen(isHit);

        // Reset for next frame
        isHit = false;
    }
}
