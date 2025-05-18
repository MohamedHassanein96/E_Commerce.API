namespace E_Commerce.Services
{
    public class ProductService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment) : IProductService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly string _imagesPath = $"{webHostEnvironment.WebRootPath}/images";

        public async Task<ProductResponse> AddAsync(int companyId, int categoryId, ProductRequest request, IFormFileCollection images, CancellationToken cancellationToken = default)
        {

            var category = await _context.Categories.AnyAsync(x => x.Id == categoryId, cancellationToken);
            var company = await _context.Companies.AnyAsync(x => x.Id == companyId, cancellationToken);
            if (!category || !company)
                return null!;

            var product = request.Adapt<Product>();
            product.CategoryId = categoryId;
            product.CompanyId = companyId;



            List<ProductImage> productImages = [];

            foreach (var image in images)
            {

                var productImage = new ProductImage
                {
                    ImageName = image.FileName,
                    ContentType = image.ContentType,
                    ImageExtension = Path.GetExtension(image.FileName)
                };

                var path = Path.Combine(_imagesPath, image.FileName);

                using var stream = File.Create(path);
                await image.CopyToAsync(stream, cancellationToken);

                productImages.Add(productImage);

            }
            product.ProductImages = productImages;
            await _context.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);


            return product.Adapt<ProductResponse>();
        }

        public async Task<IEnumerable<ProductResponse>> GetAllAsync(int companyId, int categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Products.Where(X => X.CategoryId == categoryId && X.CompanyId == companyId).ProjectToType<ProductResponse>()
                .AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<ProductResponse> GetAsync(int companyId, int categoryId, int productId, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId && x.CategoryId == categoryId && x.CompanyId == companyId, cancellationToken);
            if (product is null)
            {
                return null!;
            }
            return product.Adapt<ProductResponse>();
        }

        public async Task<bool> UpdateAsync(int companyId, int categoryId, int productId, UpdateProductRequest request, IFormFileCollection images, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == productId && x.CategoryId == categoryId && x.CompanyId == companyId, cancellationToken);

            if (product is null)
                return false;

         
            _context.ProductImages.RemoveRange(product.ProductImages);

        
            var productImages = new List<ProductImage>();
            foreach (var image in images)
            {
                var imageName = image.FileName;
                var path = Path.Combine(_imagesPath, imageName);

                using var stream = File.Create(path);
                await image.CopyToAsync(stream, cancellationToken);

                var productImage = new ProductImage
                {
                    ImageName = imageName,
                    ContentType = image.ContentType,
                    ImageExtension = Path.GetExtension(imageName),
                    ProductId = product.Id 
                };

                productImages.Add(productImage);
            }

            _context.ProductImages.AddRange(productImages);


            product = request.Adapt(product);
            product.Version += 1;

            _context.Products.Update(product);
            
           
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }


    }
}
