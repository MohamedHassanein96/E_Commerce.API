using OneOf;
using System.Linq;

namespace E_Commerce.Services;
public class ProductService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor) : IProductService
{
    private readonly ApplicationDbContext _context = context;
    private readonly string _imagesPath = $"{webHostEnvironment.WebRootPath}/images";
    private readonly string _baseUrl = $"{httpContextAccessor.HttpContext!.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";

    public async Task<OneOf<ProductResponse, ErrorResponse>> AddAsync(int categoryId, ProductRequest request, CancellationToken cancellationToken = default)
    {
        var isFound = await _context.Categories.AnyAsync(x => x.Id == categoryId, cancellationToken);
        if (!isFound)
            return new ErrorResponse("Category Not Found", StatusCodes.Status404NotFound);

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

        var response = new ProductResponse(
                product.Id,
                product.Name,
                product.Description!,
                product.Price,
                product.AvailableStock,
                product.HighlightType,
                product.ProductImages.Select(img => new ProductImageResponse(
                    img.ImageName,
        $"{_baseUrl}/images/{img.ImageName}"
    )).ToList()
);

        return response;
    }
    public async Task<OneOf<ProductResponse, ErrorResponse>> GetAsync( int productId, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .Where(p => p.Id == productId)
            .Include(p => p.ProductImages).FirstOrDefaultAsync(cancellationToken);

        if (product is null)
            return new ErrorResponse("item Not Found", StatusCodes.Status404NotFound);


        var response = new ProductResponse(
         product.Id,
         product.Name,
         product.Description!,
         product.Price,
         product.AvailableStock,
         product.HighlightType,
         product.ProductImages.Select(img => new ProductImageResponse(
                      img.ImageName,
                      $"{_baseUrl}/images/{img.ImageName}"
                     )).ToList()
        );


        return response;

    }
    public async Task<IEnumerable<ProductResponse>> GetAllAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Products.Where(X => X.CategoryId == categoryId)
            .Include(x => x.ProductImages)
            .Select(p => new ProductResponse(
                  p.Id,
                 p.Name,
                 p.Description!,
                 p.Price,
                 p.AvailableStock,
                 p.HighlightType,
                 p.ProductImages.Select(img => new ProductImageResponse(
                      img.ImageName,
                      $"{_baseUrl}/images/{img.ImageName}"
                     )).ToList()
                ))
            .AsNoTracking().ToListAsync(cancellationToken);
    }
    public async Task<bool> UpdateAsync( int productId, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {

        var product = await _context.Products.Where(p => p.Id == productId)
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
            return false;


        if (request.Images is not null && request.Images.Count > 0)
        {

            // 🧹 احذف الصور القديمة من الفولدر
            foreach (var oldImage in product.ProductImages)
            {
                var oldPath = Path.Combine(_imagesPath, oldImage.ImageName);
                if (File.Exists(oldPath))
                    File.Delete(oldPath);
            }

            // 🗑️ احذف الصور القديمة من قاعدة البيانات
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
            await _context.ProductImages.AddRangeAsync(productImages, cancellationToken);

        }


        product = request.Adapt(product);
        product.Version += 1;

        _context.Products.Update(product);


        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
    public async Task<bool> DeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.Include(c => c.ProductImages).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (product is null)
            return false;
        // 🧹 احذف الصور القديمة من الفولدر
        foreach (var oldImage in product.ProductImages)
        {
            var oldPath = Path.Combine(_imagesPath, oldImage.ImageName);
            if (File.Exists(oldPath))
                File.Delete(oldPath);
        }


        _context.ProductImages.RemoveRange(product.ProductImages);

        product.IsDeleted = true;
        product.DateDeleted = DateTime.UtcNow;
        product.DeletedBy = userId;

        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<IEnumerable<ProductResponse>> GetByHighlightTypeAsync(int categoryId, ProductHighlightType type, CancellationToken cancellationToken = default)
    {
        return await _context.Products
                                     .Where(p => p.CategoryId == categoryId && p.HighlightType == type)
                                     .ProjectToType<ProductResponse>()
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);

    }
    public async Task<bool> ToggleStatus(int id, ProductHighlightType type, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FindAsync(id, cancellationToken);
        if (product is null)
            return false;

        product.HighlightType = type;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }


}
