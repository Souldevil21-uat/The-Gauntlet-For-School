using UnityEngine;

public class PlayerController : Controller
{
    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;
    public float projectileDamage = 10f;
    public float fireRate = 0.5f; // ✅ Prevents shooting too fast
    private float nextFireTime = 0f;
    private GameObject activeProjectile = null;
    private bool canShoot = true; // ✅ Easily toggle shooting

    [Header("Movement Tracking")]
    public bool isMoving { get; private set; }
    public bool isShooting { get; private set; }

    protected override void Update()
    {
        base.Update();
        HandleMovement();
        HandleShooting();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical");
        float rotateInput = Input.GetAxis("Horizontal");

        isMoving = moveInput != 0 || rotateInput != 0;

        if (pawn != null)
        {
            pawn.Move(moveInput);
            pawn.Rotate(rotateInput);
        }
    }

    private void HandleShooting()
    {
        if (!canShoot) return; // ✅ Prevents shooting when disabled

        if (Input.GetKeyDown(KeyCode.Space) && activeProjectile == null && Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + fireRate; // ✅ Ensures fire delay is respected
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("❌ ERROR: No projectile prefab assigned!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError("❌ ERROR: No firePoint assigned!");
            return;
        }

        GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectileScript = newProjectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            activeProjectile = newProjectile; // ✅ Tracks the active projectile

            // ✅ Ensure the projectile gets initialized properly
            projectileScript.Initialize(gameObject, projectileSpeed, projectileDamage);
            Debug.Log("🎯 Fired projectile with Speed: " + projectileSpeed);
        }
        else
        {
            Debug.LogError("❌ ERROR: Projectile script missing!");
        }
    }

    public void OnProjectileDestroyed()
    {
        Debug.Log("🔄 Projectile destroyed. Player can fire again.");
        activeProjectile = null; // ✅ Ensures player can shoot again
    }

    public void EnableShooting(bool enable)
    {
        canShoot = enable;
        Debug.Log("🔫 Shooting " + (enable ? "ENABLED" : "DISABLED"));
    }

    private void ResetShootingNoise()
    {
        isShooting = false;
    }

    public bool IsMakingNoise()
    {
        return isMoving || isShooting;
    }

}








