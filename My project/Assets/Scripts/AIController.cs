using UnityEngine;
using System.Collections.Generic;

public abstract class AIController : Controller
{
    [Header("AI Detection Settings")]
    [SerializeField] protected float detectionRange = 20f;
    [SerializeField] protected float hearingRange = 15f;
    [SerializeField] protected float fieldOfView = 120f;
    [SerializeField] protected float inSightsFOV = 30f; // More aggressive vision when directly in front

    [Header("AI Behavior Settings")]
    [SerializeField] public float chaseSpeed = 6f;
    [SerializeField] public float fleeSpeed = 8f;
    [SerializeField] public float patrolSpeed = 4f;

    [Header("Shooting Settings")]
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected float projectileSpeed = 20f;
    [SerializeField] protected float projectileDamage = 10f;
    [SerializeField] protected float fireRate = 1.5f;
    public float nextFireTime = 0f;

    [Header("Patrolling Settings")]
    [SerializeField] public List<Transform> patrolPoints;

    public GameObject player; // Cached player reference
    protected State currentState;

    protected override void Start()
    {
        base.Start();
        GameManager.Instance.RegisterAI(this);
        // Attempt to find the player manually
        player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError(gameObject.name + " could not find the player!");
        }

        // Start in PatrolState if patrol points exist, else remain idle
        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            ChangeState(new PatrolState(this));
        }
    }

    public GameObject GetPlayer()
    {
        return GameManager.Instance.GetPlayer()?.gameObject;
    }


    protected override void Update()
    {
        base.Update();

        if (currentState != null)
        {
            currentState.Execute(this);
        }

        // AI reacts to hearing the player
        if (CanHearPlayer())
        {
            if (this is AIFlee && !(currentState is FleeState))
            {
                ChangeState(new FleeState(this));
            }
            else if (!(this is AIFlee) && !(currentState is ChaseState))
            {
                ChangeState(new ChaseState(this));
            }
        }

        // AI fires projectiles when it sees the player
        if (currentState is ChaseState && CanSeePlayer() && Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance?.UnregisterAI(this); // Ensure AI is removed from tracking when destroyed
    }

    // Ensures AI can move
    protected bool CanMove()
    {
        return pawn != null && currentState != null;
    }

    // Moves the AI toward a target position
    public void MoveTowards(Vector3 targetPosition, float speed)
    {
        if (!CanMove())
        {
            Debug.Log(gameObject.name + " tried to move but CannotMove() returned false.");
            return;
        }

        Debug.Log(gameObject.name + " moving towards: " + targetPosition);

        Vector3 direction = (targetPosition - transform.position).normalized;
        pawn.Move(speed);
        RotateTowards(targetPosition);
    }

    // Rotates the AI toward a target position
    public void RotateTowards(Vector3 targetPosition)
    {
        if (!CanMove()) return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180f * Time.deltaTime);
    }

    // Checks if AI can see the player
    public virtual bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 playerPosition = player.transform.position;
        Vector3 directionToPlayer = (playerPosition - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);

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
    private float hearingCooldown = 2f;

    // Checks if AI can hear the player
    public virtual bool CanHearPlayer()
    {
        if (player == null) return false;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        bool isHearing = distance <= hearingRange && player.GetComponent<PlayerController>()?.IsMakingNoise() == true;

        if (isHearing && Time.time > lastHeardTime + hearingCooldown)
        {
            lastHeardTime = Time.time;
            return true;
        }
        return false;
    }

    // Fires a projectile
    public void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectileScript = newProjectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.Initialize(gameObject, projectileSpeed, projectileDamage);
        }
    }

    // Handles AI state transitions
    public virtual void ChangeState(State newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter();
    }
}













