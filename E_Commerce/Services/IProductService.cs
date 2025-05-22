namespace E_Commerce.Services
{
    public interface IProductService
    {
        Task<ProductResponse> AddAsync( int categoryId, ProductRequest request, UploadImagesRequest requestImages, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(int categoryId,int productId, UpdateProductRequest request, CancellationToken cancellationToken = default);
        Task<ProductResponse> GetAsync(int categoryId,int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductResponse>> GetAllAsync( int categoryId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
