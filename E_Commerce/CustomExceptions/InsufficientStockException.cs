namespace E_Commerce.CustomExceptions
{
    public class InsufficientStockException :Exception
    {
        public string ProductName { get; }
        public int RequestedQuantity { get; }
        public int AvailableQuantity { get; }

        public InsufficientStockException(string productName, int requestedQuantity, int availableQuantity)
         : base($"Product'{productName}' RequestedQuantity({requestedQuantity}) 'AvailableQuantity'({availableQuantity}) ")
        {
            ProductName = productName;
            RequestedQuantity = requestedQuantity;
            AvailableQuantity = availableQuantity;
        }
    }
}
