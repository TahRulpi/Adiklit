using UnityEngine;
using TMPro;
using UnityEngine.UI; // Needed for the Button and Image components

public class GameManager : MonoBehaviour
{
    [Header("Blood Drop Setup")]
    public GameObject[] bloodDropPrefabs;
    public float maxSpawnX = 2.5f;
    public float spawnInterval = 1.5f;

    [Header("Spawning Location")]
    public Transform spawnParent;

    [Header("Game Over Setup")]
    public GameObject gameOverPanel;
    public int maxMisses = 5;

    [Header("UI (optional)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI missesText;

    // --- NEW: CAPACITY TRACKING & REMINDER UI ---
    [Header("Capacity & Reload Reminder")]
    // The maximum number of drops the panty can absorb before needing a change/reload.
    public int pantyMaxCapacity = 20;

    // Assign your 'Reload Reminder' Button GameObject here.
    public GameObject reloadReminderButton;

    // Assign the Image that visually fills up (the 'BloodFill' child of your panty).
    public Image pantyFillImage;
    // ----------------------------------------------

    private float dropSpawnY;
    private int score = 0;
    private int missedCount = 0;
    private bool isGameOver = false;
    private int nextDropIndex = 0;

    // --- NEW: TRACKER FOR THE PANTY'S CURRENT FILL LEVEL ---
    private int pantyCurrentFill = 0;
    // -------------------------------------------------------

    void Awake()
    {
        if (Camera.main != null && Camera.main.orthographic)
        {
            dropSpawnY = Camera.main.orthographicSize + 1f;
        }
        else
        {
            dropSpawnY = 5f;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Hide the reminder button at the start of the game
        if (reloadReminderButton != null)
        {
            reloadReminderButton.SetActive(false);
        }
    }

    void Start()
    {
        InvokeRepeating(nameof(SpawnNextDrop), 0f, spawnInterval);
        UpdateScoreUI();
        UpdateMissesUI();

        // Ensure fill image starts empty
        if (pantyFillImage != null)
        {
            pantyFillImage.fillAmount = 0f;
        }
    }

    /// <summary>
    /// Called when the panty successfully absorbs a drop.
    /// </summary>
    public void AddScore()
    {
        if (isGameOver) return;
        score++;
        UpdateScoreUI();

        // --- NEW: INCREMENT FILL AND CHECK CAPACITY ---
        if (pantyCurrentFill < pantyMaxCapacity)
        {
            pantyCurrentFill++;
            UpdatePantyFillUI();

            // Check if the capacity limit has been reached after this drop
            if (pantyCurrentFill >= pantyMaxCapacity)
            {
                // Capacity is full: Show the reload reminder!
                if (reloadReminderButton != null)
                {
                    reloadReminderButton.SetActive(true);
                }
            }
        }
        // If it's already full, we don't increment the score, but we should probably lose the game or lose a life.
        // For now, it just won't increment 'pantyCurrentFill' past 'pantyMaxCapacity'.
    }

    /// <summary>
    /// Updates the visual fill level of the panty UI image.
    /// </summary>
    private void UpdatePantyFillUI()
    {
        if (pantyFillImage == null) return;

        float fillRatio = (float)pantyCurrentFill / pantyMaxCapacity;
        pantyFillImage.fillAmount = Mathf.Clamp01(fillRatio);
    }

    // --- NEW: METHOD FOR THE RELOAD BUTTON TO CALL (E.G., ON CLICK) ---
    /// <summary>
    /// Resets the panty's capacity and hides the reminder button.
    /// You should hook this up to the Reload Reminder Button's OnClick event in the Inspector.
    /// </summary>
    public void ReloadPanty()
    {
        pantyCurrentFill = 0; // Reset capacity
        UpdatePantyFillUI();  // Update the UI to show empty

        if (reloadReminderButton != null)
        {
            reloadReminderButton.SetActive(false); // Hide the button
        }

        Debug.Log("Panty reloaded/switched!");
    }

    // ... (HandleMiss, EndGame, UpdateScoreUI, UpdateMissesUI, SpawnNextDrop remain the same) ...

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

    public void SpawnNextDrop()
    {
        if (isGameOver) return;

        if (bloodDropPrefabs == null || bloodDropPrefabs.Length == 0)
        {
            Debug.LogError("Blood Drop Prefabs array is empty or not assigned!");
            return;
        }

        if (spawnParent == null)
        {
            Debug.LogError("Spawn Parent is not assigned! Drops will spawn under the scene root.");
        }

        GameObject prefabToSpawn = bloodDropPrefabs[nextDropIndex];
        float randomX = Random.Range(-maxSpawnX, maxSpawnX);
        Vector3 spawnPos = new Vector3(randomX, dropSpawnY, 0);

        GameObject newDrop = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        if (spawnParent != null)
        {
            newDrop.transform.SetParent(spawnParent, false);
        }

        nextDropIndex = (nextDropIndex + 1) % bloodDropPrefabs.Length;

        BloodDrop dropScript = newDrop.GetComponent<BloodDrop>();
        if (dropScript != null)
        {
            dropScript.gameManager = this;
        }
    }
}