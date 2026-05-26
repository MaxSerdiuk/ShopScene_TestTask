using UnityEngine;

public class SellerClick : MonoBehaviour
{
    private Animator animator;
    private bool isWaving = false;

    void Start()
    {
        // Отримуємо компонент Animator з дочірніх об'єктів
        animator = GetComponentInChildren<Animator>();
    }

    // Клік мишкою безпосередньо по продавчині
    void OnMouseDown()
    {
        Wave();
    }

    // ПУБЛІЧНИЙ МЕТОД: тепер його може викликати і каса CashRegisterClick
    public void Wave()
    {
        if (!isWaving && animator != null)
        {
            // Запускаємо анімацію хвилі
            isWaving = true;
            animator.SetBool("IsWaving", true);

            // Діагностика - чи Animator отримав команду
            Debug.Log("IsWaving set to: " + animator.GetBool("IsWaving"));

            // Повертаємось до Idle через 3 секунди
            Invoke("StopWaving", 3f);

            Debug.Log("Seller is waving!");
        }
    }

    void StopWaving()
    {
        // Повертаємось до Idle
        isWaving = false;
        if (animator != null)
        {
            animator.SetBool("IsWaving", false);
        }
        Debug.Log("Seller stopped waving!");
    }
}