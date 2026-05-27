using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class CashRegisterClick : MonoBehaviour
{
    [Header("UI References")]
    public GameObject speechBalloon;
    public TextMeshProUGUI speechText;

    [Header("Checkout Flow Dependencies")]
    public Button payLeaveButton;
    public CanvasGroup uiCanvasGroup;
    public SellerClick seller;

    [Header("Item Prices")]
    public float crownPrice = 10f;
    public float laughingMaskPrice = 3f;
    public float magicWandPrice = 15f;
    public float swordPrice = 3f;
    public float mirrorPrice = 7f;

    public static CashRegisterClick Instance { get; private set; }

    private List<GameObject> counterItems = new List<GameObject>();
    private Dictionary<string, float> itemPrices;
    private Coroutine hideBalloonCoroutine;

    void Awake()
    {
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
        speechBalloon.SetActive(false);

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
        if (payLeaveButton != null && !payLeaveButton.interactable) return;

        // LOGIC FOR AN EMPTY COUNTER
        if (counterItems.Count == 0)
        {
            if (speechBalloon.activeSelf && speechText.text == "No items selected")
            {
                StopExistingHideCoroutine();
                speechBalloon.SetActive(false);
            }
            else
            {
                speechBalloon.SetActive(true);
                speechText.text = "No items selected";

                // Hide the payment button when there are no items on the counter
                if (payLeaveButton != null)
                {
                    payLeaveButton.gameObject.SetActive(false);
                }

                StopExistingHideCoroutine();
                hideBalloonCoroutine = StartCoroutine(HideNoItemsBalloonRoutine());
            }
            return;
        }

        // LOGIC IF THERE ARE ITEMS ON THE COUNTER
        StopExistingHideCoroutine();
        speechBalloon.SetActive(true);
        RefreshBalloon();
    }

    private void StopExistingHideCoroutine()
    {
        if (hideBalloonCoroutine != null)
        {
            StopCoroutine(hideBalloonCoroutine);
            hideBalloonCoroutine = null;
        }
    }

    IEnumerator HideNoItemsBalloonRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        speechBalloon.SetActive(false);
        hideBalloonCoroutine = null;
    }

    public void RegisterItem(GameObject item)
    {
        if (payLeaveButton != null && !payLeaveButton.interactable) return;

        StopExistingHideCoroutine();
        counterItems.Add(item);

        // === LIGHT INTEGRATION ===
        // Notify the glow controller about the item count change (activates if 1-5 items)
        if (CashRegisterGlow.Instance != null)
        {
            CashRegisterGlow.Instance.UpdateGlowState(counterItems.Count);
        }
        
        if (speechBalloon.activeSelf)
        {
            RefreshBalloon();
        }
    }

    public void UnregisterItem(GameObject item)
    {
        if (payLeaveButton != null && !payLeaveButton.interactable) return;

        if (counterItems.Contains(item))
        {
            counterItems.Remove(item);
        }

        // === LIGHT INTEGRATION ===
        // Notify the glow controller about the updated item count after removal
        if (CashRegisterGlow.Instance != null)
        {
            CashRegisterGlow.Instance.UpdateGlowState(counterItems.Count);
        }

        if (speechBalloon.activeSelf || counterItems.Count == 0)
        {
            RefreshBalloon();
        }
    }

    public void ClearCounter()
    {
        StopExistingHideCoroutine();

        // === LIGHT INTEGRATION ===
        // Defensive check to turn off the glow during manual or complete counter clear
        if (CashRegisterGlow.Instance != null)
        {
            CashRegisterGlow.Instance.StopBreathing();
        }

        foreach (GameObject item in counterItems)
        {
            if (item != null) Destroy(item);
        }

        counterItems.Clear();
        ItemClick.ResetCounter();
        speechBalloon.SetActive(false);
    }

    public void RefreshBalloon()
    {
        // If no items are left, hide the entire speech balloon
        if (counterItems.Count == 0)
        {
            speechBalloon.SetActive(false);
            return;
        }

        float totalPrice = 0f;
        string itemList = "";

        foreach (GameObject item in counterItems)
        {
            if (item == null) continue;

            string rawName = item.name;

            if (itemPrices.ContainsKey(rawName))
            {
                float price = itemPrices[rawName];
                totalPrice += price;

                string cleanName = rawName
                    .Replace("(Clone)", "")
                    .Replace("Item_", "")
                    .Replace("LaughingMask", "Laughing Mask")
                    .Replace("MagicWand", "Magic Wand")
                    .Trim();

                itemList += $"- {cleanName}: {price} ¤\n";
            }
        }

        speechText.text = $"You're going to use:\n\n{itemList}\nYou will owe {totalPrice} ¤";
        
        // Since there are items on the counter, ensure the pay button is visible again
        if (payLeaveButton != null)
        {
            payLeaveButton.gameObject.SetActive(true);
        }

        speechBalloon.SetActive(true);
    }

    // ==========================================
    // TIMELINE INTERACTION FLOW (9.0 sec total)
    // ==========================================

    public void PayAndLeave()
    {
        StartCoroutine(PayAndLeaveRoutine());
    }

    private IEnumerator PayAndLeaveRoutine()
    {
        // === TIMESTAMP 0.0 sec ===
        if (payLeaveButton != null) 
        {
            payLeaveButton.interactable = false;
            payLeaveButton.gameObject.SetActive(false); // Hide the button immediately to prevent double-clicks
        }
        speechText.text = "It's a great choice!\n\nGood luck and see you soon!";

        // === LIGHT INTEGRATION ===
        // Once payment is pressed, the green light immediately starts fading out smoothly
        if (CashRegisterGlow.Instance != null)
        {
            CashRegisterGlow.Instance.StopBreathing();
        }

        // === PAUSE 3.0 sec ===
        yield return new WaitForSeconds(3.0f);

        // === TIMESTAMP 3.0 sec ===
        yield return StartCoroutine(FadeOutUIRoutine(3.0f));

        // === TIMESTAMP 6.0 sec ===
        yield return StartCoroutine(ShrinkItemsRoutine(3.0f));

        // === TIMESTAMP 9.0 sec (FINAL) ===
        if (seller != null)
        {
            seller.Wave();
        }

        CleanupCheckout();
    }

    private IEnumerator FadeOutUIRoutine(float duration)
    {
        float elapsed = 0f;
        if (uiCanvasGroup != null)
        {
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                uiCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                yield return null;
            }
            uiCanvasGroup.alpha = 0f;
        }
        else
        {
            yield return new WaitForSeconds(duration);
        }

        if (speechBalloon != null) speechBalloon.SetActive(false);
    }

    private IEnumerator ShrinkItemsRoutine(float duration)
    {
        float elapsed = 0f;

        List<Vector3> originalScales = new List<Vector3>();
        foreach (GameObject item in counterItems)
        {
            if (item != null) originalScales.Add(item.transform.localScale);
            else originalScales.Add(Vector3.zero);
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            for (int i = 0; i < counterItems.Count; i++)
            {
                if (counterItems[i] != null)
                {
                    counterItems[i].transform.localScale = Vector3.Lerp(originalScales[i], Vector3.zero, t);
                }
            }
            yield return null;
        }
    }

    private void CleanupCheckout()
    {
        foreach (GameObject item in counterItems)
        {
            if (item != null) Destroy(item);
        }

        counterItems.Clear();
        ItemClick.ResetCounter();

        if (uiCanvasGroup != null) uiCanvasGroup.alpha = 1f;
        if (payLeaveButton != null)
        {
            payLeaveButton.interactable = true;
            payLeaveButton.gameObject.SetActive(true); // Restore the initial visibility state of the button
        }
    }
}