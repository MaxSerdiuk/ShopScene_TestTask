using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CashRegisterClick : MonoBehaviour
{
    [Header("UI")]
    public GameObject speechBalloon;    // Панель бульбашки
    public TextMeshProUGUI speechText;  // Текст бульбашки

    [Header("Settings")]
    public float balloonDuration = 12f;  // Час показу бульбашки

    [Header("Item Prices")]
    public float crownPrice = 10f;
    public float laughingMaskPrice = 3f;
    public float magicWandPrice = 15f;
    public float swordPrice = 3f;
    public float mirrorPrice = 7f;

    private Dictionary<string, float> itemPrices;

    void Start()
    {
        // Ховаємо бульбашку на старті
        speechBalloon.SetActive(false);

        // Словник: назва об'єкта → ціна
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
        // Збираємо всі товари на прилавку
        List<string> boughtItems = new List<string>();
        float totalPrice = 0f;

        GameObject[] allObjects = FindObjectsByType<GameObject>
            (FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (itemPrices.ContainsKey(obj.name))
            {
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

        // Якщо прилавок порожній
        if (boughtItems.Count == 0)
        {
            ShowBalloon("No items selected!");
            return;
        }

        // Формуємо текст з кожним товаром і ціною
        string itemList = "";
        foreach (GameObject obj in FindObjectsByType<GameObject>
            (FindObjectsSortMode.None))
        {
            if (itemPrices.ContainsKey(obj.name))
            {
                string cleanName = obj.name
                    .Replace("(Clone)", "")
                    .Replace("Item_", "")
                    .Replace("LaughingMask", "Laughing Mask")
                    .Replace("MagicWand", "Magic Wand")
                    .Trim();
                itemList += $"{cleanName}: {itemPrices[obj.name]}Ѧ\n";
            }
        }

        // Показуємо бульбашку з текстом
        string message = $"You're going to use\n\n{itemList}\nYou will owe {totalPrice} Ѧ ";
        ShowBalloon(message);
    }

    void ShowBalloon(string message)
    {
        // Встановлюємо текст і показуємо бульбашку
        speechText.text = message;
        speechBalloon.SetActive(true);
        StartCoroutine(BalloonSequence());
    }

    IEnumerator BalloonSequence()
    {
        // Чекаємо поки гравець читає
        yield return new WaitForSeconds(balloonDuration);

        // Ховаємо бульбашку
        speechBalloon.SetActive(false);

        // Пауза перед очищенням прилавку
        yield return new WaitForSeconds(2f);

        // Очищаємо прилавок
        ClearCounter();
    }

    void ClearCounter()
    {
        // Знаходимо і видаляємо всі товари на прилавку
        GameObject[] allObjects = FindObjectsByType<GameObject>
            (FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (itemPrices.ContainsKey(obj.name))
            {
                Destroy(obj);
            }
        }

        // Скидаємо лічильник товарів
        ItemClick.ResetCounter();
        Debug.Log("Counter cleared!");
    }
}