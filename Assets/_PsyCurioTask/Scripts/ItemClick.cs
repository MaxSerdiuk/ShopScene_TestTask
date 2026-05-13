using UnityEngine;

public class ItemClick : MonoBehaviour
{
    [Header("Settings")]
    public Transform counterTop;
    public int maxItems = 5;

    private static int currentItemCount = 0;

    void OnMouseDown()
    {
        if (currentItemCount >= maxItems)
        {
            Debug.Log("Counter is full! Maximum 5 items.");
            return;
        }

        PlaceItemOnCounter();
    }

    void PlaceItemOnCounter()
    {
        // Створюємо копію товару
        GameObject copy = Instantiate(gameObject);

        // Розміщуємо на прилавку
        copy.transform.position = new Vector3(
            counterTop.position.x + (currentItemCount * 0.6f) - 1.2f,
            counterTop.position.y + 0.3f,
            counterTop.position.z
        );

        // Видаляємо скрипт з копії
        Destroy(copy.GetComponent<ItemClick>());

        currentItemCount++;
        Debug.Log($"Item placed! Total: {currentItemCount}");
    }

    public static void ResetCounter()
    {
        currentItemCount = 0;
    }
}