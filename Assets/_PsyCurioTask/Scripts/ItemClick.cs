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

    void Awake()
    {
        // Ініціалізуємо парковку один раз на старті, задаючи їй розмір maxItems
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
            // Якщо він вже в процесі зникнення — ігноруємо кліки
            if (isDisappearing) return;

            // Запускаємо корутину видалення із затримкою
            StartCoroutine(RemoveFromCounterRoutine());
            return;
        }

        // Якщо клікнули по товару на полиці, але прилавок переповнений
        if (currentItemCount >= maxItems)
        {
            Debug.Log("Counter is full! Maximum " + maxItems + " items.");
            return;
        }

        // Якщо все ок — кладемо на прилавок
        PlaceItemOnCounter();
    }

    void PlaceItemOnCounter()
    {
        // 1. Шукаємо перше ВІЛЬНЕ місце на парковці (зліва направо)
        int availableSlot = -1;
        for (int i = 0; i < maxItems; i++)
        {
            if (!occupiedSlots[i]) // Якщо слот false (вільний)
            {
                availableSlot = i;
                break;
            }
        }

        // Перестраховка: якщо вільних місць чомусь немає
        if (availableSlot == -1) return;

        // 2. Створюємо копію об'єкта (клонуємо товар з полиці)
        GameObject copy = Instantiate(gameObject);

        // Налаштовуємо скрипт на створеній копії
        ItemClick copyClick = copy.GetComponent<ItemClick>();
        copyClick.isCounterItem = true; // Тепер це товар на прилавку
        
        // Передаємо клону його номер місця і відзначаємо це місце як зайняте
        copyClick.mySlotIndex = availableSlot;
        occupiedSlots[availableSlot] = true;

        // 3. Фізично переміщуємо клон на його паркувальне місце
        copy.transform.position = new Vector3(
            counterTop.position.x + (availableSlot * 0.9f),
            counterTop.position.y + 0.5f + positionOffset.y,
            counterTop.position.z + positionOffset.z
        );

        // Задаємо обертання
        copy.transform.rotation = Quaternion.Euler(rotationOffset);

        // Збільшуємо глобальний лічильник товарів
        currentItemCount++;
        Debug.Log($"Item placed in slot {availableSlot}! Total: {currentItemCount}");

        // Реєструємо цей новий клон у списку каси
        if (CashRegisterClick.Instance != null)
        {
            CashRegisterClick.Instance.RegisterItem(copy);
        }
    }

    // Корутина для видалення товару із затримкою
    IEnumerator RemoveFromCounterRoutine()
    {
        // Блокуємо можливість повторного кліку
        isDisappearing = true;

        // Зменшуємо загальний лічильник товарів
        if (currentItemCount > 0)
        {
            currentItemCount--;
        }
        
        // МИТТЄВО звільняємо паркувальне місце для нових товарів
        if (mySlotIndex != -1)
        {
            occupiedSlots[mySlotIndex] = false;
        }

        Debug.Log($"Item removed from slot {mySlotIndex}. Remaining total: {currentItemCount}");

        // МИТТЄВО видаляємо товар зі списку каси, щоб перерахувалася сума в чеку
        if (CashRegisterClick.Instance != null)
        {
            CashRegisterClick.Instance.UnregisterItem(gameObject);
        }

        // Чекаємо рівно 1 секунду перед фізичним знищенням об'єкта
        yield return new WaitForSeconds(1.0f);

        // Видаляємо об'єкт зі сцени
        Destroy(gameObject);
    }

    // Метод для повного скидання парковки (викликається з CashRegisterClick)
    public static void ResetCounter()
    {
        currentItemCount = 0;
        
        // Робимо всі слоти знову вільними
        if (occupiedSlots != null)
        {
            for (int i = 0; i < occupiedSlots.Length; i++)
            {
                occupiedSlots[i] = false;
            }
        }
    }
}