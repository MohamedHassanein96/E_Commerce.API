namespace E_Commerce.Controllers
{
    [Route("api/company/{companyId}/category/{categoryId}/[controller]")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        [HttpPost("")]
        public async Task<IActionResult> Add([FromRoute] int companyId, [FromRoute] int categoryId,[FromForm] IFormFileCollection images, [FromForm] ProductRequest request, CancellationToken cancellationToken)
        {
            var productResponse = await _productService.AddAsync(companyId,categoryId,request,images ,cancellationToken);
            if (productResponse is null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(Get), new {productResponse.Id, companyId, categoryId},productResponse);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int companyId, [FromRoute] int categoryId, [FromRoute] int id ,[FromForm] UpdateProductRequest request, [FromForm] IFormFileCollection images, CancellationToken cancellationToken)
        {
            var productIsUpdated = await _productService.UpdateAsync(companyId, categoryId, id ,request,images ,cancellationToken);
            if (!productIsUpdated)
            {
                return NotFound();
            }
            return Ok();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int companyId, [FromRoute] int categoryId, [FromRoute] int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetAsync(companyId, categoryId, id, cancellationToken);
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromRoute] int companyId, [FromRoute] int categoryId, CancellationToken cancellationToken)
        {
            return Ok(await _productService.GetAllAsync( companyId, categoryId ,cancellationToken));
        }
    }
}
