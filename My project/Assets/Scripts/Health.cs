using UnityEngine;
using UnityEngine.UI;
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
        OnHealthChanged?.Invoke(currentHealth);

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

        if (healthBarInstance != null)
        {
            healthBarInstance.UpdateHealthBar(currentHealth, maxHealth); // Smooth animation
        }

        if (currentHealth <= 0)
        {
            int killerPlayerNumber = GetPlayerNumberFromSource(damageSource);
            Die(killerPlayerNumber);
        }
    }

    private int GetPlayerNumberFromSource(GameObject source)
    {
        if (source == null) return -1;

        PlayerController controller = source.GetComponent<PlayerController>();
        return controller != null ? controller.playerNumber : -1;
    }

    private void Die(int killerPlayerNumber)
    {
        Debug.Log(gameObject.name + " has died!");

        if (killerPlayerNumber > 0)
        {
            Debug.Log($"[Health] {gameObject.name} was killed by player {killerPlayerNumber}");

            int updatedScore = ScoreManager.Instance.AddScore(killerPlayerNumber, pointsForKill);
            UIManager_GameScene.Instance.UpdateScore(killerPlayerNumber, updatedScore);
        }

        // Destroy the health bar UI GameObject
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance.gameObject);
        }

        AudioManager.Instance?.PlaySFX(deathClip);
        OnDeath?.Invoke();

        if (CompareTag("Player"))
        {
            PlayerController player = GetComponent<PlayerController>();
            if (player != null && GameManager.Instance != null)
            {
                GameManager.Instance.PlayerDied(player.playerNumber);
            }
        }

        if (CompareTag("AI"))
        {
            GameManager.Instance.RespawnAIWithDelay(3f);
        }

        Destroy(gameObject, 0.3f); // Delayed destruction for SFX
    }

    private IEnumerator DestroyAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}






