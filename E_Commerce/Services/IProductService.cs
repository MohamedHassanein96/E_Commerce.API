namespace E_Commerce.Services
{
    public interface IProductService
    {
        Task<ProductResponse> AddAsync( int categoryId, ProductRequest request, IFormFileCollection images, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(int companyId, int categoryId,int productId, UpdateProductRequest request, IFormFileCollection images, CancellationToken cancellationToken = default);
        Task<ProductResponse> GetAsync(int companyId, int categoryId,int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductResponse>> GetAllAsync(int companyId, int categoryId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
