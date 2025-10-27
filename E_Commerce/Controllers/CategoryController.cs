using E_Commerce.Extension;
using Microsoft.AspNetCore.Authorization;
using OneOf.Types;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryService _categoryService) : ControllerBase
    {
        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> Add( [FromBody] CategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _categoryService.AddAsync(request, cancellationToken);

            return result.Match<IActionResult>(
                success=> CreatedAtAction(nameof(Get), new { success.Id }, success),
                error=> Conflict(error.Message));
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll( CancellationToken cancellationToken)
        {
            var categoryResponses = await _categoryService.GetAllAsync(cancellationToken);
            return Ok(categoryResponses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id , CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetAsync(id, cancellationToken);
            return result.Match<IActionResult>(
               success => Ok(success),
               error => NotFound(error.Message));
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCategoryRequest request ,CancellationToken cancellationToken)
        {
            var result = await _categoryService.UpdateAsync(id,request, cancellationToken);

            return result.Match<IActionResult>(
                success => NoContent(),
                error => error.StatusCode switch
                {
                    StatusCodes.Status404NotFound => NotFound(error.Message),
                    StatusCodes.Status409Conflict => Conflict(error.Message),
                    _ => BadRequest(),
                });
        }
      
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _categoryService.DeleteAsync(id, User.GetUserId()!, cancellationToken);

            return result.Match<IActionResult>(
              success => NoContent(),
                error => error.StatusCode switch
                {
                    StatusCodes.Status404NotFound => NotFound(error.Message),
                    StatusCodes.Status401Unauthorized => Unauthorized(error.Message),
                    _ => BadRequest(),
                });
        }
    }
}
