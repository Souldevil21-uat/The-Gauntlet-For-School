using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float openHeight = 3f;  // How high the door moves when opening
    public float openSpeed = 2f;   // Speed of opening and closing
    public float closeDelay = 3f;  // How long before the door closes
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpening = false;
    private bool isClosing = false;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AI"))
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        if (!isOpening)
        {
            isOpening = true;
            isClosing = false;
            StopAllCoroutines();
            StartCoroutine(MoveDoor(openPosition));
            Invoke(nameof(CloseDoor), closeDelay);
        }
    }

    void CloseDoor()
    {
        if (!isClosing)
        {
            isClosing = true;
            isOpening = false;
            StopAllCoroutines();
            StartCoroutine(MoveDoor(closedPosition));
        }
    }

    System.Collections.IEnumerator MoveDoor(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * openSpeed);
            yield return null;
        }
    }
}

