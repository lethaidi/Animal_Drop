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
    public GameObject fruitPrefab; 

    [Header("Game Info")]
    public int currentScore = 0;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText, scoreTextLosePanel, highestScoreText;

    [Header("Lose Condition")]
    public bool isGameOver = false;
    public GameObject losePanel; 
    
    [Header("Audio")]
    public AudioSource effectMusic;

    [Header("Shake Effect")]
    public float shakeForce = 5f;
    public float duration = 0.5f;
    private int shakeCountMax = 20, shakeCountCurrent = 0;
    public TextMeshProUGUI countText;

    private void Awake()
    {
        // Khởi tạo Singleton
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        ShowScore();

        shakeCountCurrent = shakeCountMax;
        if (countText != null)
        {
            countText.text = shakeCountCurrent.ToString() + "/" + shakeCountMax.ToString();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Shake();
        }
    }

    // Hàm này được gọi từ script Fruit.cs khi 2 quả chạm nhau
    public void LevelUpFruit(int currentIndex, Vector3 spawnPosition)
    {
        // Phát âm thanh khi quả được nâng cấp
        if (effectMusic != null)
        {
            effectMusic.Play();
        }

        int nextIndex = currentIndex + 1;

        // Cộng điểm
        currentScore += allFruitData[currentIndex].scoreValue;
        Debug.Log("Score: " + currentScore);
        ShowScore();

        // Nếu đã là quả to nhất (Dưa hấu) thì không nâng cấp nữa
        if (nextIndex >= allFruitData.Count)
        {
            Debug.Log("Bạn đã đạt đến cấp độ cao nhất!");
            return;
        }

        // Tạo quả mới
        SpawnFruit(nextIndex, spawnPosition);
    }

    public void SpawnFruit(int index, Vector3 position)
    {
        FruitData data = allFruitData[index];
        GameObject newFruit = Instantiate(fruitPrefab, position, Quaternion.identity);

        // Cập nhật dữ liệu cho quả mới tạo
        Fruit fruitScript = newFruit.GetComponent<Fruit>();
        fruitScript.data = data;

        // Cập nhật hình ảnh và kích thước dựa trên Data
        newFruit.GetComponent<SpriteRenderer>().sprite = data.sprite; // Giả sử trong FruitData có biến sprite
        newFruit.transform.localScale = Vector3.one * data.spawnScale;

        // Đảm bảo quả mới sinh ra có vật lý ngay
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

        // Cập nhật điểm cao nếu cần
        int highestScore = PlayerPrefs.GetInt("HighestScore", 0);
        if (currentScore > highestScore)
        {
            PlayerPrefs.SetInt("HighestScore", currentScore);
            highestScore = currentScore;
        }
        // Hiển thị điểm cao nhất
        if (highestScoreText != null)
        {
            highestScoreText.text = highestScore.ToString();
        }

        // Hiển thị lose panel
        if (losePanel != null)
        {
            losePanel.gameObject.SetActive(true);
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("Main");
    }

    public void OutGame()
    {
        Application.Quit();
    }

    // Shake 
    public void Shake()
    {
        if (shakeCountCurrent == 0) return;
        StartCoroutine(DoShake());

        shakeCountCurrent--;
        if (countText != null)
        {
            countText.text = shakeCountCurrent.ToString() + "/" + shakeCountMax.ToString();
        }
    }

    IEnumerator DoShake()
    {
        float time = 0;

        while (time < duration)
        {
            Fruit[] all = FindObjectsOfType<Fruit>();

            foreach (var rb in all)
            {
                if (rb == null) continue;
                if (!rb.isDropped) continue;

                Rigidbody2D rid = rb.GetComponent<Rigidbody2D>();
                if (rid == null) continue;

                float horizontal = Random.Range(-0.4f, 0.4f);
                float vertical = Random.Range(0.08f, 0.1f);

                Vector2 force = new Vector2(horizontal, vertical);
                rid.AddForce(force * shakeForce, ForceMode2D.Impulse);
            }

            time += Time.deltaTime;
            yield return null;
        }
    }
}