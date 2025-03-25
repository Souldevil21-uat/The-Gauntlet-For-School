using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 20f;     // Speed of the projectile
    public float damage = 10f;    // Damage dealt upon impact
    public float lifespan = 2f;   // Time before the projectile is destroyed

    private Rigidbody rb;         // Rigidbody component for movement
    private GameObject owner;     // The shooter that fired this projectile
    private PlayerController ownerController;
    private bool hasHit = false;  // Prevents multiple hits
    public AudioClip hitClip;     // Assign in Inspector

    /// <summary>
    /// Initializes the projectile with speed, damage, and owner.
    /// </summary>
    public void Initialize(GameObject shooter, float projectileSpeed, float projectileDamage)
    {
        owner = shooter;
        speed = projectileSpeed;
        damage = projectileDamage;
        rb = GetComponent<Rigidbody>();

        if (shooter != null)
        {
            ownerController = shooter.GetComponent<PlayerController>();
        }

        if (rb == null)
        {
            Debug.LogError("ERROR: Rigidbody missing on projectile!");
            Destroy(gameObject);
            return;
        }

        // Set projectile velocity to move forward
        rb.linearVelocity = transform.forward * speed; // 🛠️ Fix: should be velocity not linearVelocity

        // Prevent self-collision
        Collider projectileCollider = GetComponent<Collider>();
        Collider ownerCollider = owner?.GetComponent<Collider>();
        if (projectileCollider != null && ownerCollider != null)
        {
            Physics.IgnoreCollision(projectileCollider, ownerCollider);
        }

        // Destroy projectile after its lifespan
        Invoke(nameof(DestroyProjectile), lifespan);
    }

    /// <summary>
    /// Handles collision with other objects.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        if (other.gameObject == owner) return;

        Health targetHealth = other.GetComponent<Health>();
        if (targetHealth != null)
        {
            // ✅ FIX: Pass the owner to credit the kill
            targetHealth.TakeDamage(damage, owner);
        }

        AudioManager.Instance?.PlaySFX(hitClip);

        hasHit = true;
        DestroyProjectile();
    }

    /// <summary>
    /// Handles projectile destruction.
    /// </summary>
    private void DestroyProjectile()
    {
        if (ownerController != null)
        {
            ownerController.OnProjectileDestroyed();
        }
        Destroy(gameObject);
    }
}















