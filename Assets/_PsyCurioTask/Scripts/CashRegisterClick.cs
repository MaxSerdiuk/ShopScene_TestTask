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
        // Singleton pattern to ensure only one active CashRegisterClick
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
        // Ignore clicks if the pay button is already pressed (checkout in progress)
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
        
        if (payLeaveButton != null)
        {
            payLeaveButton.gameObject.SetActive(true);
        }

        speechBalloon.SetActive(true);
    }

    // ==========================================
    // TIMELINE INTERACTION FLOW
    // ==========================================

    public void PayAndLeave()
    {
        StartCoroutine(PayAndLeaveRoutine());
    }

    private IEnumerator PayAndLeaveRoutine()
    {
        if (payLeaveButton != null) 
        {
            payLeaveButton.interactable = false;
            payLeaveButton.gameObject.SetActive(false);
        }
        speechText.text = "It's a great choice!\n\nGood luck and see you soon!";

        if (CashRegisterGlow.Instance != null)
        {
            CashRegisterGlow.Instance.StopBreathing();
        }

        yield return new WaitForSeconds(3.0f);

        yield return StartCoroutine(FadeOutUIRoutine(3.0f));

        // === SEQUENTIAL ITEM PICKUP ===
        yield return StartCoroutine(TakeItemsSequentiallyRoutine());

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

    private IEnumerator TakeItemsSequentiallyRoutine()
    {
        if (counterItems.Count == 0) yield break;

        float moveDuration = 0.6f;      
        
        // 1. Calculate the absolute position of Parking Slot 0 (the leftmost space)
        Vector3 slot0Pos = counterItems[0].transform.position; // Default fallback
        ItemClick itemLogic = counterItems[0].GetComponent<ItemClick>();
        
        if (itemLogic != null && itemLogic.counterTop != null)
        {
            // We reconstruct the exact coordinate of Slot 0 dynamically
            slot0Pos = new Vector3(
                itemLogic.counterTop.position.x, // Slot 0 has no (availableSlot * 0.9f) offset
                itemLogic.counterTop.position.y + 0.5f + itemLogic.positionOffset.y,
                itemLogic.counterTop.position.z + itemLogic.positionOffset.z
            );
        }

        // 2. Calculate the SINGLE common target point for ALL items based on Slot 0
        Camera mainCam = Camera.main;
        Vector3 commonTargetPos = slot0Pos - mainCam.transform.up * 1.2f - mainCam.transform.forward * 2.5f;

        // 3. Move items strictly one after another towards the common target
        for (int i = 0; i < counterItems.Count; i++)
        {
            GameObject item = counterItems[i];
            if (item != null)
            {
                // yield return StartCoroutine forces the loop to wait until this specific item's animation is 100% finished
                yield return StartCoroutine(AnimateItemTake(item, moveDuration, commonTargetPos));
            }
        }
    }

    // UPDATED TRAJECTORY ANIMATION WITH FIXED COMMON TARGET
    private IEnumerator AnimateItemTake(GameObject item, float duration, Vector3 targetPos)
    {
        if (item == null) yield break;

        Vector3 startPos = item.transform.position;
        Vector3 startScale = item.transform.localScale;
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (item == null) break;
            
            float t = elapsed / duration;
            
            // SmoothStep easing for premium game feel
            float smoothT = Mathf.SmoothStep(0f, 1f, t); 
            
            // Apply position transition to the fixed common targetPos
            item.transform.position = Vector3.Lerp(startPos, targetPos, smoothT);
            
            // Fine-tuned 1.15x scale to avoid breaking immersion
            float scaleMultiplier = Mathf.Lerp(1f, 1.15f, smoothT);
            item.transform.localScale = startScale * scaleMultiplier;
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (item != null)
        {
            item.SetActive(false); 
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
            payLeaveButton.gameObject.SetActive(true); 
        }
    }
}