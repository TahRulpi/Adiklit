using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Blood Drop Setup")]
    public GameObject[] bloodDropPrefabs;
    public float maxSpawnX = 2.5f;     // how far left/right drops can spawn
    public float spawnInterval = 1.5f; // time between spawns

    // NEW FIELD: Assign the Transform of the Panel where drops should appear!
    [Header("Spawning Location")]
    public Transform spawnParent;

    [Header("Game Over Setup")]
    public GameObject gameOverPanel;
    public int maxMisses = 5;

    [Header("UI (optional)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI missesText;

    private float dropSpawnY;
    private int score = 0;
    private int missedCount = 0;
    private bool isGameOver = false;

    // NEW: Index to track which prefab to spawn next
    private int nextDropIndex = 0;

    void Awake()
    {
        // Calculate top of screen for drop spawning
        if (Camera.main != null && Camera.main.orthographic)
        {
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

    // ... (HandleMiss, EndGame, AddScore, UpdateScoreUI, UpdateMissesUI methods remain unchanged) ...

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
        CancelInvoke(nameof(SpawnNextDrop));
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
    /// Selects the next blood drop prefab sequentially and spawns it at a random X position.
    /// </summary>
    public void SpawnNextDrop()
    {
        if (isGameOver) return;

        if (bloodDropPrefabs == null || bloodDropPrefabs.Length == 0)
        {
            Debug.LogError("Blood Drop Prefabs array is empty or not assigned! Cannot spawn drops.");
            return;
        }

        // CRITICAL CHECK: Ensure the parent is assigned
        if (spawnParent == null)
        {
            Debug.LogError("Spawn Parent is not assigned! Drops will spawn under the scene root.");
        }

        // 1. Select the current prefab in the sequence
        GameObject prefabToSpawn = bloodDropPrefabs[nextDropIndex];

        // 2. Determine random spawn position (in World Space)
        float randomX = Random.Range(-maxSpawnX, maxSpawnX);
        Vector3 spawnPos = new Vector3(randomX, dropSpawnY, 0);

        // 3. Instantiate the selected prefab
        GameObject newDrop = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        // 4. Set the parent immediately after instantiation
        if (spawnParent != null)
        {
            // The 'false' argument ensures the drop retains its world position (spawnPos)
            newDrop.transform.SetParent(spawnParent, false);
        }

        // 5. Update the index for the next spawn
        // This makes the sequence loop back to the first prefab after the last one
        nextDropIndex = (nextDropIndex + 1) % bloodDropPrefabs.Length;


        // 6. Pass the GameManager reference
        BloodDrop dropScript = newDrop.GetComponent<BloodDrop>();
        if (dropScript != null)
        {
            dropScript.gameManager = this;
        }
    }
}
