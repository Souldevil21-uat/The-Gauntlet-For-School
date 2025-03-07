using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour
{
    public enum PowerupType { SpeedBoost, HealthPickup, DamageBoost }
    public PowerupType type;
    public float duration = 5f; // Only for temporary boosts
    private Transform spawnPoint;
    private float respawnTime;

    public void SetRespawn(Transform spawn, float time)
    {
        spawnPoint = spawn;
        respawnTime = time;
    }

    private void OnTriggerEnter(Collider other)
    {
        TankPawn tank = other.GetComponent<TankPawn>();

        if (tank != null)
        {
            ApplyEffect(tank);
            GameManager.Instance.StartPowerupRespawn(this);
        }
    }

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
        }
        gameObject.SetActive(false); // ✅ Now it's safe to deactivate!
    }

    private IEnumerator SpeedBoost(TankPawn tank)
    {
        tank.moveSpeed += 5f;
        yield return new WaitForSeconds(duration);
        tank.moveSpeed -= 5f;
    }

    private void HealTank(TankPawn tank)
    {
        Health health = tank.GetComponent<Health>();
        if (health != null)
        {
            health.currentHealth = Mathf.Min(health.currentHealth + 20f, health.maxHealth);
        }
    }

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
}




