using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour
{
    // Define types of powerups
    public enum PowerupType { SpeedBoost, HealthPickup, DamageBoost, AddScore }
    public PowerupType type;
    public float duration = 5f; // Duration for temporary effects like speed or damage boost

    private Transform spawnPoint; // Reference to the spawn location for respawning
    private float respawnTime; // Time until the powerup respawns
    private Collider powerupCollider; // Cached reference to collider for enabling/disabling
    public AudioClip pickupClip; // Sound played when powerup is picked up

    private void Start()
    {
        powerupCollider = GetComponent<Collider>(); // Get and store the collider component
        SetColorByType(); // Set powerup color based on type
    }

    // Changes the material color based on powerup type
    private void SetColorByType()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend == null) return;

        Color color = Color.white;

        switch (type)
        {
            case PowerupType.SpeedBoost:
                color = Color.green;
                break;
            case PowerupType.HealthPickup:
                color = Color.red;
                break;
            case PowerupType.DamageBoost:
                color = Color.yellow;
                break;
        }

        rend.material.color = color;
    }

    // Called by the spawner to track its location and respawn delay
    public void SetRespawn(Transform spawn, float time)
    {
        spawnPoint = spawn;
        respawnTime = time;
    }

    // Detect when a tank touches the powerup
    private void OnTriggerEnter(Collider other)
    {
        TankPawn tank = other.GetComponent<TankPawn>();

        // Ensure only player-controlled tanks can pick up powerups
        if (tank != null)
        {
            PlayerController playerController = tank.GetComponent<PlayerController>();
            if (playerController == null)
            {
                // This tank is not player-controlled (likely AI), ignore
                return;
            }

            ApplyEffect(tank);
            AudioManager.Instance.PlaySFX(pickupClip);
            powerupCollider.enabled = false;
            gameObject.SetActive(false);
            GameManager.Instance?.StartPowerupRespawn(this);
        }
    }


    // Applies the appropriate effect to the tank
    private void ApplyEffect(TankPawn tank)
    {
        switch (type)
        {
            case PowerupType.SpeedBoost:
                StartCoroutine(SpeedBoost(tank));
                break;
            case PowerupType.HealthPickup:
                HealTank(tank);
                break;
            case PowerupType.DamageBoost:
                StartCoroutine(DamageBoost(tank));
                break;
            case PowerupType.AddScore:
                AddScore(tank);
                break;

        }
    }
    private void AddScore(TankPawn tank)
    {
        PlayerController pc = tank.GetComponent<PlayerController>();
        if (pc != null)
        {
            ScoreManager.Instance.AddScore(pc.playerNumber, 100); // Add 100 points
        }
    }

    // Temporarily increases tank movement speed
    private IEnumerator SpeedBoost(TankPawn tank)
    {
        tank.moveSpeed += 5f;
        yield return new WaitForSeconds(duration);
        tank.moveSpeed -= 5f;
    }

    // Heals the tank by a set amount, up to the max health
    private void HealTank(TankPawn tank)
    {
        Health health = tank.GetComponent<Health>();
        if (health != null)
        {
            health.currentHealth = Mathf.Min(health.currentHealth + 20f, health.maxHealth);
        }
    }

    // Temporarily increases projectile damage for the player
    private IEnumerator DamageBoost(TankPawn tank)
    {
        PlayerController playerController = tank.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.projectileDamage += 10f;
            yield return new WaitForSeconds(duration);
            playerController.projectileDamage -= 10f;
        }
    }

    // Reactivate the powerup and re-enable its collider
    public void ResetPowerup()
    {
        powerupCollider.enabled = true;
        gameObject.SetActive(true);
    }
}






