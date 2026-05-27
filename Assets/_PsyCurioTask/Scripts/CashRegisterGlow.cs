using UnityEngine;
using System.Collections;

public class CashRegisterGlow : MonoBehaviour
{
    [Header("Світло (Перетягни сюди Point Light)")]
    public Light breathingLight;

    [Header("Налаштування дихання")]
    public float minIntensity = 0.8f;   // Мінімальна яскравість (на спаді)
    public float maxIntensity = 2.5f;   // Максимальна яскравість (на піку)
    public float breathDuration = 3f;   // Тривалість одного вдиху-видиху (в секундах)
    public float fadeDuration = 0.5f;   // Швидкість плавного вмикання/вимикання

    // Singleton для швидкого доступу з інших скриптів
    public static CashRegisterGlow Instance { get; private set; }

    private bool isBreathing = false;
    private Coroutine breathRoutine;
    private Coroutine fadeRoutine;

    void Awake()
    {
        // Безпечна ініціалізація із захистом від дублікатів
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return; // Миттєво зупиняємо виконання коду для дубліката
        }
    }

    void Start()
    {
        // Захист: перевірка чи не забули перетягнути світло в інспекторі
        if (breathingLight == null)
        {
            Debug.LogWarning("CashRegisterGlow: Світло (Light) не призначено в Inspector!");
            return;
        }

        // На старті гри світло гарантовано вимкнено
        breathingLight.enabled = false;
        breathingLight.intensity = 0f;
    }

    // Головний керуючий метод, який викликається при зміні кількості товарів
    public void UpdateGlowState(int itemCount)
    {
        // Світимось ТІЛЬКИ якщо на столі від 1 до 5 товарів
        if (itemCount >= 1 && itemCount <= 5)
        {
            StartBreathing();
        }
        else
        {
            StopBreathing();
        }
    }

    private void StartBreathing()
    {
        if (isBreathing || breathingLight == null) return;
        
        isBreathing = true;
        breathingLight.enabled = true;

        // Перериваємо поточне згасання, якщо воно тривало, і плавно вмикаємо
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeInThenBreathe());
    }

    // Публічний метод для примусового вимкнення світла під час Checkout
    public void StopBreathing()
    {
        if (!isBreathing || breathingLight == null) return;
        
        isBreathing = false;

        // Зупиняємо анімацію дихання та плавного вмикання, запускаємо повне згасання
        if (breathRoutine != null) StopCoroutine(breathRoutine);
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        
        fadeRoutine = StartCoroutine(FadeOut());
    }

    private IEnumerator FadeInThenBreathe()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            breathingLight.intensity = Mathf.Lerp(0f, minIntensity, timer / fadeDuration);
            yield return null;
        }
        
        // Після повного входу у мінімальну яскравість запускаємо циклічне дихання
        breathRoutine = StartCoroutine(BreathingLoop());
    }

    private IEnumerator FadeOut()
    {
        float startIntensity = breathingLight.intensity;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            breathingLight.intensity = Mathf.Lerp(startIntensity, 0f, timer / fadeDuration);
            yield return null;
        }
        
        breathingLight.intensity = 0f;
        breathingLight.enabled = false;
    }

    private IEnumerator BreathingLoop()
    {
        float timer = 0f;
        while (isBreathing)
        {
            timer += Time.deltaTime;
            // Математична синусоїда для створення ефекту плавного руху хвилі світла
            float t = (Mathf.Sin(timer * Mathf.PI * 2f / breathDuration) + 1f) / 2f;
            breathingLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
            yield return null;
        }
    }
}