using NUnit.Framework;

public class ShopLogicTests
{
    [Test]
    public void CounterIncrementsOnAdd()
    {
        // Arrange
        ShopLogic shop = new ShopLogic();

        // Act
        shop.TryAddItem(3f);

        // Assert
        Assert.AreEqual(1, shop.CurrentItemCount);
    }

    [Test]
    public void CounterDoesNotExceedMax()
    {
        // Arrange
        ShopLogic shop = new ShopLogic();

        // Act
        for (int i = 0; i < 6; i++)
        {
            shop.TryAddItem(3f);
        }

        // Assert
        Assert.AreEqual(5, shop.CurrentItemCount);
    }

    [Test]
    public void TotalPriceForAllItems()
    {
        // Arrange
        ShopLogic shop = new ShopLogic();

        // Act
        shop.TryAddItem(10f); // Crown
        shop.TryAddItem(3f);  // LaughingMask
        shop.TryAddItem(15f); // MagicWand
        shop.TryAddItem(3f);  // Sword
        shop.TryAddItem(7f);  // Mirror

        // Assert
        Assert.AreEqual(38f, shop.TotalPrice);
    }

    [Test]
    public void CheckoutClearsCounter()
    {
        // Arrange
        ShopLogic shop = new ShopLogic();
        shop.TryAddItem(10f);
        shop.TryAddItem(3f);

        // Act
        shop.Checkout();

        // Assert
        Assert.AreEqual(0, shop.CurrentItemCount);
        Assert.AreEqual(0f, shop.TotalPrice);
    }

    [Test]
    public void CanAddItemsAfterCheckout()
    {
        // Arrange
        ShopLogic shop = new ShopLogic();

        for (int i = 0; i < 5; i++)
        {
            shop.TryAddItem(10f);
        }

        shop.Checkout();

        // Act
        bool itemAdded = shop.TryAddItem(10f);

        // Assert
        Assert.IsTrue(itemAdded);
        Assert.AreEqual(1, shop.CurrentItemCount);
        Assert.AreEqual(10f, shop.TotalPrice);
    }
}