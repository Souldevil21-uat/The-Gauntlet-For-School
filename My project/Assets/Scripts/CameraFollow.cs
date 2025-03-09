using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The player (set this in Unity Inspector)
    public Vector3 offset = new Vector3(0, 10, -10); // Default position behind the player
    public float smoothSpeed = 5f; // How smoothly the camera follows

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target assigned!");
            return;
        }

        // Calculate the desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera to the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Make the camera look at the player
        transform.LookAt(target);
    }
}

