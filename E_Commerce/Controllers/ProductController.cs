namespace E_Commerce.Controllers
{
    [Route("api/category/{categoryId}/[controller]")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        [HttpPost("")]
        public async Task<IActionResult> Add( [FromRoute] int categoryId, [FromForm] ProductRequest request, CancellationToken cancellationToken)
        {
            var productResponse = await _productService.AddAsync(categoryId,request, cancellationToken);
            if (productResponse is null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(Get), new { id = productResponse.Id, categoryId},productResponse);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update( [FromRoute] int categoryId, [FromRoute] int id ,[FromForm]  UpdateProductRequest request, CancellationToken cancellationToken)
        {
            var productIsUpdated = await _productService.UpdateAsync( categoryId, id ,request, cancellationToken);
            if (!productIsUpdated)
            {
                return NotFound();
            }
            return Ok();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get( [FromRoute] int categoryId, [FromRoute] int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetAsync( categoryId, id, cancellationToken);
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromRoute] int categoryId, CancellationToken cancellationToken)
        {
            return Ok(await _productService.GetAllAsync(categoryId, cancellationToken));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var isDeleted = await _productService.DeleteAsync(id, cancellationToken);
            if (isDeleted)
                return NoContent();
            else
                return NotFound();
        }
    }
}
