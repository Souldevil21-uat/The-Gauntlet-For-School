using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : Controller
{
    [Header("Player Settings")]
    public int playerNumber = 1;
    public int maxHealth = 100;
    private int currentHealth;
    public int lives = 3;
    public AudioClip fireSound;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;
    public float projectileDamage = 10f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;
    private bool canShoot = true;

    [Header("Movement Tracking")]
    public bool isMoving { get; private set; }
    public bool isShooting { get; private set; }

    private GameObject activeProjectile = null;
    private GameManager gameManager;
    private UIManager_GameScene gameUIManager;

    // 🔹 Explicit Input Mappings (Fixed)
    private string moveAxis;
    private string rotateAxis;
    private KeyCode shootKey;

    protected override void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        FindGameUIManager();
        currentHealth = maxHealth;

        // ✅ Ensure player number is assigned correctly
        if (gameObject.CompareTag("Player 2"))
        {
            playerNumber = 2;
        }
        else
        {
            playerNumber = 1;
        }

        Debug.Log($"🎮 PlayerController Initialized: Player {playerNumber}");

        // 🔹 Assign controls based on player number
        if (playerNumber == 1)
        {
            moveAxis = "Vertical";  // WASD
            rotateAxis = "Horizontal";
            shootKey = KeyCode.Space;
        }
        else if (playerNumber == 2)
        {
            moveAxis = "P2_Vertical";  // Arrow Keys
            rotateAxis = "P2_Horizontal";
            shootKey = KeyCode.RightControl;
        }

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
        HandleMovement();
        HandleShooting();
    }

    // ✅ FIX: Player-Specific Movement
    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw(moveAxis);  // Ensures only one input per player
        float rotateInput = Input.GetAxisRaw(rotateAxis);

        isMoving = moveInput != 0 || rotateInput != 0;

        if (pawn != null)
        {
            pawn.Move(moveInput);
            pawn.Rotate(rotateInput);
        }

        Debug.Log($"Player {playerNumber} Move Input: {Input.GetAxisRaw(moveAxis)}");
        Debug.Log($"Player {playerNumber} Rotate Input: {Input.GetAxisRaw(rotateAxis)}");

    }

    // ✅ FIX: Player-Specific Shooting
    private void HandleShooting()
    {
        if (!canShoot) return;

        if (Input.GetKeyDown(shootKey))
        {
            isShooting = true;
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

        // 🎵 Fire sound
        if (fireSound != null)
        {
            AudioManager.Instance.PlaySFX(fireSound);
        }
    }


    public void OnProjectileDestroyed()
    {
        activeProjectile = null;
    }

    private void Respawn()
    {
        currentHealth = maxHealth;
        if (gameManager != null)
        {
            gameManager.RespawnPlayer(playerNumber);
        }
    }

    public bool IsMakingNoise()
    {
        return isMoving || isShooting;
    }

    private void FindGameUIManager()
    {
        gameUIManager = FindObjectOfType<UIManager_GameScene>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

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

        if (lives <= 0)
        {
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
















