using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : Controller
{
    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;
    public float projectileDamage = 10f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;
    private GameObject activeProjectile = null;
    private bool canShoot = true;

    [Header("Health & Lives")]
    public int maxHealth = 100;  // Maximum health per life
    private int currentHealth;   // Tracks health
    public int lives = 3;        // Number of lives

    private GameOverManager gameOverManager;
    private GameManager gameManager;

    [Header("Movement Tracking")]
    public bool isMoving { get; private set; }
    public bool isShooting { get; private set; }

    void Start()
    {
        gameOverManager = FindObjectOfType<GameOverManager>();
        gameManager = FindObjectOfType<GameManager>();
        currentHealth = maxHealth;  // Start with full health
    }

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
        if (!canShoot) return;

        if (Input.GetKeyDown(KeyCode.Space) && activeProjectile == null && Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + fireRate;
        }
    }

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
            activeProjectile = newProjectile;
            projectileScript.Initialize(gameObject, projectileSpeed, projectileDamage);
        }
        else
        {
            Debug.LogError("ERROR: Projectile script missing!");
        }
    }

    public void OnProjectileDestroyed()
    {
        activeProjectile = null;
    }

    public void EnableShooting(bool enable)
    {
        canShoot = enable;
    }

    public bool IsMakingNoise()
    {
        return isMoving || isShooting;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)  // Player loses a life when health hits 0
        {
            LoseLife();
        }
        UIManager.Instance.UpdateLives(lives); // Update UI
    }


    private void LoseLife()
    {
        lives--;

        if (lives <= 0)
        {
            // If no lives left, trigger Game Over
            gameOverManager.ShowGameOverScreen(GetFinalScore());
            Destroy(gameObject); // Remove player tank
        }
        else
        {
            // Respawn player
            Respawn();
        }
    }

    private void Respawn()
    {
        currentHealth = maxHealth; // Reset health
        gameManager.RespawnPlayer(gameObject); // Move player to new location
    }

    private int GetFinalScore()
    {
        return ScoreManager.Instance.GetScore();
    }
}










