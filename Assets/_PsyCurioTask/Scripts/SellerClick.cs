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

    void OnMouseDown()
    {
        if (!isWaving)
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
        animator.SetBool("IsWaving", false);
        Debug.Log("Seller stopped waving!");
    }
}