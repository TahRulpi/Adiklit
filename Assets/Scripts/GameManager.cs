using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject bloodDropPrefab;
    public TMP_Text scoreText;

    private int score = 0;

    void Start()
    {
        SpawnNextDrop();
        UpdateScore();
    }

    public void SpawnNextDrop()
    {
        float randomX = Random.Range(-2.2f, 2.2f);
        Vector3 spawnPos = new Vector3(randomX, 5.5f, 0);
        Instantiate(bloodDropPrefab, spawnPos, Quaternion.identity);
    }

    public void AddScore()
    {
        score++;
        UpdateScore();
    }

    private void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}
