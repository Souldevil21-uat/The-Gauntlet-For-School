using UnityEngine;
using System.Collections.Generic;

public abstract class AIController : Controller
{
    [Header("AI Detection Settings")]
    [SerializeField] public float detectionRange = 20f;
    [SerializeField] public float hearingRange = 15f;
    [SerializeField] public float fieldOfView = 120f;
    [SerializeField] public float inSightsFOV = 30f; // 🔹 More aggressive vision when directly in front

    [Header("AI Behavior Settings")]
    [SerializeField] public float chaseSpeed = 6f;
    [SerializeField] public float fleeSpeed = 8f;
    [SerializeField] public float patrolSpeed = 4f;

    [Header("Shooting Settings")]
    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public Transform firePoint;
    [SerializeField] public float projectileSpeed = 20f;
    [SerializeField] public float projectileDamage = 10f;
    [SerializeField] public float fireRate = 1.5f;
    public float nextFireTime = 0f;

    [Header("Patrolling Settings")]
    [SerializeField] public List<Transform> patrolPoints;

    public GameObject player { get; private set; }
    public State currentState;

    protected override void Start()
    {
        base.Start();
        GameManager.Instance.RegisterAI(this);
        player = GameManager.Instance.GetPlayer()?.gameObject;

        Debug.Log(gameObject.name + " 🟢 AI Variables Assigned - " +
                  "Hearing: " + hearingRange +
                  ", FOV: " + fieldOfView +
                  ", Chase Speed: " + chaseSpeed +
                  ", Flee Speed: " + fleeSpeed);

        // ✅ Start in PatrolState if patrol points exist, else idle
        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            Debug.Log(gameObject.name + " 🟢 Starting in PatrolState.");
            ChangeState(new PatrolState(this));
        }
        else
        {
            Debug.LogWarning(gameObject.name + " ❗ No patrol points assigned. AI will stay idle.");
        }
    }

    protected override void Update()
    {
        base.Update();

        if (currentState != null)
        {
            Debug.Log(gameObject.name + " 🔄 Running state: " + currentState.GetType().Name);
            currentState.Execute(this);
        }

        // ✅ FIX: AI Flee Tanks Should Not Enter ChaseState
        if (CanHearPlayer())
        {
            if (this is AIFlee && !(currentState is FleeState))
            {
                Debug.Log(gameObject.name + " 🚨 Heard the player! **Entering FleeState!**");
                ChangeState(new FleeState(this));
            }
            else if (!(this is AIFlee) && !(currentState is ChaseState)) // ✅ Other AI enters ChaseState
            {
                Debug.Log(gameObject.name + " 👂 Heard the player! **Entering ChaseState!**");
                ChangeState(new ChaseState(this));
            }
        }

        if (currentState is ChaseState && CanSeePlayer() && Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + fireRate;
        }
    }

    // ✅ AI Can Move Check
    public bool CanMove()
    {
        return pawn != null && currentState != null;
    }

    // ✅ Improved MoveTowards function
    public void MoveTowards(Vector3 targetPosition, float speed = 4f)
    {
        if (!CanMove()) return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        pawn.Move(speed);
        RotateTowards(targetPosition);
    }

    // ✅ Improved RotateTowards function
    public void RotateTowards(Vector3 targetPosition)
    {
        if (!CanMove()) return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180f * Time.deltaTime);
    }

    // ✅ Optimized CanSeePlayer function
    public virtual bool CanSeePlayer()
    {
        if (player == null)
        {
            Debug.LogWarning(gameObject.name + " ❗ Cannot see player: Player is null.");
            return false;
        }

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (angleToPlayer < fieldOfView / 2 && distanceToPlayer < detectionRange)
        {
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, detectionRange))
            {
                return hit.collider.gameObject == player;
            }
        }
        return false;
    }

    private float lastHeardTime = -5f;
    private float hearingCooldown = 2f;  // AI won’t react to the same sound again for 2 seconds

    public virtual bool CanHearPlayer()
    {
        if (player == null) return false;

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController == null) return false;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        bool isHearing = distance <= hearingRange && playerController.IsMakingNoise();

        if (isHearing && Time.time > lastHeardTime + hearingCooldown)
        {
            lastHeardTime = Time.time;
            Debug.Log(gameObject.name + " 👂 HEARD the player and is reacting!");
            return true;
        }

        return false;
    }

    // ✅ Optimized FireProjectile function
    public void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError(gameObject.name + " ❌ ERROR: Missing Projectile Prefab or FirePoint!");
            return;
        }

        GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectileScript = newProjectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.Initialize(gameObject, projectileSpeed, projectileDamage);
        }
        else
        {
            Debug.LogError(gameObject.name + " ❌ ERROR: Projectile script missing!");
        }
    }

    public virtual void ChangeState(State newState)
    {
        if (currentState != null)
        {
            Debug.Log(gameObject.name + " 🔄 Exiting State: " + currentState.GetType().Name);
            currentState.Exit();
        }

        currentState = newState;
        Debug.Log(gameObject.name + " 🏁 Entering State: " + currentState.GetType().Name);
        currentState.Enter();
    }
}










