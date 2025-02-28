using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 20f;
    public float damage = 10f;
    public float lifespan = 2f;
    private Rigidbody rb;
    private GameObject owner;
    private PlayerController ownerController;
    private bool hasHit = false; // ✅ Prevent multiple hits

    public void Initialize(GameObject shooter, float projectileSpeed, float projectileDamage)
    {
        owner = shooter;
        speed = projectileSpeed;
        damage = projectileDamage;
        rb = GetComponent<Rigidbody>();

        ownerController = shooter.GetComponent<PlayerController>();

        if (rb == null)
        {
            Debug.LogError("❌ ERROR: Rigidbody missing on projectile!");
            Destroy(gameObject); // ✅ Destroy projectile if it has no Rigidbody
            return;
        }

        // ✅ Ensure the projectile always moves forward
        rb.linearVelocity = transform.forward * speed;
        Debug.Log("✅ Projectile launched with Speed: " + speed);

        // ✅ Ignore collision with the owner to prevent self-hits
        Collider projectileCollider = GetComponent<Collider>();
        Collider ownerCollider = owner?.GetComponent<Collider>();
        if (projectileCollider != null && ownerCollider != null)
        {
            Physics.IgnoreCollision(projectileCollider, ownerCollider);
        }

        // ✅ Destroy after lifespan to avoid clutter
        Destroy(gameObject, lifespan);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return; // ✅ Prevent multiple hits

        Debug.Log("🚨 Projectile collided with: " + other.gameObject.name);

        if (other.gameObject == owner)
        {
            Debug.Log("❌ Projectile hit its owner, ignoring...");
            return; // ✅ Ignore self-collisions
        }

        Health targetHealth = other.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
            Debug.Log("💥 " + other.gameObject.name + " took " + damage + " damage! Current Health: " + targetHealth.currentHealth);
        }

        hasHit = true; // ✅ Mark the projectile as having hit something
        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        Debug.Log("🗑 Projectile destroyed!");
        if (ownerController != null)
        {
            ownerController.OnProjectileDestroyed(); // ✅ Notify the player they can shoot again
        }
        Destroy(gameObject);
    }
}












