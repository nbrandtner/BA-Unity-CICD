using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class Reflection : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private LineRenderer lineRenderer;
    private LineRenderer splitLR1;
    private LineRenderer splitLR2;
    [SerializeField] private int maxReflections = 15;
    [SerializeField] private float maxDistance = 100f;

    [Header("Splitting Settings")]
    [SerializeField] private float splitAngle = 30f;

    [Tooltip("How many times a single beam is allowed to split (e.g., 2 = main beam splits once, split beam can split once too)")]
    [SerializeField] private int maxSplitsPerBeam = 1;

    [HideInInspector] public int splitCount = 0; // how many times this specific beam has already split

    public UnityEvent onTargetHit;

    private void ClearSplitBeams()
    {
        // Recursively destroy any existing child beams
        if (splitLR1 != null)
        {
            Reflection child = splitLR1.GetComponent<Reflection>();
            if (child != null) child.ClearSplitBeams(); // clear its children too
            Destroy(splitLR1.gameObject);
            splitLR1 = null;
        }

        if (splitLR2 != null)
        {
            Reflection child = splitLR2.GetComponent<Reflection>();
            if (child != null) child.ClearSplitBeams(); // clear its children too
            Destroy(splitLR2.gameObject);
            splitLR2 = null;
        }
    }


    public void CastLaser(Vector3 origin, Vector3 direction, int depth = 0)
    {
        // Always clear existing split beams for this laser
        ClearSplitBeams();

        // Reset split count only for the root beam
        if (depth == 0)
        {
            splitCount = 0;
        }

        List<Vector3> points = new List<Vector3>();
        points.Add(origin);
        GameObject lastHit = null;


        for (int i = 0; i < maxReflections; i++)
        {
            Ray ray = new Ray(origin, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            {
                if (hit.collider.gameObject == lastHit || hit.collider.gameObject == this.gameObject)
                    break;

                lastHit = hit.collider.gameObject;
                points.Add(hit.point);

                // --- TARGET ---
                if (hit.collider.CompareTag("Target"))
                {
                    onTargetHit?.Invoke();
                    break;
                }

                // --- SPLITTER ---
                if (hit.collider.CompareTag("Splitter") && splitCount < maxSplitsPerBeam)
                {
                    splitCount++; // this beam has split one more time

                    Vector3 splitOrigin = hit.point + direction.normalized * 0.05f;
                    Vector3 baseDir = direction.normalized;

                    Quaternion rotLeft = Quaternion.AngleAxis(-splitAngle, Vector3.up);
                    Quaternion rotRight = Quaternion.AngleAxis(splitAngle, Vector3.up);

                    Vector3 dirLeft = (rotLeft * baseDir).normalized;
                    Vector3 dirRight = (rotRight * baseDir).normalized;

                    CreateSplitBeam(splitOrigin + dirLeft * 0.05f, dirLeft, depth + 1, 0);
                    CreateSplitBeam(splitOrigin + dirRight * 0.05f, dirRight, depth + 1, 1);
                    break;
                }

                // --- MIRROR ---
                if (hit.collider.CompareTag("Mirror"))
                {
                    origin = hit.point + hit.normal * 0.01f;
                    direction = Vector3.Reflect(direction, hit.normal);
                    continue;
                }

                // --- LASER BUTTON ---
                if (hit.collider.CompareTag("LaserButton"))
                {
                    LaserButton button = hit.collider.GetComponent<LaserButton>();
                    if (button != null) button.OnLaserHit();
                    break;
                }

                // --- OTHER COLLISION ---
                break;
            }
            else
            {
                points.Add(origin + direction * maxDistance);
                break;
            }
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    private void CreateSplitBeam(Vector3 origin, Vector3 direction, int depth, int beamIndex)
    {
        LineRenderer splitLR;

        if (beamIndex == 0 && splitLR1 == null)
        {
            GameObject beamObj = new GameObject("SplitBeam1");
            splitLR1 = beamObj.AddComponent<LineRenderer>();
            CopyLineSettings(splitLR1);
        }
        if (beamIndex == 1 && splitLR2 == null)
        {
            GameObject beamObj = new GameObject("SplitBeam2");
            splitLR2 = beamObj.AddComponent<LineRenderer>();
            CopyLineSettings(splitLR2);
        }

        splitLR = (beamIndex == 0) ? splitLR1 : splitLR2;

        // Add Reflection to split beams so they also reflect and can split
        Reflection splitReflection = splitLR.GetComponent<Reflection>();
        if (splitReflection == null) splitReflection = splitLR.gameObject.AddComponent<Reflection>();

        splitReflection.lineRenderer = splitLR;
        splitReflection.maxReflections = maxReflections;
        splitReflection.maxDistance = maxDistance;
        splitReflection.splitAngle = splitAngle;
        splitReflection.onTargetHit = onTargetHit;

        splitReflection.splitCount = splitCount;
        splitReflection.maxSplitsPerBeam = maxSplitsPerBeam;

        splitReflection.CastLaser(origin, direction, depth);
    }

    private void CopyLineSettings(LineRenderer lr)
    {
        lr.startWidth = lineRenderer.startWidth;
        lr.endWidth = lineRenderer.endWidth;
        lr.widthMultiplier = lineRenderer.widthMultiplier;
        lr.sortingLayerID = lineRenderer.sortingLayerID;
        lr.sortingOrder = lineRenderer.sortingOrder;

        lr.useWorldSpace = true;
        lr.alignment = lineRenderer.alignment;
        lr.textureMode = lineRenderer.textureMode;
        lr.textureMode = LineTextureMode.Stretch;

        // Copy material but also reset HDR brightness manually
        lr.material = lineRenderer.sharedMaterial;
        if (lr.material.HasProperty("_BaseColor"))
        {
            Color c = lr.material.GetColor("_BaseColor");
            lr.material.SetColor("_BaseColor", c.linear * 1.2f);
        }
    }
}
