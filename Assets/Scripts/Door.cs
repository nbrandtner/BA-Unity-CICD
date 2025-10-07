using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform doorObject;
    [SerializeField] private Vector3 openPositionOffset = new Vector3(0, -10f, 0);
    [SerializeField] private float speed = 2f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen = false;

    private void Start()
    {
        closedPos = doorObject.position;
        openPos = closedPos + openPositionOffset;
    }

    public void SetOpen(bool open)
    {
        isOpen = open;
    }

    private void Update()
    {
        Vector3 target = isOpen ? openPos : closedPos;
        doorObject.position = Vector3.MoveTowards(doorObject.position, target, speed * Time.deltaTime);
    }
}
