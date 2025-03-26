using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage; // Assigned in the prefab
    private Coroutine currentLerp;

    public Transform followTarget;
    public Vector3 offset = new Vector3(0, 2f, 0); // Height above tank

    // Sets the transform this health bar should follow
    public void SetTarget(Transform target)
    {
        followTarget = target;
    }

    // Called whenever health changes
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        float targetFill = currentHealth / maxHealth;

        if (currentLerp != null)
        {
            StopCoroutine(currentLerp);
        }

        currentLerp = StartCoroutine(SmoothFill(targetFill));
    }

    // Smooth transition of health bar fill
    private IEnumerator SmoothFill(float target)
    {
        float duration = 0.4f;
        float start = fillImage.fillAmount;
        float time = 0f;

        while (time < duration)
        {
            fillImage.fillAmount = Mathf.Lerp(start, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        fillImage.fillAmount = target;
    }

    // Follow the assigned target and face the camera
    private void LateUpdate()
    {
        if (followTarget != null)
        {
            transform.position = followTarget.position + offset;
            transform.forward = Camera.main.transform.forward;
        }
    }
}

