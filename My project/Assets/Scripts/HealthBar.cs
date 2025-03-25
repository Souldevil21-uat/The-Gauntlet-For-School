using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    public Transform followTarget;
    public Vector3 offset = new Vector3(0, 2f, 0); // Adjust as needed

    public void SetTarget(Transform target)
    {
        followTarget = target;
    }

    public void UpdateHealthBar(float current, float max)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = current / max;
        }
    }

    private void LateUpdate()
    {
        if (followTarget != null)
        {
            transform.position = followTarget.position + offset;
            transform.forward = Camera.main.transform.forward; // Face the camera
        }
    }
}
