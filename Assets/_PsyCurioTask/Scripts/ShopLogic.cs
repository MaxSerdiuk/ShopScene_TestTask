public class ShopLogic
{
    // Максимальна кількість товарів на прилавку
    public int MaxItems { get; private set; }

    // Поточна кількість товарів на прилавку
    public int CurrentItemCount { get; private set; }

    // Загальна ціна товарів на прилавку
    public float TotalPrice { get; private set; }

    public ShopLogic(int maxItems = 5)
    {
        MaxItems = maxItems;
        CurrentItemCount = 0;
        TotalPrice = 0f;
    }

    // Додати товар на прилавок з ціною
    // Повертає true якщо товар додано, false якщо прилавок повний
    public bool TryAddItem(float price)
    {
        if (CurrentItemCount >= MaxItems)
            return false;

        CurrentItemCount++;
        TotalPrice += price;
        return true;
    }

    // Очистити прилавок після checkout
    public void Checkout()
    {
        CurrentItemCount = 0;
        TotalPrice = 0f;
    }
}