using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    public Transform target; // The player (set this in Unity Inspector)
    public Vector3 offset = new Vector3(0, 10, -10); // Default position behind the player
    public float smoothSpeed = 5f; // How smoothly the camera follows
    public bool followRotation = false; // Toggle to follow player's rotation

    private bool targetMissingWarningShown = false; // Prevents spamming warnings

    private void LateUpdate()
    {
        if (target == null)
        {
            if (!targetMissingWarningShown)
            {
                Debug.LogWarning("CameraFollow: No target assigned!");
                targetMissingWarningShown = true;
            }
            return;
        }
        else
        {
            targetMissingWarningShown = false; // Reset warning flag if target is found
        }

        // Calculate the desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera to the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Make the camera look at the player
        if (followRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, smoothSpeed * Time.deltaTime);
        }
        else
        {
            transform.LookAt(target);
        }
    }

    // Allows dynamically changing the target at runtime
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        targetMissingWarningShown = false; // Reset warning flag
    }

    // Allows adjusting the camera offset dynamically
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    // Enables or disables rotation following
    public void ToggleFollowRotation(bool enable)
    {
        followRotation = enable;
    }
}


