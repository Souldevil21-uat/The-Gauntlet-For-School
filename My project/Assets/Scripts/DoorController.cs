using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float openHeight = 3f;  // How high the door moves when opening
    public float openSpeed = 2f;   // Speed of opening and closing
    public float closeDelay = 3f;  // Time before the door closes

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isMoving = false;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isMoving && (other.CompareTag("Player") || other.CompareTag("AI")))
        {
            StartCoroutine(OpenDoor());
        }
    }

    IEnumerator OpenDoor()
    {
        isMoving = true;
        yield return MoveDoor(openPosition);
        yield return new WaitForSeconds(closeDelay);
        yield return MoveDoor(closedPosition);
        isMoving = false;
    }

    IEnumerator MoveDoor(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, openSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}

