namespace E_Commerce.Services
{
    public class CategoryService(ApplicationDbContext context) : ICategoryService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<CategoryResponse> AddAsync(CategoryRequest request,CancellationToken cancellationToken = default)
        {

            var category = request.Adapt<Category>();
            await _context.Categories.AddAsync(category, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return category.Adapt<CategoryResponse>();
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories.ProjectToType<CategoryResponse>().AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<CategoryResponse> GetAsync(int id, CancellationToken cancellationToken = default)
        {
           var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (category  is null)
            {
                return null!;
            }
            return category.Adapt<CategoryResponse>();
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (category is null)
                return false;

            category = request.Adapt(category);
            _context.Categories.Update(category);
            await _context.SaveChangesAsync(cancellationToken);
            return true;

        }
        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var category = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (category is null)
                return false;


            foreach (var product in category.Products)
            {
                _context.Remove(product);
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
