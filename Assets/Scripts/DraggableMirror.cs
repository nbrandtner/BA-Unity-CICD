using System.Collections;
using UnityEngine;

public class DraggableMirror : MonoBehaviour
{
    private Camera mainCam;
    private bool isDragging = false;
    private Vector3 offset;
    private Plane dragPlane;
    private float rotationStep = 45f;
    private KeyCode rotateKey = KeyCode.R;
    [SerializeField] private Coroutine shakeRoutine;
    [SerializeField] private Color errorColor = Color.red;
    [SerializeField] private float flashDuration = 0.15f;

    private Material mirrorMat;
    private Color originalColor;
    private Coroutine flashRoutine;
    private bool levelFinished = false;
    public static DraggableMirror activeMirror;

    void Start()
    {
        mainCam = Camera.main;
        dragPlane = new Plane(Vector3.up, transform.position);

        mirrorMat = GetComponent<Renderer>().material;
        originalColor = mirrorMat.color;
        levelFinished = false;
    }
    public void DisableMirrors(bool isFinished)
    {
        levelFinished = isFinished;
    }

    void Update()
    {
        if (levelFinished) return; // stop rotation input

        if (isDragging && Input.GetKeyDown(rotateKey))
        {
            RotateMirror();
        }
    }

    // Called by UI button
    public void RotateMirror()
    {
        if (levelFinished) return;

        Quaternion originalRotation = transform.rotation;
        transform.Rotate(0f, rotationStep, 0f);

        if (IsOverlappingAt(transform.position))
        {
            transform.rotation = originalRotation;

            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(FlashRed());
        }
    }


    void OnMouseDown()
    {

        if (levelFinished) return; // stop dragging completely
        activeMirror = this;

        isDragging = true;

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            offset = transform.position - hitPoint;
        }
    }

    void OnMouseDrag()
    {
        if (levelFinished) return; // stop dragging completely
        if (!isDragging) return;

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 targetPos = ray.GetPoint(enter) + offset;

            // Only move if not overlapping with other mirrors
            if (!IsOverlappingAt(targetPos))
            {
                transform.position = targetPos;
            }
        }
    }

    void OnMouseUp()
    {
        if (levelFinished) return; // stop dragging completely
        isDragging = false;
    }


    private bool IsOverlappingAt(Vector3 testPos) {
        Collider col = GetComponent<Collider>();
        Vector3 size = col.bounds.size * 0.45f; // Shrink box slightly

        Collider[] hits = Physics.OverlapBox(testPos, size, transform.rotation);
        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject && hit.CompareTag("Mirror"))
                return true;
        }
        return false;
    }
    private IEnumerator FlashRed()
    {
        mirrorMat.color = errorColor;
        yield return new WaitForSeconds(flashDuration);
        mirrorMat.color = originalColor;
    }
}
