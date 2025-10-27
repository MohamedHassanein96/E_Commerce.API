namespace E_Commerce.Contracts.Product
{
    public record ProductResponse(int Id,string Name, string Description, decimal Price, int AvailableStock, ProductHighlightType HighlightType  , List<ProductImageResponse> Images); 

    public record ProductImageResponse(string ImageName, string Url);

}
