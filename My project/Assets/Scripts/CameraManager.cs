using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; } // Singleton

    public Camera singlePlayerCamera; // Used for single player mode
    public Camera player1Camera;      // Used for split-screen Player 1
    public Camera player2Camera;      // Used for split-screen Player 2

    public Transform player1Target;   // Target to follow for Player 1
    public Transform player2Target;   // Target to follow for Player 2
    public float smoothSpeed = 5f;    // Camera follow speed

    private int playerCount = 1;      // Tracks how many players are active

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
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

        // Set initial camera mode based on player count
        UpdateCameraMode();
    }

    private void LateUpdate()
    {
        FindPlayerTargets();  // Constantly update references to active players
        FollowPlayers();      // Smoothly follow each target
    }

    public void UpdateCameraMode()
    {
        bool isTwoPlayer = PlayerPrefs.GetInt("TwoPlayerMode", 0) == 1;
        playerCount = isTwoPlayer ? 2 : 1;

        // Set up the correct camera(s) depending on player count
        if (playerCount == 1)
        {
            singlePlayerCamera.gameObject.SetActive(true);
            player1Camera.gameObject.SetActive(false);
            player2Camera.gameObject.SetActive(false);
            singlePlayerCamera.rect = new Rect(0f, 0f, 1f, 1f);
        }
        else if (playerCount == 2)
        {
            singlePlayerCamera.gameObject.SetActive(false);
            player1Camera.gameObject.SetActive(true);
            player2Camera.gameObject.SetActive(true);
            player1Camera.rect = new Rect(0f, 0f, 0.5f, 1f);
            player2Camera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
        }

        // Update the camera targets to follow correct players
        FindPlayerTargets();
    }

    private void FindPlayerTargets()
    {
        // Update Player 1 target if necessary
        if (GameManager.Instance.player1 != null)
        {
            Transform newTarget = GameManager.Instance.player1.transform;
            if (player1Target != newTarget)
            {
                player1Target = newTarget;
            }
        }

        // Update Player 2 target dynamically based on tag
        GameObject activePlayer2 = GameObject.FindWithTag("Player 2");
        if (activePlayer2 != null)
        {
            Transform newTarget = activePlayer2.transform;
            if (player2Target != newTarget)
            {
                player2Target = newTarget;
            }
        }
    }

    private void FollowPlayers()
    {
        // Single player follow
        if (playerCount == 1 && singlePlayerCamera != null && player1Target != null)
        {
            Vector3 targetPosition = player1Target.position + new Vector3(0, 10, -10);
            singlePlayerCamera.transform.position = Vector3.Lerp(singlePlayerCamera.transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            singlePlayerCamera.transform.LookAt(player1Target);
        }
        // Split-screen follow logic
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

    // External control for updating player count manually
    public void SetPlayerCount(int count)
    {
        playerCount = Mathf.Clamp(count, 1, 2);
        UpdateCameraMode();
    }
}







