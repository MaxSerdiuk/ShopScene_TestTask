using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections; // Обов'язково для корутин (IEnumerator)

public class CashRegisterClick : MonoBehaviour
{
    [Header("UI")]
    // Бульбашка з текстом
    public GameObject speechBalloon;
    // Текст усередині бульбашки
    public TextMeshProUGUI speechText;

    [Header("Item Prices")]
    // Ціни товарів
    public float crownPrice = 10f;
    public float laughingMaskPrice = 3f;
    public float magicWandPrice = 15f;
    public float swordPrice = 3f;
    public float mirrorPrice = 7f;

    // Singleton: єдина точка доступу до каси для всіх товарів
    public static CashRegisterClick Instance { get; private set; }

    // Список товарів, які фізично лежать на прилавку
    private List<GameObject> counterItems = new List<GameObject>();
    // Словник для швидкого пошуку ціни за назвою об'єкта
    private Dictionary<string, float> itemPrices;

    // Посилання на активну корутину автоприховування (щоб її можна було скасувати)
    private Coroutine hideBalloonCoroutine;

    void Awake()
    {
        // Ініціалізуємо Singleton та знищуємо дублікати, якщо вони є
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Ховаємо балон на старті гри
        speechBalloon.SetActive(false);

        // Заповнюємо словник цін (назви мають збігатися з іменами префабів)
        itemPrices = new Dictionary<string, float>
        {
            { "Item_Crown(Clone)", crownPrice },
            { "Item_LaughingMask(Clone)", laughingMaskPrice },
            { "Item_MagicWand(Clone)", magicWandPrice },
            { "Item_Sword(Clone)", swordPrice },
            { "Item_Mirror(Clone)", mirrorPrice }
        };
    }

    void OnMouseDown()
    {
        // Логіка для ПОРОЖНЬОГО прилавка
        if (counterItems.Count == 0)
        {
            // Якщо балон уже відкритий і показує "No items selected" — повторний клік його закриває
            if (speechBalloon.activeSelf && speechText.text == "No items selected")
            {
                StopExistingHideCoroutine();
                speechBalloon.SetActive(false);
            }
            else
            {
                // Інакше — відкриваємо балон і запускаємо таймер на 3 секунди
                speechBalloon.SetActive(true);
                speechText.text = "No items selected";

                StopExistingHideCoroutine();
                hideBalloonCoroutine = StartCoroutine(HideNoItemsBalloonRoutine());
            }
            return; // Перериваємо метод, далі рахувати чек не потрібно
        }

        // Логіка, якщо на прилавку Є ТОВАРИ
        StopExistingHideCoroutine(); // Про всяк випадок зупиняємо таймер помилки
        speechBalloon.SetActive(true);
        RefreshBalloon();
    }

    // Допоміжний метод для безпечної зупинки корутини таймера
    private void StopExistingHideCoroutine()
    {
        if (hideBalloonCoroutine != null)
        {
            StopCoroutine(hideBalloonCoroutine);
            hideBalloonCoroutine = null;
        }
    }

    // Корутина автоматичного приховування повідомлення через 3 секунди
    IEnumerator HideNoItemsBalloonRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        speechBalloon.SetActive(false);
        hideBalloonCoroutine = null;
    }

    // Метод для додавання товару в список каси (викликається з ItemClick)
    public void RegisterItem(GameObject item)
    {
        // Якщо товар додається, повідомлення "No items selected" більше не актуальне
        StopExistingHideCoroutine();

        counterItems.Add(item);
        
        // Оновлюємо чек у реальному часі, якщо балон зараз відкритий
        if (speechBalloon.activeSelf)
        {
            RefreshBalloon();
        }
    }

    // Метод для видалення товару зі списку каси
    public void UnregisterItem(GameObject item)
    {
        if (counterItems.Contains(item))
        {
            counterItems.Remove(item);
        }

        // Оновлюємо чек, або ховаємо його, якщо товарів більше не лишилося
        if (speechBalloon.activeSelf || counterItems.Count == 0)
        {
            RefreshBalloon();
        }
    }

    // Повне очищення прилавка
    public void ClearCounter()
    {
        StopExistingHideCoroutine();

        foreach (GameObject item in counterItems)
        {
            if (item != null) Destroy(item);
        }

        counterItems.Clear();
        ItemClick.ResetCounter();
        speechBalloon.SetActive(false);
    }

    // Метод перерахунку суми та формування тексту чека
    public void RefreshBalloon()
    {
        // Якщо товарів немає — просто ховаємо балон (захист)
        if (counterItems.Count == 0)
        {
            speechBalloon.SetActive(false);
            return;
        }

        float totalPrice = 0f;
        string itemList = "";

        // Проходимось по всіх товарах на прилавку
        foreach (GameObject item in counterItems)
        {
            if (item == null) continue;

            string rawName = item.name;

            // Якщо такий товар є у словнику цін
            if (itemPrices.ContainsKey(rawName))
            {
                float price = itemPrices[rawName];
                totalPrice += price;

                // Робимо назву красивою для гравця (без (Clone) та Item_)
                string cleanName = rawName
                    .Replace("(Clone)", "")
                    .Replace("Item_", "")
                    .Replace("LaughingMask", "Laughing Mask")
                    .Replace("MagicWand", "Magic Wand")
                    .Trim();

                itemList += $"- {cleanName}: {price} ¤\n";
            }
        }

        // Виводимо фінальний текст на екран
        speechText.text = $"You're going to use:\n\n{itemList}\nYou will owe {totalPrice} ¤";
        speechBalloon.SetActive(true);
    }
}