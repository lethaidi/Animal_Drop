using UnityEngine;

public class BoxWall : MonoBehaviour
{
    public float minLimitX = -3f; // Giới hạn bên trái
    public float maxLimitX = 3f;  // Giới hạn bên phải

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Fruit"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            // 1. Kiểm tra xem nó văng sang trái hay phải
            float clampX = Mathf.Clamp(other.transform.position.x, minLimitX, maxLimitX);

            // 2. Teleport quả cầu về lại trong biên (giữ nguyên độ cao Y)
            other.transform.position = new Vector3(clampX, other.transform.position.y, 0);

            // 3. Quan trọng: Reset lực để không bị văng tiếp
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;

            Debug.Log("Đã cứu một quả văng khỏi map!");
        }
    }
}
