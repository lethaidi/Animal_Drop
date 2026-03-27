using UnityEngine;

public class Fruit : MonoBehaviour
{
    public FruitData data;
    public bool isCombined = false;
    public bool isDropped = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fruit"))
        {
            Fruit other = collision.gameObject.GetComponent<Fruit>();

            // Kiểm tra nếu cùng loại và chưa bị kết hợp
            if (other.data.index == this.data.index && !isCombined && !other.isCombined)
            {
                isCombined = true;
                other.isCombined = true;

                // Gọi Manager để tạo quả mới ở vị trí giữa 2 quả cũ
                Vector3 spawnPos = (transform.position + collision.transform.position) / 2;
                GameManager.Instance.LevelUpFruit(data.index, spawnPos);

                Destroy(gameObject);
                Destroy(collision.gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("OutZone"))
        {
            GameManager.Instance.LoseGame();
        }
    }
}