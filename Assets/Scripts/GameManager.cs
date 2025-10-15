using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Blood Drop Setup")]
    // This is now an array. Drag ALL your blood drop prefabs here!
    public GameObject[] bloodDropPrefabs;
    public float maxSpawnX = 2.5f;     // how far left/right drops can spawn
    public float spawnInterval = 1.5f; // time between spawns

    [Header("Game Over Setup")]
    // Assign your Game Over Panel here in the Inspector!
    public GameObject gameOverPanel;
    public int maxMisses = 5; // The maximum allowed misses before game over

    [Header("UI (optional)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI missesText; // Text to display the current miss count

    private float dropSpawnY;
    private int score = 0;
    private int missedCount = 0;
    private bool isGameOver = false;

    void Awake()
    {
        // Calculate top of screen for drop spawning
        if (Camera.main != null && Camera.main.orthographic)
        {
            // Spawns drops just off the top edge of the screen
            dropSpawnY = Camera.main.orthographicSize + 1f;
        }
        else
        {
            dropSpawnY = 5f;
        }

        // Ensure the Game Over panel is hidden at the start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Start()
    {
        // Start continuously spawning drops on a timer
        InvokeRepeating(nameof(SpawnNextDrop), 0f, spawnInterval);
        UpdateScoreUI();
        UpdateMissesUI();
    }

    /// <summary>
    /// Called by a BloodDrop when it hits the "Ground" collider.
    /// </summary>
    public void HandleMiss()
    {
        if (isGameOver) return;

        missedCount++;
        UpdateMissesUI();

        if (missedCount >= maxMisses)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        isGameOver = true;

        // Stop the spawning of new drops
        CancelInvoke(nameof(SpawnNextDrop));

        // Activate the Game Over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Debug.Log("Game Over! Missed too many drops.");
    }

    public void AddScore()
    {
        if (isGameOver) return;
        score++;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    private void UpdateMissesUI()
    {
        if (missesText != null)
            missesText.text = $"Misses: {missedCount} / {maxMisses}";
    }

    /// <summary>
    /// Selects a random blood drop prefab and spawns it at a random X position.
    /// </summary>
    public void SpawnNextDrop()
    {
        if (isGameOver) return;

        // CRITICAL CHECK: Ensure we have prefabs to spawn
        if (bloodDropPrefabs == null || bloodDropPrefabs.Length == 0)
        {
            Debug.LogError("Blood Drop Prefabs array is empty or not assigned! Cannot spawn drops.");
            return;
        }

        // 1. Select a random index from the prefabs array
        int randomIndex = Random.Range(0, bloodDropPrefabs.Length);
        GameObject prefabToSpawn = bloodDropPrefabs[randomIndex];

        // 2. Determine random spawn position
        float randomX = Random.Range(-maxSpawnX, maxSpawnX);
        Vector3 spawnPos = new Vector3(randomX, dropSpawnY, 0);

        // 3. Instantiate the selected prefab
        GameObject newDrop = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        // 4. Pass the GameManager reference to the new drop object
        BloodDrop dropScript = newDrop.GetComponent<BloodDrop>();
        if (dropScript != null)
        {
            dropScript.gameManager = this;
        }
    }
}
