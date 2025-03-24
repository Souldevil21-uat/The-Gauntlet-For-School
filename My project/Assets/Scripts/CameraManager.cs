using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; } // Singleton

    [Header("Camera References")]
    public Camera singlePlayerCamera;
    public Camera player1Camera;
    public Camera player2Camera;

    [Header("Camera Follow Settings")]
    public Transform player1Target;
    public Transform player2Target;
    public float smoothSpeed = 5f;

    private int playerCount = 1;

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

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("CameraManager: GameManager instance is missing.");
            return;
        }

        UpdateCameraMode();
    }

    private void LateUpdate()
    {
        FindPlayerTargets();  // Keep checking if player targets exist
        FollowPlayers();      // Then follow them
    }

    public void UpdateCameraMode()
    {
        bool isTwoPlayer = PlayerPrefs.GetInt("TwoPlayerMode", 0) == 1;
        playerCount = isTwoPlayer ? 2 : 1;

        if (playerCount == 1)
        {
            Debug.Log("🎥 Single Player Mode: Enabling SinglePlayerCamera only");

            singlePlayerCamera.gameObject.SetActive(true);
            player1Camera.gameObject.SetActive(false);
            player2Camera.gameObject.SetActive(false);

            singlePlayerCamera.rect = new Rect(0f, 0f, 1f, 1f); // Fullscreen
        }
        else if (playerCount == 2)
        {
            Debug.Log("🎮 Two Player Mode: Enabling Split-Screen Cameras");

            singlePlayerCamera.gameObject.SetActive(false);
            player1Camera.gameObject.SetActive(true);
            player2Camera.gameObject.SetActive(true);

            player1Camera.rect = new Rect(0f, 0f, 0.5f, 1f);  // Left half
            player2Camera.rect = new Rect(0.5f, 0f, 0.5f, 1f); // Right half
        }

        FindPlayerTargets();
    }

    private void FindPlayerTargets()
    {
        // Find Player 1
        if (GameManager.Instance.player1 != null)
        {
            player1Target = GameManager.Instance.player1.transform;
            Debug.Log("🎯 Player 1 Target Updated: " + player1Target.name);
        }

        // Find Player 2 (Check for clones dynamically)
        GameObject activePlayer2 = GameObject.FindWithTag("Player 2"); // Find active player 2
        if (activePlayer2 != null)
        {
            player2Target = activePlayer2.transform;
            Debug.Log("🎯 Player 2 Target Updated: " + player2Target.name);
        }
        else
        {
            Debug.LogWarning("⚠️ No Active Player 2 Found! Split-screen camera might be broken.");
        }
    }


    private void FollowPlayers()
    {
        if (playerCount == 1 && singlePlayerCamera != null && player1Target != null)
        {
            Vector3 targetPosition = player1Target.position + new Vector3(0, 10, -10);
            singlePlayerCamera.transform.position = Vector3.Lerp(singlePlayerCamera.transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            singlePlayerCamera.transform.LookAt(player1Target);
        }
        else if (playerCount == 2)
        {
            if (player1Camera != null && player1Target != null)
            {
                Vector3 targetPosition = player1Target.position + new Vector3(0, 10, -10);
                player1Camera.transform.position = Vector3.Lerp(player1Camera.transform.position, targetPosition, smoothSpeed * Time.deltaTime);
                player1Camera.transform.LookAt(player1Target);
            }

            if (player2Camera != null && player2Target != null)
            {
                Vector3 targetPosition = player2Target.position + new Vector3(0, 10, -10);
                player2Camera.transform.position = Vector3.Lerp(player2Camera.transform.position, targetPosition, smoothSpeed * Time.deltaTime);
                player2Camera.transform.LookAt(player2Target);
            }
        }
    }

    // Allows external scripts to manually update the camera settings
    public void SetPlayerCount(int count)
    {
        playerCount = Mathf.Clamp(count, 1, 2);
        UpdateCameraMode();
    }
}






