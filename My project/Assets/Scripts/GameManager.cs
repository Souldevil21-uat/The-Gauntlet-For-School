using UnityEngine;
using System.Collections.Generic; // Allows us to use Lists

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Singleton instance

    public List<PlayerController> playerControllers = new List<PlayerController>(); // Stores all PlayerControllers
    public List<TankPawn> tankPawns = new List<TankPawn>(); // Stores all TankPawns

    private void Awake()
    {
        // Ensure there's only one GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps GameManager when switching scenes
        }
        else
        {
            Destroy(gameObject); // Prevents duplicate GameManagers
        }
    }

    // Register a PlayerController when a new tank joins
    public void RegisterPlayer(PlayerController player)
    {
        if (!playerControllers.Contains(player))
        {
            playerControllers.Add(player);
        }
    }

    // Unregister a PlayerController when a tank leaves
    public void UnregisterPlayer(PlayerController player)
    {
        if (playerControllers.Contains(player))
        {
            playerControllers.Remove(player);
        }
    }

    // Register a TankPawn when it spawns
    public void RegisterTank(TankPawn tank)
    {
        if (!tankPawns.Contains(tank))
        {
            tankPawns.Add(tank);
        }
    }

    // Unregister a TankPawn when it is destroyed
    public void UnregisterTank(TankPawn tank)
    {
        if (tankPawns.Contains(tank))
        {
            tankPawns.Remove(tank);
        }
    }
}
