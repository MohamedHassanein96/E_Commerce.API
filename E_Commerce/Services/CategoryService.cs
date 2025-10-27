using OneOf;

namespace E_Commerce.Services
{
    public class CategoryService(ApplicationDbContext _context) : ICategoryService
    {
        public async Task<OneOf<CategoryResponse , ErrorResponse>> AddAsync(CategoryRequest request,CancellationToken cancellationToken = default)
        {

            var isExistedName = await _context.Categories.AnyAsync(c => c.Name == request.Name, cancellationToken);
            if (isExistedName)
                return new ErrorResponse("another category with the same name is existed",StatusCodes.Status409Conflict);


            var category = request.Adapt<Category>();
            await _context.Categories.AddAsync(category, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return category.Adapt<CategoryResponse>();
        }
        public async Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories.ProjectToType<CategoryResponse>().AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
        }
        public async Task<OneOf<CategoryResponse, ErrorResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
        {
           var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (category  is null)
                return new ErrorResponse("Category is not found",StatusCodes.Status404NotFound);
            
            return category.Adapt<CategoryResponse>();
        }
        public async Task<OneOf<bool,ErrorResponse>> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
        {

            var isExistedCategoryName = await _context.Categories.AnyAsync(x => x.Name == request.Name && x.Id != id , cancellationToken);
            if (isExistedCategoryName)
                return new ErrorResponse("another category with the same name is existed", StatusCodes.Status409Conflict);


            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id,cancellationToken);
            if (category is null)
                return new ErrorResponse("category not found", StatusCodes.Status404NotFound);



            category = request.Adapt(category);
            _context.Categories.Update(category!);
            await _context.SaveChangesAsync(cancellationToken);
            return true;

        }
        public async Task<OneOf<bool, ErrorResponse>> DeleteAsync(int id,string userId,CancellationToken cancellationToken = default)
        {
            var isExisted = await _context.Users.AnyAsync(x => x.Id == userId, cancellationToken);
            if (!isExisted)
                return new ErrorResponse("Invalid User", StatusCodes.Status401Unauthorized);

            var exists = await _context.Categories.AnyAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
            if (!exists)
                return new ErrorResponse("Category Not Found",StatusCodes.Status404NotFound);

            await _context.Products
                .Where(p => p.CategoryId == id && !p.IsDeleted)
                .ExecuteUpdateAsync(p => p
                .SetProperty(x => x.IsDeleted, true)
                .SetProperty(x => x.DateDeleted, DateTime.UtcNow)
                .SetProperty(x => x.DeletedBy, userId),
                cancellationToken);

            await _context.Categories
                .Where(c => c.Id == id && !c.IsDeleted)
                .ExecuteUpdateAsync(c => c
                    .SetProperty(x => x.IsDeleted, true)
                    .SetProperty(x => x.DateDeleted, DateTime.UtcNow)
                    .SetProperty(x => x.DeletedBy, userId),
                    cancellationToken);

            return true;
        }

       
    }
}
