namespace E_Commerce.Services
{
    public class ProductService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment) : IProductService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly string _imagesPath = $"{webHostEnvironment.WebRootPath}/images";

        public async Task<ProductResponse> AddAsync(int categoryId, ProductRequest request, CancellationToken cancellationToken = default)
        {
            var isFound = await _context.Categories.AnyAsync(x => x.Id == categoryId , cancellationToken);
            if (!isFound)
                return null!;

            var product = request.Adapt<Product>();
            product.CategoryId = categoryId;
    
            List<ProductImage> productImages = [];

            if (!Directory.Exists(_imagesPath))
            {
                Directory.CreateDirectory(_imagesPath);
            }

            foreach (var image in request.Images)
            {
                var uniqueFileName = $"{Guid.CreateVersion7()}{Path.GetExtension(image.FileName)}";
                var path = Path.Combine(_imagesPath, uniqueFileName);


                var productImage = new ProductImage
                {
                    ImageName = uniqueFileName,
                    ContentType = image.ContentType,
                    ImageExtension = Path.GetExtension(image.FileName)
                };

                using var stream = File.Create(path);
                await image.CopyToAsync(stream, cancellationToken);

                productImages.Add(productImage);

            }
            product.ProductImages = productImages;
            await _context.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);


            return product.Adapt<ProductResponse>();
        }

        public async Task<IEnumerable<ProductResponse>> GetAllAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Products.Where(X => X.CategoryId == categoryId).ProjectToType<ProductResponse>()
                .AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<ProductResponse> GetAsync(int categoryId, int productId, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId && x.CategoryId == categoryId , cancellationToken);
            if (product is null)
            {
                return null!;
            }
            return product.Adapt<ProductResponse>();
        }

        public async Task<bool> UpdateAsync( int categoryId, int productId, UpdateProductRequest request, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == productId && x.CategoryId == categoryId , cancellationToken);

            if (product is null)
                return false;

            if (request.Images is not null && request.Images.Count > 0)
            {
                _context.ProductImages.RemoveRange(product.ProductImages);
                var productImages = new List<ProductImage>();

                foreach (var image in request.Images!)
                {
                    var uniqueFileName = $"{Guid.CreateVersion7()}{Path.GetExtension(image.FileName)}";
                    var path = Path.Combine(_imagesPath, uniqueFileName);

                    using var stream = File.Create(path);
                    await image.CopyToAsync(stream, cancellationToken);

                    var productImage = new ProductImage
                    {
                        ImageName = uniqueFileName,
                        ContentType = image.ContentType,
                        ImageExtension = Path.GetExtension(uniqueFileName),
                        ProductId = product.Id
                    };

                    productImages.Add(productImage);
                }
                await _context.ProductImages.AddRangeAsync(productImages);

            }


            product = request.Adapt(product);
            product.Version += 1;

            _context.Products.Update(product);
            
           
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products.Include(c => c.ProductImages).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (product is null)
                return false;

            _context.ProductImages.RemoveRange(product.ProductImages);

            _context.Products.Remove(product);

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

    }
}
