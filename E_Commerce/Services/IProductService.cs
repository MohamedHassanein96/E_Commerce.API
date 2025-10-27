using OneOf;

namespace E_Commerce.Services
{
    public interface IProductService
    {
        Task<OneOf<ProductResponse, ErrorResponse>> AddAsync( int categoryId, ProductRequest request, CancellationToken cancellationToken = default);
        Task<OneOf<ProductResponse, ErrorResponse>> GetAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductResponse>> GetAllAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(int productId, UpdateProductRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id,string userId ,CancellationToken cancellationToken = default);
        Task<bool> ToggleStatus(int id, ProductHighlightType type, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductResponse>> GetByHighlightTypeAsync(int categoryId,ProductHighlightType type, CancellationToken cancellationToken = default);

    }
}
