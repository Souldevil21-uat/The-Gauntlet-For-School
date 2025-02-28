using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<PlayerController> playerControllers = new List<PlayerController>();
    public List<TankPawn> tankPawns = new List<TankPawn>();
    public List<AIController> aiControllers = new List<AIController>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ✅ Add back Tank Registration functions
    public void RegisterTank(TankPawn tank)
    {
        if (!tankPawns.Contains(tank))
        {
            tankPawns.Add(tank);
        }
    }

    public void UnregisterTank(TankPawn tank)
    {
        if (tankPawns.Contains(tank))
        {
            tankPawns.Remove(tank);
        }
    }

    // ✅ Register and Unregister AI
    public void RegisterAI(AIController ai)
    {
        if (!aiControllers.Contains(ai))
        {
            aiControllers.Add(ai);
            Debug.Log("✅ GameManager: AI Registered -> " + ai.gameObject.name);
        }
    }

    public void UnregisterAI(AIController ai)
    {
        if (aiControllers.Contains(ai))
        {
            aiControllers.Remove(ai);
            Debug.Log("❌ GameManager: AI Unregistered -> " + ai.gameObject.name);
        }
    }

    // ✅ Get the player (Ensures PlayerController is available)
    public PlayerController GetPlayer()
    {
        if (playerControllers.Count > 0)
        {
            Debug.Log("✅ GameManager: Returning Player -> " + playerControllers[0].gameObject.name);
            return playerControllers[0];
        }
        Debug.LogError("❌ GameManager: No player found!");
        return null;
    }

    // ✅ Register and Unregister Player
    public void RegisterPlayer(PlayerController player)
    {
        if (!playerControllers.Contains(player))
        {
            playerControllers.Add(player);
        }
    }

    public void UnregisterPlayer(PlayerController player)
    {
        if (playerControllers.Contains(player))
        {
            playerControllers.Remove(player);
        }
    }

    // ✅ Helper Function: Get Nearest AI to a Position
    public AIController GetNearestAI(Vector3 position)
    {
        AIController nearestAI = null;
        float minDistance = Mathf.Infinity;

        foreach (AIController ai in aiControllers)
        {
            float distance = Vector3.Distance(position, ai.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestAI = ai;
            }
        }

        Debug.Log("🔍 Nearest AI Found: " + (nearestAI != null ? nearestAI.gameObject.name : "None"));
        return nearestAI;
    }

    // ✅ Helper Function: Get AI with Highest Health
    public AIController GetStrongestAI()
    {
        AIController strongestAI = null;
        float highestHealth = 0f;

        foreach (AIController ai in aiControllers)
        {
            Health aiHealth = ai.GetComponent<Health>();
            if (aiHealth != null && aiHealth.currentHealth > highestHealth)
            {
                highestHealth = aiHealth.currentHealth;
                strongestAI = ai;
            }
        }

        Debug.Log("💪 Strongest AI Found: " + (strongestAI != null ? strongestAI.gameObject.name : "None"));
        return strongestAI;
    }

    // ✅ Helper Function: Get All AI in Chase Mode
    public List<AIController> GetAllChasingAI()
    {
        List<AIController> chasingAI = new List<AIController>();

        foreach (AIController ai in aiControllers)
        {
            if (ai is AIPatrolChase patrolAI && patrolAI.currentState is ChaseState)
            {
                chasingAI.Add(ai);
            }
        }

        Debug.Log("🚨 AI in ChaseState: " + chasingAI.Count);
        return chasingAI;
    }
}



