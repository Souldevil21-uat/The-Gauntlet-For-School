using UnityEngine;

public class AIChase : AIController
{
    protected override void Start()
    {
        base.Start();
        Debug.Log(gameObject.name + " 🚀 AIChase initialized, entering ChaseState.");
        ChangeState(new ChaseState(this)); // ✅ Ensures AI starts in chase mode
    }

    public override bool CanSeePlayer()
    {
        if (player == null)
        {
            Debug.LogWarning(gameObject.name + " ❗ Cannot see player: Player is null.");
            return false;
        }

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        Debug.Log(gameObject.name + " 👀 Checking vision: Distance=" + distanceToPlayer + ", Angle=" + angleToPlayer);

        if (distanceToPlayer < detectionRange && angleToPlayer < fieldOfView / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
            {
                if (hit.collider.gameObject == player)
                {
                    Debug.Log(gameObject.name + " ✅ Sees the player!");
                    return true;
                }
                else
                {
                    Debug.LogWarning(gameObject.name + " ❌ Raycast hit " + hit.collider.gameObject.name + " instead of player.");
                }
            }
        }
        return false;
    }

    public override void ChangeState(State newState)
    {
        if (currentState != null) currentState.Exit();
        base.ChangeState(newState); // ✅ Calls base to ensure proper transitions
    }
}




