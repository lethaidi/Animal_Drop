using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FruitSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] fruitPrefabs; // Kéo 5 loại quả nhỏ nhất vào đây
    public float spawnDelay = 1.0f;
    public float boxWidth = 2.5f;    // Độ rộng của miệng hộp để giới hạn quả

    private GameObject currentFruit;
    private bool canSpawn = true, isPause = false;

    [Header("Next Fruit Preview")]
    private int fruitIndex = 0;
    public Image nextFruitPreview;
    private GameObject nextFruit;

    [Header("Check Condition")]
    public bool canInteract = true;
    public Collider2D col;

    public Loading loading;

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
    }

    IEnumerator SpawnNewFruit()
    {
        canSpawn = false;
        yield return new WaitForSeconds(spawnDelay);

        currentFruit = Instantiate(fruitPrefabs[fruitIndex], transform.position, Quaternion.identity);

        // QUAN TRỌNG: Tạm thời tắt vật lý để quả không rơi ngay
        Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
        rb.simulated = false;

        canSpawn = true;

        // Cập nhật hình ảnh quả tiếp theo trong preview
        if (nextFruitPreview != null)
        {
            fruitIndex = Random.Range(0, fruitPrefabs.Length);
            nextFruitPreview.sprite = fruitPrefabs[fruitIndex].GetComponent<SpriteRenderer>().sprite;
        }
    }

    void DropFruit()
    {
        Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
        rb.simulated = true;
        Fruit fruit = currentFruit.GetComponent<Fruit>();
        fruit.isDropped = true;

        currentFruit = null;
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
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (col == Physics2D.OverlapPoint(mousePos))
        {
            canInteract = false;
        }
        else
        {
            canInteract = true;
        }
    }
}