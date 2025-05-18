namespace E_Commerce.Services
{
    public interface ICategoryService
    {
        Task<CategoryResponse> AddAsync(CategoryRequest request, CancellationToken cancellationToken = default);
        Task<CategoryResponse> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryResponse>> GetAllAsync( CancellationToken cancellationToken = default);
    }
}
