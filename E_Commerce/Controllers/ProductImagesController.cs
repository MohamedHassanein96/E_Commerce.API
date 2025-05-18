namespace E_Commerce.Controllers
{
    [Route("api/{productId}/[controller]")]
    [ApiController]
    public class ProductImagesController(IImageService imageService) : ControllerBase
    {
        private readonly IImageService _imageService = imageService;

        [HttpPost("")]
        public async Task <IActionResult> UploadProductImages([FromRoute] int productId, [FromForm] UploadImagesRequest request , CancellationToken cancellationToken )
        {
            await _imageService.UploadManyAsync(productId, request.Images , cancellationToken);
            return Created();
        }
    }
}
