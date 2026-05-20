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
}