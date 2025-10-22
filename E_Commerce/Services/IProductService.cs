using OneOf;

namespace E_Commerce.Services
{
    public interface IProductService
    {
        Task<ProductResponse> AddAsync( int categoryId, ProductRequest request, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(int categoryId,int productId, UpdateProductRequest request, CancellationToken cancellationToken = default);
        Task<ProductResponse> GetAsync(int categoryId,int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductResponse>> GetAllAsync( int categoryId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<OneOf<bool, ErrorResponse>> ToggleStatus(int id, ProductHighlightType type, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductResponse>> GetByHighlightTypeAsync(ProductHighlightType type, CancellationToken cancellationToken = default);

    }
}
