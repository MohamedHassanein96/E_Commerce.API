namespace E_Commerce.Services
{
    public class ImageService(ApplicationDbContext context , IWebHostEnvironment webHostEnvironment) : IImageService
    {
        private readonly string _imagesPath = $"{webHostEnvironment.WebRootPath}/images";
        private readonly ApplicationDbContext _context = context;

        public async Task UploadManyAsync(int productId , IFormFileCollection images, CancellationToken cancellationToken = default)
        {

            List<ProductImage> productImages = [];

            foreach (var image in images)
            {
                var randomImageName = Path.GetRandomFileName();

                var productImage = new ProductImage
                {
                     ImageName = image.Name,
                     ContentType = image.ContentType,
                     ImageExtension = Path.GetExtension(image.FileName)
                };

                var path = Path.Combine(_imagesPath, image.FileName);

                using var stream = File.Create(path);
                await image.CopyToAsync(stream, cancellationToken);

                productImage.ProductId = productId;

                productImages.Add(productImage);
            }
            await _context.AddRangeAsync(productImages, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
