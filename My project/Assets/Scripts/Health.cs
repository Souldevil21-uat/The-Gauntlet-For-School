using UnityEngine;
using System;
using System.Collections;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public AudioClip deathClip;
    public GameObject healthBarPrefab;
    private HealthBar healthBarInstance;

    // Events for health changes and death
    public event Action<float> OnHealthChanged;
    public event Action OnDeath;

    // Points awarded for killing this unit
    public int pointsForKill = 100;

    void Start()
    {
        ResetHealth();
    }

    // Resets health to maximum (useful for respawning mechanics)
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth); // Notify UI or effects
        if (healthBarPrefab != null && healthBarInstance == null)
        {
            GameObject bar = Instantiate(healthBarPrefab);
            healthBarInstance = bar.GetComponent<HealthBar>();
            healthBarInstance.SetTarget(transform);
        }

        if (healthBarInstance != null)
        {
            healthBarInstance.UpdateHealthBar(currentHealth, maxHealth);
        }


    }

    // Handles taking damage, with an optional damage source
    public void TakeDamage(float damage, GameObject damageSource = null)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            int killerPlayerNumber = GetPlayerNumberFromSource(damageSource);
            Die(killerPlayerNumber);
        }

        if (healthBarInstance != null)
        {
            healthBarInstance.UpdateHealthBar(currentHealth, maxHealth);
        }

    }

    // Extract player number from the damage source GameObject
    private int GetPlayerNumberFromSource(GameObject source)
    {
        if (source == null) return -1;

        PlayerController controller = source.GetComponent<PlayerController>();
        if (controller != null)
        {
            return controller.playerNumber;
        }

        return -1;
    }

    // Handles object destruction and reward logic
    private void Die(int killerPlayerNumber)
    {
        Debug.Log(gameObject.name + " has died!");

        if (killerPlayerNumber > 0)
        {
            Debug.Log($"[Health] {gameObject.name} was killed by player {killerPlayerNumber}");

            int updatedScore = ScoreManager.Instance.AddScore(killerPlayerNumber, pointsForKill);
            UIManager_GameScene.Instance.UpdateScore(killerPlayerNumber, updatedScore);
        }

        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }


        AudioManager.Instance?.PlaySFX(deathClip);
        OnDeath?.Invoke();

        if (gameObject.CompareTag("Player"))
        {
            PlayerController player = GetComponent<PlayerController>();
            if (player != null && GameManager.Instance != null)
            {
                GameManager.Instance.PlayerDied(player.playerNumber);
            }
        }

        Destroy(gameObject, 0.3f); // Wait for SFX
    }

    private IEnumerator DestroyAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}





