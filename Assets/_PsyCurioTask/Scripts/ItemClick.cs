using UnityEngine;
using System.Collections;

public class ItemClick : MonoBehaviour
{
    [Header("Settings")]
    // Точка прив'язки на прилавку
    public Transform counterTop;
    // Максимальна кількість товарів на прилавку
    public int maxItems = 5;

    [Header("Placement Offset")]
    // Зміщення позиції та обертання для красивого розташування
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;

    // Глобальний лічильник для перевірки ліміту товарів
    private static int currentItemCount = 0;
    
    // Глобальний масив "паркувальних місць", який підлаштовується під maxItems
    private static bool[] occupiedSlots; 

    // Чи є цей конкретний об'єкт клоном на прилавку (а не оригіналом на полиці)
    private bool isCounterItem = false;
    
    // Захист від подвійного кліку під час зникнення товару
    private bool isDisappearing = false;
    
    // Номер паркувального місця (слота), яке займає ЦЕЙ клон
    private int mySlotIndex = -1; 

    // --- МЕТОД ДЛЯ ЗВ'ЯЗКУ З CounterGlow ---
    // Дозволяє іншим скриптам безпечно дізнатися кількість товарів
    public static int GetCurrentItemCount()
    {
        return currentItemCount;
    }
    // ----------------------------------------

    void Awake()
    {
        // Ініціалізуємо парковку один раз на старті
        if (occupiedSlots == null || occupiedSlots.Length != maxItems)
        {
            occupiedSlots = new bool[maxItems];
        }
    }

    void OnMouseDown()
    {
        // Якщо клікнули по товару, який ВЖЕ лежить на прилавку
        if (isCounterItem)
        {
            if (isDisappearing) return;
            StartCoroutine(RemoveFromCounterRoutine());
            return;
        }

        // Якщо клікнули по товару на полиці, але прилавок переповнений
        if (currentItemCount >= maxItems)
        {
            Debug.Log("Counter is full! Maximum " + maxItems + " items.");
            return;
        }

        PlaceItemOnCounter();
    }

    void PlaceItemOnCounter()
    {
        // 1. Шукаємо перше ВІЛЬНЕ місце
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

        // 2. Створюємо копію товару
        GameObject copy = Instantiate(gameObject);
        ItemClick copyClick = copy.GetComponent<ItemClick>();
        copyClick.isCounterItem = true;
        copyClick.mySlotIndex = availableSlot;
        occupiedSlots[availableSlot] = true;

        // 3. Переміщуємо та обертаємо клон
        copy.transform.position = new Vector3(
            counterTop.position.x + (availableSlot * 0.9f),
            counterTop.position.y + 0.5f + positionOffset.y,
            counterTop.position.z + positionOffset.z
        );
        copy.transform.rotation = Quaternion.Euler(rotationOffset);

        // 4. Оновлюємо лічильник
        currentItemCount++;
        
        if (CashRegisterClick.Instance != null)
        {
            CashRegisterClick.Instance.RegisterItem(copy);
        }
    }

    IEnumerator RemoveFromCounterRoutine()
    {
        // Блокуємо повторний клік
        isDisappearing = true;

        // Зменшуємо лічильник
        if (currentItemCount > 0)
        {
            currentItemCount--;
        }
        
        // Звільняємо слот
        if (mySlotIndex != -1)
        {
            occupiedSlots[mySlotIndex] = false;
        }

        if (CashRegisterClick.Instance != null)
        {
            CashRegisterClick.Instance.UnregisterItem(gameObject);
        }

        // Затримка перед знищенням
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }

    // Метод для повного скидання
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