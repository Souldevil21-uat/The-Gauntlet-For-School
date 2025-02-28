using UnityEngine;
using System; // ✅ Needed for Action events

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    // ✅ Event for when this object dies
    public event Action OnDeath;

    void Start()
    {
        ResetHealth();
    }

    // ✅ New function to reset health (useful for respawning mechanics)
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        Debug.Log(gameObject.name + " 🔄 Health reset to " + maxHealth);
    }

    // ✅ TakeDamage with optional damage source
    public void TakeDamage(float damage, GameObject damageSource = null)
    {
        if (currentHealth <= 0)
        {
            Debug.LogWarning(gameObject.name + " ❌ Already dead, ignoring damage.");
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // ✅ Prevents negative health
        Debug.Log(gameObject.name + " took " + damage + " damage! Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " 💀 has been destroyed!");

        // ✅ Trigger any death-related events before destruction
        OnDeath?.Invoke();

        // ✅ Optional: Replace with a deactivation system instead of immediate destruction
        Destroy(gameObject);
    }
}


