using OneOf;namespace E_Commerce.Services
{
    public interface ICategoryService
    {
        Task<OneOf<CategoryResponse, ErrorResponse>> AddAsync(CategoryRequest request, CancellationToken cancellationToken = default);
        Task<OneOf<CategoryResponse,ErrorResponse>> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<OneOf<bool, ErrorResponse>> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryResponse>> GetAllAsync( CancellationToken cancellationToken = default);
        Task<OneOf<bool,ErrorResponse>> DeleteAsync(int id, string userId, CancellationToken cancellationToken = default);
    }
}
