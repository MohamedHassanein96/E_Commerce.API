namespace E_Commerce.Contracts.Product
{
    public record UpdateProductRequest(string Name, string Description, decimal Price, int Quantity );
    
}
