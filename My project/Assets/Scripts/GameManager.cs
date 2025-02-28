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

    // ✅ Tank Registration
    public void RegisterTank(TankPawn tank)
    {
        if (tank != null && !tankPawns.Contains(tank))
        {
            tankPawns.Add(tank);
            Debug.Log("✅ GameManager: Registered Tank -> " + tank.gameObject.name);
        }
    }

    public void UnregisterTank(TankPawn tank)
    {
        if (tank != null && tankPawns.Contains(tank))
        {
            tankPawns.Remove(tank);
            Debug.Log("❌ GameManager: Unregistered Tank -> " + tank.gameObject.name);
        }
    }

    // ✅ AI Registration
    public void RegisterAI(AIController ai)
    {
        if (ai != null && !aiControllers.Contains(ai))
        {
            aiControllers.Add(ai);
            Debug.Log("✅ GameManager: AI Registered -> " + ai.gameObject.name);
        }
    }

    public void UnregisterAI(AIController ai)
    {
        if (ai != null && aiControllers.Contains(ai))
        {
            aiControllers.Remove(ai);
            Debug.Log("❌ GameManager: AI Unregistered -> " + ai.gameObject.name);
        }
    }

    // ✅ Get the Player (Ensures PlayerController is available)
    public PlayerController GetPlayer()
    {
        if (playerControllers.Count > 0 && playerControllers[0] != null)
        {
            Debug.Log("✅ GameManager: Returning Player -> " + playerControllers[0].gameObject.name);
            return playerControllers[0];
        }
        Debug.LogError("❌ GameManager: No valid player found!");
        return null;
    }

    // ✅ Player Registration
    public void RegisterPlayer(PlayerController player)
    {
        if (player != null && !playerControllers.Contains(player))
        {
            playerControllers.Add(player);
            Debug.Log("✅ GameManager: Registered Player -> " + player.gameObject.name);
        }
    }

    public void UnregisterPlayer(PlayerController player)
    {
        if (player != null && playerControllers.Contains(player))
        {
            playerControllers.Remove(player);
            Debug.Log("❌ GameManager: Unregistered Player -> " + player.gameObject.name);
        }
    }

    // ✅ Get Nearest AI to a Position
    public AIController GetNearestAI(Vector3 position)
    {
        if (aiControllers.Count == 0) return null;

        AIController nearestAI = null;
        float minDistance = Mathf.Infinity;

        foreach (AIController ai in aiControllers)
        {
            if (ai == null) continue; // Skip null AI

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

    // ✅ Get AI with Highest Health
    public AIController GetStrongestAI()
    {
        if (aiControllers.Count == 0) return null;

        AIController strongestAI = null;
        float highestHealth = 0f;

        foreach (AIController ai in aiControllers)
        {
            if (ai == null) continue; // Skip null AI

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

    // ✅ Get All AI in Chase Mode
    public List<AIController> GetAllChasingAI()
    {
        List<AIController> chasingAI = new List<AIController>();

        foreach (AIController ai in aiControllers)
        {
            if (ai != null && ai.currentState is ChaseState) // ✅ Works for all AI types
            {
                chasingAI.Add(ai);
            }
        }

        Debug.Log("🚨 AI in ChaseState: " + chasingAI.Count);
        return chasingAI;
    }
}




