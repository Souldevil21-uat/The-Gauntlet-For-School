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
        // Singleton pattern to ensure only one GameManager exists
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

    // Registers a tank and ensures no duplicates
    public void RegisterTank(TankPawn tank)
    {
        if (tank != null && !tankPawns.Contains(tank))
        {
            tankPawns.Add(tank);
        }
    }

    // Unregisters a tank
    public void UnregisterTank(TankPawn tank)
    {
        if (tank != null && tankPawns.Contains(tank))
        {
            tankPawns.Remove(tank);
        }
    }

    // Registers an AI and ensures no duplicates
    public void RegisterAI(AIController ai)
    {
        if (ai != null && !aiControllers.Contains(ai))
        {
            aiControllers.Add(ai);
        }
    }

    // Unregisters an AI
    public void UnregisterAI(AIController ai)
    {
        if (ai != null && aiControllers.Contains(ai))
        {
            aiControllers.Remove(ai);
        }
    }

    // Returns the first registered player controller
    public PlayerController GetPlayer()
    {
        if (playerControllers.Count > 0 && playerControllers[0] != null)
        {
            return playerControllers[0];
        }
        return null;
    }

    // Registers a player controller
    public void RegisterPlayer(PlayerController player)
    {
        if (player != null && !playerControllers.Contains(player))
        {
            playerControllers.Add(player);
        }
    }

    // Unregisters a player controller
    public void UnregisterPlayer(PlayerController player)
    {
        if (player != null && playerControllers.Contains(player))
        {
            playerControllers.Remove(player);
        }
    }

    // Returns the AI controller closest to a given position
    public AIController GetNearestAI(Vector3 position)
    {
        if (aiControllers.Count == 0) return null;

        AIController nearestAI = null;
        float minDistance = Mathf.Infinity;

        foreach (AIController ai in aiControllers)
        {
            if (ai == null) continue;

            float distance = Vector3.Distance(position, ai.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestAI = ai;
            }
        }
        return nearestAI;
    }

    // Returns the AI controller with the highest health
    public AIController GetStrongestAI()
    {
        if (aiControllers.Count == 0) return null;

        AIController strongestAI = null;
        float highestHealth = 0f;

        foreach (AIController ai in aiControllers)
        {
            if (ai == null) continue;

            Health aiHealth = ai.GetComponent<Health>();
            if (aiHealth != null && aiHealth.currentHealth > highestHealth)
            {
                highestHealth = aiHealth.currentHealth;
                strongestAI = ai;
            }
        }
        return strongestAI;
    }

    // Returns a list of AI controllers that are currently in ChaseState
    public List<AIController> GetAllChasingAI()
    {
        List<AIController> chasingAI = new List<AIController>();

        foreach (AIController ai in aiControllers)
        {
            if (ai != null && ai.currentState is ChaseState)
            {
                chasingAI.Add(ai);
            }
        }
        return chasingAI;
    }
}





