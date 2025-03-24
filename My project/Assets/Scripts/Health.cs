using UnityEngine;
using System;
using System.Collections;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public AudioClip deathClip;

    // Events for health changes and death
    public event Action<float> OnHealthChanged;
    public event Action OnDeath;

    void Start()
    {
        //AudioManager.Instance.SetSFXVolume(0.3f);
        ResetHealth();

    }

    // Resets health to maximum (useful for respawning mechanics)
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth); // Notify UI or effects
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
        OnHealthChanged?.Invoke(currentHealth); // Notify UI or effects

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Handles object destruction when health reaches zero
    private void Die()
    {
        Debug.Log(gameObject.name + " has died!");

        AudioManager.Instance.PlaySFX(deathClip);
        OnDeath?.Invoke(); // Fire death event

        if (gameObject.CompareTag("Player"))
        {
            PlayerController player = GetComponent<PlayerController>();
            if (player != null && GameManager.Instance != null)
            {
                GameManager.Instance.PlayerDied(player.playerNumber);
            }
        }

        // Delay destruction so the SFX has time to play
        Destroy(gameObject, 0.3f); // 300 ms delay
    }


    private IEnumerator DestroyAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }



}




