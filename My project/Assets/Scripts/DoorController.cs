using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float openHeight = 3f;  // How high the door moves when opening
    public float openSpeed = 2f;   // Speed of opening and closing
    public float closeDelay = 3f;  // Time to wait before the door closes again

    private Vector3 closedPosition; // Original position of the door
    private Vector3 openPosition;   // Target position when the door opens
    private bool isMoving = false;  // Prevents overlapping animations

    void Start()
    {
        // Store initial position as closed position
        closedPosition = transform.position;

        // Calculate the open position by adding height to the Y axis
        openPosition = closedPosition + Vector3.up * openHeight;
    }

    void OnTriggerEnter(Collider other)
    {
        // Only trigger for Player or AI and if not already opening/closing
        if (!isMoving && (other.CompareTag("Player") || other.CompareTag("AI")))
        {
            StartCoroutine(OpenDoor());
        }
    }

    // Opens the door, waits, then closes it
    IEnumerator OpenDoor()
    {
        isMoving = true;

        // Animate door up
        yield return MoveDoor(openPosition);

        // Wait before closing
        yield return new WaitForSeconds(closeDelay);

        // Animate door down
        yield return MoveDoor(closedPosition);

        isMoving = false;
    }

    // Moves the door toward the target position over time
    IEnumerator MoveDoor(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, openSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}


