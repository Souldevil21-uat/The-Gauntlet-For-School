using UnityEngine;

public class PlayerController : Controller
{
    [Header("Shooting Settings")]
    public GameObject projectilePrefab; // Prefab for the projectile
    public Transform firePoint;         // Fire point location
    public float projectileSpeed = 20f; // Speed of the projectile
    public float projectileDamage = 10f; // Damage per shot
    public float fireRate = 0.5f;       // Cooldown time between shots
    private float nextFireTime = 0f;    // Time when the next shot is allowed
    private GameObject activeProjectile = null; // Tracks the currently active projectile
    private bool canShoot = true;       // Flag to enable/disable shooting

    [Header("Movement Tracking")]
    public bool isMoving { get; private set; }  // Tracks if the player is moving
    public bool isShooting { get; private set; } // Tracks if the player is shooting

    protected override void Update()
    {
        base.Update();
        HandleMovement();
        HandleShooting();
    }
    /// Handles player movement input and updates pawn movement.
    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical"); // Forward/backward movement
        float rotateInput = Input.GetAxis("Horizontal"); // Left/right rotation

        isMoving = moveInput != 0 || rotateInput != 0; // Updates movement status

        if (pawn != null)
        {
            pawn.Move(moveInput);
            pawn.Rotate(rotateInput);
        }
    }
    /// Handles shooting input and triggers projectile firing.
    private void HandleShooting()
    {
        if (!canShoot) return; // Prevents shooting if disabled

        if (Input.GetKeyDown(KeyCode.Space) && activeProjectile == null && Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + fireRate; // Enforces fire rate cooldown
        }
    }

    /// Instantiates and fires a projectile.
    private void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("ERROR: Missing projectile prefab or fire point!");
            return;
        }

        GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectileScript = newProjectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            activeProjectile = newProjectile; // Tracks active projectile
            projectileScript.Initialize(gameObject, projectileSpeed, projectileDamage);
        }
        else
        {
            Debug.LogError("ERROR: Projectile script missing!");
        }
    }

    /// Called when the active projectile is destroyed, allowing the player to fire again.
    public void OnProjectileDestroyed()
    {
        activeProjectile = null;
    }

    /// Enables or disables the ability to shoot.
    public void EnableShooting(bool enable)
    {
        canShoot = enable;
    }

    /// Checks if the player is making noise (moving or shooting).
    public bool IsMakingNoise()
    {
        return isMoving || isShooting;
    }
}









