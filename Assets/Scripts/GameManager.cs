using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Blood Drop Setup")]
    public GameObject bloodDropPrefab; // assign your prefab in inspector
    public float maxSpawnX = 2.5f;     // how far left/right drops can spawn
    public float spawnInterval = 1.5f; // time between spawns

    [Header("UI (optional)")]
    public TextMeshProUGUI scoreText;

    private float dropSpawnY;
    private int score = 0;

    void Awake()
    {
        // Calculate top of screen if camera is orthographic
        if (Camera.main != null && Camera.main.orthographic)
        {
            dropSpawnY = Camera.main.orthographicSize + 1f;
        }
        else
        {
            dropSpawnY = 5f;
        }
    }

    void Start()
    {
        // Start continuously spawning drops
        InvokeRepeating(nameof(SpawnNextDrop), 0f, spawnInterval);
        UpdateScoreUI();
    }

    public void AddScore()
    {
        score++;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void SpawnNextDrop()
    {
        if (bloodDropPrefab == null)
        {
            Debug.LogError("Blood Drop Prefab is not assigned!");
            return;
        }

        float randomX = Random.Range(-maxSpawnX, maxSpawnX);
        Vector3 spawnPos = new Vector3(randomX, dropSpawnY, 0);
        Instantiate(bloodDropPrefab, spawnPos, Quaternion.identity);
    }
}
