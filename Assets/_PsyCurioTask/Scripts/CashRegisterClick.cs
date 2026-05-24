using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CashRegisterClick : MonoBehaviour
{
    [Header("UI")]
    // Посилання на об'єкт бульбашки з текстом
    public GameObject speechBalloon;
    // Посилання на текстовий компонент бульбашки
    public TextMeshProUGUI speechText;

    [Header("Settings")]
    // Тривалість показу бульбашки (розраховується автоматично залежно від кількості товарів)
    public float balloonDuration = 7f;

    [Header("Item Prices")]
    // Ціни кожного товару
    public float crownPrice = 10f;
    public float laughingMaskPrice = 3f;
    public float magicWandPrice = 15f;
    public float swordPrice = 3f;
    public float mirrorPrice = 7f;

    // Словник: назва товару → ціна
    private Dictionary<string, float> itemPrices;

    void Start()
    {
        // Ховаємо бульбашку на старті
        speechBalloon.SetActive(false);

        // Заповнюємо словник цінами товарів
        // Назви з "(Clone)" бо Unity додає це до копій об'єктів
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
        // Список куплених товарів і загальна сума
        List<string> boughtItems = new List<string>();
        float totalPrice = 0f;

        // Знаходимо всі об'єкти у сцені
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        // Перевіряємо кожен об'єкт — чи є він товаром на прилавку
        foreach (GameObject obj in allObjects)
        {
            if (itemPrices.ContainsKey(obj.name))
            {
                // Очищаємо назву від технічних префіксів для відображення
                string cleanName = obj.name
                    .Replace("(Clone)", "")
                    .Replace("Item_", "")
                    .Replace("LaughingMask", "Laughing Mask")
                    .Replace("MagicWand", "Magic Wand")
                    .Trim();
                boughtItems.Add(cleanName);
                totalPrice += itemPrices[obj.name];
            }
        }

        // Якщо товарів на прилавку немає — показуємо повідомлення
        if (boughtItems.Count == 0)
        {
            ShowBalloon("No items selected!", 0);
            return;
        }

        // Формуємо список товарів з цінами для бульбашки
        string itemList = "";
        foreach (GameObject obj in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (itemPrices.ContainsKey(obj.name))
            {
                string cleanName = obj.name
                    .Replace("(Clone)", "")
                    .Replace("Item_", "")
                    .Replace("LaughingMask", "Laughing Mask")
                    .Replace("MagicWand", "Magic Wand")
                    .Trim();
                // Символ валюти ¤
                itemList += $"{cleanName}: {itemPrices[obj.name]} ¤\n";
            }
        }

        // Формуємо фінальне повідомлення з переліком і сумою
        string message = $"You're going to use:\n\n{itemList}\nYou will owe {totalPrice} ¤";
        ShowBalloon(message, boughtItems.Count);
    }

    void ShowBalloon(string message, int itemCount = 0)
    {
        // Встановлюємо текст і показуємо бульбашку
        speechText.text = message;
        speechBalloon.SetActive(true);

        // Розраховуємо тривалість: 5 сек + 1 сек на кожен товар
        // 1 товар = 6 сек, 2 = 7 сек, ..., 5 = 10 сек
       /*  balloonDuration = 5f + itemCount;

        StartCoroutine(BalloonSequence()); */
    }

/*     IEnumerator BalloonSequence()
    {
        // Чекаємо поки закінчиться час показу бульбашки
        yield return new WaitForSeconds(balloonDuration);

        // Ховаємо бульбашку
        speechBalloon.SetActive(false);

        // Чекаємо 3 секунди перед очищенням прилавку
        yield return new WaitForSeconds(3f);

        // Очищаємо прилавок
        ClearCounter();
    } */

    void ClearCounter()
    {
        // Знаходимо всі об'єкти у сцені і видаляємо товари з прилавку
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (itemPrices.ContainsKey(obj.name))
            {
                Destroy(obj);
            }
        }

        // Скидаємо лічильник товарів у ItemClick
        ItemClick.ResetCounter();
        Debug.Log("Counter cleared!");
    }
}