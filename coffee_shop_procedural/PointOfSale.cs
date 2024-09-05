namespace coffee_shop_procedural;

public class PointOfSale
{
    public void CalculateTotal(List<int> orderItems, List<decimal> menuPrices, string currentUser)
    {
        decimal total = 0;
        foreach (int itemIndex in orderItems)
        {
            total += menuPrices[itemIndex];
        }
        Console.WriteLine($"Total Amount Payable by {currentUser}: {total:C}");
    }
}
