using UnityEngine;

public class ItemClick : MonoBehaviour
{
    [Header("Settings")]
    public Transform counterTop;
    public int maxItems = 5;

    [Header("Placement Offset")]
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;

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
        GameObject copy = Instantiate(gameObject);
        copy.transform.position = new Vector3(
            counterTop.position.x + (currentItemCount * 0.9f) + 0.0f,
            counterTop.position.y + 0.5f + positionOffset.y,
            counterTop.position.z + positionOffset.z
        );
        copy.transform.rotation = Quaternion.Euler(rotationOffset);
        Destroy(copy.GetComponent<ItemClick>());
        currentItemCount++;
        Debug.Log($"Item placed! Total: {currentItemCount}");
    }

    public static void ResetCounter()
    {
        currentItemCount = 0;
    }
}