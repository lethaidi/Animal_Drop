using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class FruitSpawner : MonoBehaviour
{
    [Header("Spawn Rate")]
    public float[] spawnWeights; // tỉ lệ tương ứng với fruitPrefabs

    [Header("Settings")]
    public GameObject[] fruitPrefabs; // Kéo 5 loại quả nhỏ nhất vào đây
    public float spawnDelay = 1.0f;
    public float boxWidth = 2.5f;    // Độ rộng của miệng hộp để giới hạn quả

    public GameObject arrow;
    public GameObject currentFruit;
    public bool canSpawn = true, isPause = false;

    [Header("Next Fruit Preview")]
    private int fruitIndex = 0;
    public Image nextFruitPreview;
    private GameObject nextFruit;
    public Sprite[] fruitSprites; 

    [Header("Check Condition")]
    public bool canInteract = true;

    [Header("UI Block Areas")]
    public List<RectTransform> uiBlocks; 

    public Loading loading;

    private int currentIndex;
    private int nextIndex;

    public AudioSource audioSource;

    void Update()
    {
        CheckMouse();
        if (!canInteract) return;

        if (GameManager.Instance.isGameOver) return;

        if (isPause) return;

        if (canSpawn && currentFruit == null)
        {
            StartCoroutine(SpawnNewFruit());
        }

        if (loading != null && !loading.isLoaded) return;

        if (currentFruit != null)
        {
            MoveFruit();

            if (Input.GetMouseButtonUp(0)) // Khi thả chuột
            {
                DropFruit();

                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
        }
    }

    void MoveFruit()
    {
        // Chuyển tọa độ chuột sang tọa độ thế giới trong Game
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Giới hạn vị trí X để quả không đâm xuyên qua tường hai bên
        float xPos = Mathf.Clamp(mousePos.x, -boxWidth, boxWidth);

        // Cập nhật vị trí quả (chỉ thay đổi X, giữ nguyên Y của Spawner)
        currentFruit.transform.position = new Vector3(xPos, transform.position.y, 0);
        // Cập nhật vị trí mũi tên
        if (arrow != null)
        {
            arrow.transform.position = new Vector3(xPos, arrow.transform.position.y, 0);
        }
    }

    IEnumerator SpawnNewFruit()
    {
        canSpawn = false;
        yield return new WaitForSeconds(spawnDelay);

        arrow.SetActive(true);

        currentIndex = nextIndex;

        currentFruit = Instantiate(fruitPrefabs[currentIndex], transform.position, Quaternion.identity);

        Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
        rb.simulated = false;

        canSpawn = true;

        // random next
        nextIndex = GetRandomFruitIndex();
        nextFruitPreview.sprite = fruitSprites[nextIndex];
    }

    void DropFruit()
    {
        Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
        rb.simulated = true;
        Fruit fruit = currentFruit.GetComponent<Fruit>();
        fruit.isDropped = true;

        currentFruit = null;

        arrow.SetActive(false);
    }

    public void Pause()
    {
        isPause = true;
    }

    public void Resume()
    {
        StartCoroutine(WaitPause());
    }

    IEnumerator WaitPause()
    {
        yield return new WaitForSeconds(0.2f);
        isPause = !isPause;
    }

    public void CheckMouse()
    {
        if (uiBlocks == null || uiBlocks.Count == 0) return;

        Vector2 mousePos = Input.mousePosition;
        canInteract = true;

        foreach (RectTransform rect in uiBlocks)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, mousePos))
            {
                canInteract = false;
                return;
            }
        }
    }

    public void Change()
    {
        isPause = true;

        StartCoroutine(WaitChange());
    }

    IEnumerator WaitChange()
    {
        yield return new WaitForSeconds(5f);

        if (currentFruit != null)
        {
            Destroy(currentFruit);
        }

        int oldIndex = currentIndex;

        do
        {
            currentIndex = Random.Range(0, fruitPrefabs.Length);
        }
        while (currentIndex == oldIndex);

        currentFruit = Instantiate(fruitPrefabs[currentIndex], transform.position, Quaternion.identity);

        Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
        rb.simulated = false;

        isPause = false;
    }

    int GetRandomFruitIndex()
    {
        float totalWeight = 0f;

        foreach (float w in spawnWeights)
            totalWeight += w;

        float randomValue = Random.Range(0, totalWeight);

        float current = 0f;

        for (int i = 0; i < spawnWeights.Length; i++)
        {
            current += spawnWeights[i];
            if (randomValue <= current)
                return i;
        }

        return 0; // fallback
    }
}