using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }

    [Header("Fruit Data")]
    public List<FruitData> allFruitData;
    public GameObject[] fruitPrefabs;

    [Header("Game Info")]
    public int currentScore = 0;
    public int highestScore = 0;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText, scoreTextLosePanel, highestScoreText;

    [Header("Lose Condition")]
    public bool isGameOver = false;
    public GameObject losePanel;

    [Header("Audio")]
    public AudioSource effectMusic;
    public AudioSource shakeSound;

    [Header("Shake Effect")]
    public float shakeForce = 5f;
    public float duration = 0.5f;
    private int shakeCountMax = 50, shakeCountCurrent = 0;
    public TextMeshProUGUI countText;
    public GameObject collectShakePanel;

    // ===== CAMERA SHAKE =====
    [Header("Camera Shake")]
    public Transform cam;
    private Vector3 camOriginalPos;

    public FruitSpawner fruitSpawner;

    private void Awake()
    {
        // Singleton
        if (Instance == null) { Instance = this; }

        ShowScore();

        shakeCountCurrent = shakeCountMax;
        if (countText != null)
        {
            countText.text = shakeCountCurrent + "/" + shakeCountMax;
        }

        // Load high score
        highestScore = PlayerPrefs.GetInt("HighestScore", 0);

        // Lưu vị trí camera
        if (cam != null)
        {
            camOriginalPos = cam.localPosition;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Shake();
        }
    }

    // Merge fruit
    public void LevelUpFruit(int currentIndex, Vector3 spawnPosition)
    {
        if (effectMusic != null)
        {
            effectMusic.Play();
        }

        int nextIndex = currentIndex + 1;

        currentScore += allFruitData[currentIndex].scoreValue;
        Debug.Log("Score: " + currentScore);
        ShowScore();

        if (nextIndex >= allFruitData.Count)
        {
            Debug.Log("Max level reached!");
            return;
        }

        SpawnFruit(nextIndex, spawnPosition);
    }

    public void SpawnFruit(int index, Vector3 position)
    {
        GameObject newFruit = Instantiate(fruitPrefabs[index], position, Quaternion.identity);

        newFruit.GetComponent<Rigidbody2D>().simulated = true;

        Fruit fruit = newFruit.GetComponent<Fruit>();
        fruit.isDropped = true;
    }

    void ShowScore()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    public void LoseGame()
    {
        if (isGameOver) return;

        isGameOver = true;

        if (scoreTextLosePanel != null)
        {
            scoreTextLosePanel.text = currentScore.ToString();
        }

        highestScore = PlayerPrefs.GetInt("HighestScore", 0);
        if (currentScore > highestScore)
        {
            PlayerPrefs.SetInt("HighestScore", currentScore);
            highestScore = currentScore;
        }

        if (highestScoreText != null)
        {
            highestScoreText.text = highestScore.ToString();
        }

        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        RateScore.Instance.Rate();
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("Main");
    }

    public void OutGame()
    {
        Application.Quit();
    }

    // ===== SHAKE =====
    public void Shake()
    {
        if (shakeCountCurrent == 0)
        {
            collectShakePanel.SetActive(true);
            fruitSpawner.isPause = true;
            return;
        }

        if (shakeSound != null)
        {
            shakeSound.Play();
        }

        shakeCountCurrent--;

        if (countText != null)
        {
            countText.text = shakeCountCurrent + "/" + shakeCountMax;
        }
        StartCoroutine(DoShake());
    }

    IEnumerator DoShake()
    {
        float time = 0;

        while (time < duration)
        {
            // Rung trái
            Fruit[] all = FindObjectsOfType<Fruit>();

            foreach (var rb in all)
            {
                if (rb == null) continue;
                if (!rb.isDropped) continue;

                Rigidbody2D rid = rb.GetComponent<Rigidbody2D>();
                if (rid == null) continue;

                float horizontal = Random.Range(-0.4f, 0.4f);
                float vertical = Random.Range(0.08f, 0.1f);

                rid.AddForce(new Vector2(horizontal, vertical) * shakeForce, ForceMode2D.Impulse);
            }

            // Rung camera (giảm dần)
            if (cam != null)
            {
                float strength = Mathf.Lerp(0.2f, 0f, time / duration);
                float shakeX = Random.Range(-strength, strength);
                float shakeY = Random.Range(-strength, strength);

                cam.localPosition = camOriginalPos + new Vector3(shakeX, shakeY, 0);
            }

            time += Time.deltaTime;
            yield return null;
        }

        // Reset camera
        if (cam != null)
        {
            cam.localPosition = camOriginalPos;
        }

        if (shakeCountCurrent == 0)
        {
            collectShakePanel.SetActive(true);
            fruitSpawner.isPause = true;
        }
    }

    public void MoreShake()
    {
        StartCoroutine(WaitPlus());
    }

    IEnumerator WaitPlus()
    {
        yield return new WaitForSeconds(5f);

        shakeCountCurrent = shakeCountMax;
        if (countText != null)
        {
            countText.text = shakeCountCurrent + "/" + shakeCountMax;
        }

        fruitSpawner.isPause = false;
    }
}