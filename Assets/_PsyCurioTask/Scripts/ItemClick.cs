using UnityEngine;
using System.Collections;

public class ItemClick : MonoBehaviour
{
    [Header("Settings")]
    // Anchor point on the counter for placing items
    public Transform counterTop;
    // Maximum number of items allowed on the counter
    public int maxItems = 5;

    [Header("Placement Offset")]
    // Position and rotation offsets for proper visual alignment
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;

    // Global counter to enforce the item limit
    private static int currentItemCount = 0;
    
    // Global array representing "parking slots" on the counter, bounded by maxItems
    private static bool[] occupiedSlots; 

    // Indicates if this specific object is a clone sitting on the counter (not the shelf original)
    private bool isCounterItem = false;
    
    // Prevents double-clicking while the item is already being removed
    private bool isDisappearing = false;
    
    // The specific parking slot index occupied by this clone
    private int mySlotIndex = -1; 

    // --- METHOD FOR COMMUNICATION WITH CounterGlow ---
    // Allows other scripts to safely retrieve the current item count
    public static int GetCurrentItemCount()
    {
        return currentItemCount;
    }
    // ---------------------------------------------------

    void Awake()
    {
        // Initialize the slot parking array once on startup
        if (occupiedSlots == null || occupiedSlots.Length != maxItems)
        {
            occupiedSlots = new bool[maxItems];
        }
    }

    void OnMouseDown()
    {
        // Logic for clicking an item that is ALREADY on the counter
        if (isCounterItem)
        {
            if (isDisappearing) return;
            StartCoroutine(RemoveFromCounterRoutine());
            return;
        }

        // Logic for clicking a shelf item when the counter is full
        if (currentItemCount >= maxItems)
        {
            Debug.Log("Counter is full! Maximum " + maxItems + " items.");
            return;
        }

        PlaceItemOnCounter();
    }

    void PlaceItemOnCounter()
    {
        // 1. Find the first available empty slot
        int availableSlot = -1;
        for (int i = 0; i < maxItems; i++)
        {
            if (!occupiedSlots[i])
            {
                availableSlot = i;
                break;
            }
        }

        if (availableSlot == -1) return;

        // 2. Create a clone of the item
        GameObject copy = Instantiate(gameObject);
        ItemClick copyClick = copy.GetComponent<ItemClick>();
        copyClick.isCounterItem = true;
        copyClick.mySlotIndex = availableSlot;
        occupiedSlots[availableSlot] = true;

        // 3. Move and rotate the clone to the proper slot on the counter
        copy.transform.position = new Vector3(
            counterTop.position.x + (availableSlot * 0.9f),
            counterTop.position.y + 0.5f + positionOffset.y,
            counterTop.position.z + positionOffset.z
        );
        copy.transform.rotation = Quaternion.Euler(rotationOffset);

        // 4. Update the global counter
        currentItemCount++;
        
        if (CashRegisterClick.Instance != null)
        {
            CashRegisterClick.Instance.RegisterItem(copy);
        }
    }

    // RESTORED TO ORIGINAL LOGIC: Just waits 1 second and destroys the object
    IEnumerator RemoveFromCounterRoutine()
    {
        // Block double-clicking
        isDisappearing = true;

        // Decrease global counter
        if (currentItemCount > 0)
        {
            currentItemCount--;
        }
        
        // Free up the parking slot
        if (mySlotIndex != -1)
        {
            occupiedSlots[mySlotIndex] = false;
        }

        // Unregister from the cash register logic immediately
        if (CashRegisterClick.Instance != null)
        {
            CashRegisterClick.Instance.UnregisterItem(gameObject);
        }

        // Delay before destruction (Original logic restored)
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    // Method to fully reset the counter state (used during final checkout)
    public static void ResetCounter()
    {
        currentItemCount = 0;
        if (occupiedSlots != null)
        {
            for (int i = 0; i < occupiedSlots.Length; i++)
            {
                occupiedSlots[i] = false;
            }
        }
    }
}