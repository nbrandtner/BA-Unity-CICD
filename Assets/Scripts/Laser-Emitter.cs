using UnityEngine;

public class LaserEmitter : MonoBehaviour
{
    private Reflection reflection;

    void Start()
    {
        reflection = GetComponent<Reflection>();
    }

    void Update()
    {
        if (reflection != null)
        {
            reflection.CastLaser(transform.position, transform.forward);
        }
    }
}