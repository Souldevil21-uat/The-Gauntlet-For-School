using UnityEngine;
using System; // Needed for Action events

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    // Event triggered when the object dies
    public event Action OnDeath;

    void Start()
    {
        ResetHealth();
    }

    // Resets health to maximum (useful for respawning mechanics)
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    // Handles taking damage, with an optional damage source
    public void TakeDamage(float damage, GameObject damageSource = null)
    {
        if (currentHealth <= 0)
        {
            return; // Prevents applying damage to an already destroyed object
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Ensures health doesn't go negative

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Handles object destruction when health reaches zero
    private void Die()
    {
        // Trigger any death-related events before destruction
        OnDeath?.Invoke();

        // Optional: Replace with a deactivation system instead of immediate destruction
        Destroy(gameObject);
    }
}



