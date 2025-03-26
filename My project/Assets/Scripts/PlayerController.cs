using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : Controller
{
    [Header("Player Settings")]
    public int playerNumber = 1; // Indicates whether this is Player 1 or Player 2
    public int maxHealth = 100; // Maximum health value for the player
    private int currentHealth; // Tracks current health
    public int lives = 3; // Number of lives the player has
    public AudioClip fireSound; // Sound played when firing

    [Header("Shooting Settings")]
    public GameObject projectilePrefab; // Projectile to instantiate on fire
    public Transform firePoint; // Position from where projectiles are fired
    public float projectileSpeed = 20f; // Speed of the projectile
    public float projectileDamage = 10f; // Damage dealt by the projectile
    public float fireRate = 0.5f; // Delay between shots
    private float nextFireTime = 0f; // Timestamp for when next shot is allowed
    private bool canShoot = true; // Toggle whether the player can shoot

    [Header("Movement Tracking")]
    public bool isMoving { get; private set; } // Tracks movement input
    public bool isShooting { get; private set; } // Tracks shooting input

    private GameObject activeProjectile = null; // Tracks the currently active projectile
    private GameManager gameManager; // Reference to GameManager instance
    private UIManager_GameScene gameUIManager; // Reference to UI manager

    private string moveAxis; // Movement input axis for the player
    private string rotateAxis; // Rotation input axis for the player
    private KeyCode shootKey; // Key assigned to shoot for this player

    protected override void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); // Get reference to GameManager
        FindGameUIManager(); // Locate UI manager in scene
        currentHealth = maxHealth; // Initialize health

        // Determine player number by tag
        if (gameObject.CompareTag("Player 2"))
        {
            playerNumber = 2;
        }
        else
        {
            playerNumber = 1;
        }

        // Assign control scheme based on player number
        if (playerNumber == 1)
        {
            moveAxis = "Vertical"; // Default vertical input
            rotateAxis = "Horizontal"; // Default horizontal input
            shootKey = KeyCode.Space; // Player 1 fires with Space
        }
        else if (playerNumber == 2)
        {
            moveAxis = "P2_Vertical"; // Player 2 custom axis
            rotateAxis = "P2_Horizontal";
            shootKey = KeyCode.RightControl; // Player 2 fires with Right Ctrl
        }

        // Verify pawn is assigned
        if (pawn == null)
        {
            Debug.LogError($"ERROR: Player {playerNumber} does not have an assigned pawn!");
        }
        else
        {
            Debug.Log($"Player {playerNumber} is controlling {pawn.name}");
        }
    }

    protected override void Update()
    {
        base.Update();
        HandleMovement(); // Process player movement input
        HandleShooting(); // Process player shooting input
    }

    private void HandleMovement()
    {
        // Get axis values for movement and rotation
        float moveInput = Input.GetAxisRaw(moveAxis);
        float rotateInput = Input.GetAxisRaw(rotateAxis);

        // Track whether movement is occurring
        isMoving = moveInput != 0 || rotateInput != 0;

        // Send input to pawn
        if (pawn != null)
        {
            pawn.Move(moveInput);
            if (Mathf.Abs(rotateInput) > 0.1f)
            {
                pawn.Rotate(rotateInput);
            }
        }

        // Debugging movement input
        Debug.Log($"Player {playerNumber} Move Input: {moveInput}");
        Debug.Log($"Player {playerNumber} Rotate Input: {rotateInput}");
    }

    private void HandleShooting()
    {
        // Prevent firing when shooting is disabled
        if (!canShoot) return;

        // Check for shoot key press
        if (Input.GetKeyDown(shootKey))
        {
            isShooting = true;

            // Ensure enough time has passed since last shot
            if (Time.time >= nextFireTime)
            {
                FireProjectile();
                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            isShooting = false;
        }
    }

    private void FireProjectile()
    {
        // Ensure prefab and fire point are valid
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("ERROR: Missing projectile prefab or fire point!");
            return;
        }

        // Instantiate and launch projectile
        GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectileScript = newProjectile.GetComponent<Projectile>();

        // Initialize the projectile with this player as the source
        if (projectileScript != null)
        {
            activeProjectile = newProjectile;
            projectileScript.Initialize(gameObject, projectileSpeed, projectileDamage);
        }

        // Play shooting sound
        if (fireSound != null)
        {
            AudioManager.Instance.PlaySFX(fireSound);
        }
    }

    // Called by projectile when it is destroyed
    public void OnProjectileDestroyed()
    {
        activeProjectile = null;
    }

    // Respawn the player at a spawn point
    private void Respawn()
    {
        currentHealth = maxHealth;
        if (gameManager != null)
        {
            gameManager.RespawnPlayer(playerNumber);
        }

        if (pawn != null && pawn is TankPawn tankPawn)
        {
            tankPawn.ResetMovement(); //reset speed after respawn
            tankPawn.ResetPhysics();
        }
    }



    // Used by AI to detect player noise
    public bool IsMakingNoise()
    {
        return isMoving || isShooting;
    }

    private void FindGameUIManager()
    {
        gameUIManager = FindObjectOfType<UIManager_GameScene>();
    }

    // Apply damage to the player and update UI
    public void TakeDamage(int damage)
    {

        if (pawn is TankPawn tankPawn)
        {
            tankPawn.ResetPhysics(); // stop unwanted movement after hit
        }

        currentHealth -= damage;
        Debug.Log($"[Player {playerNumber}] Took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            LoseLife();
        }

        if (gameUIManager != null)
        {
            gameUIManager.UpdateLives(playerNumber, lives);
        }
    }


    private void LoseLife()
    {
        lives--;
        Debug.Log($"[Player {playerNumber}] Lost a life. Lives remaining: {lives}");

        if (lives <= 0)
        {
            Debug.Log($"[Player {playerNumber}] OUT OF LIVES. Calling GameManager.PlayerDied");
            if (gameManager != null)
            {
                gameManager.PlayerDied(playerNumber);
            }

            Destroy(gameObject);
        }
        else
        {
            Respawn();
        }
    }


}

















