using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CashRegisterClick : MonoBehaviour
{
    [Header("Item Prices")]
    public float crownPrice = 10f;
    public float laughingMaskPrice = 3f;
    public float magicWandPrice = 15f;
    public float swordPrice = 3f;
    public float mirrorPrice = 7f;

    private Dictionary<string, float> itemPrices;

    void Start()
    {
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
                    .Trim();
                boughtItems.Add(cleanName);
                totalPrice += itemPrices[obj.name];
            }
        }

        if (boughtItems.Count == 0)
        {
            Debug.Log("No items on counter!");
            return;
        }

        string itemList = string.Join(", ", boughtItems);
        Debug.Log($"You bought: {itemList} | Total: {totalPrice}€");

        StartCoroutine(ClearCounter());
    }

    IEnumerator ClearCounter()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject[] allObjects = FindObjectsByType<GameObject>
            (FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (itemPrices.ContainsKey(obj.name))
            {
                Destroy(obj);
            }
        }

        ItemClick.ResetCounter();
        Debug.Log("Counter cleared!");
    }
}