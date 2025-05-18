namespace E_Commerce.Services
{
    public interface IImageService
    {
        Task UploadManyAsync(int productId, IFormFileCollection images, CancellationToken cancellationToken = default);
       
    }

}
