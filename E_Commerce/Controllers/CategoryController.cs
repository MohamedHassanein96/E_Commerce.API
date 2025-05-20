namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;

        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] CategoryRequest request, CancellationToken cancellationToken)
        {

            var categoryResponse = await _categoryService.AddAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { categoryResponse.Id }, categoryResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id , CancellationToken cancellationToken)
        {

            var categoryRespponse = await _categoryService.GetAsync(id, cancellationToken);
            if (categoryRespponse is null)
            {
                return NotFound();
            }
            return Ok(categoryRespponse);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCategoryRequest request ,CancellationToken cancellationToken)
        {

            var categoryIsUpdated = await _categoryService.UpdateAsync(id, request, cancellationToken);
            if (categoryIsUpdated)
            {
                return NoContent ();
            }
            return NotFound();
        }
        [HttpGet("")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var categoryResponses = await _categoryService.GetAllAsync(cancellationToken);
            return Ok(categoryResponses);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var isDeleted = await _categoryService.DeleteAsync(id, cancellationToken);
            if (isDeleted)
                return NoContent();
            else
                return NotFound();
        }
    }
}
