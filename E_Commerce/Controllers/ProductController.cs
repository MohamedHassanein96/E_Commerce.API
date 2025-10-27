using E_Commerce.Extension;
using Microsoft.AspNetCore.Authorization;

namespace E_Commerce.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductController(IProductService _productService) : ControllerBase
{

    [HttpPost("")]
    public async Task<IActionResult> Add([FromQuery] int categoryId,[FromForm] ProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _productService.AddAsync(categoryId, request, cancellationToken);
        return result.Match<IActionResult>(
            success => CreatedAtAction(nameof(Get), new { id = success.Id, categoryId }, success),
            error => NotFound(error.Message)
            );
    }
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _productService.GetAsync(id, cancellationToken);
        return result.Match<IActionResult>(
            success => Ok(success),
            error => NotFound(error.Message)
            );
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var isUpdated = await _productService.UpdateAsync(id, request, cancellationToken);
        if (isUpdated)
            return NoContent();
        return NotFound();
    }


    [AllowAnonymous]
    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromQuery] int categoryId, CancellationToken cancellationToken)
    {
        return Ok(await _productService.GetAllAsync(categoryId, cancellationToken));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id ,CancellationToken cancellationToken)
    {
        var isDeleted = await _productService.DeleteAsync(id, User.GetUserId()!, cancellationToken);
        if (isDeleted)
            return NoContent();
        else
            return NotFound();
    }
    [HttpGet("highlight/{type}")]
    public async Task<IActionResult> GetHighlightedProducts([FromQuery]int categoryId, [FromRoute] ProductHighlightType type, CancellationToken cancellationToken)
    {
        var products = await _productService.GetByHighlightTypeAsync(categoryId, type, cancellationToken);

        return Ok(products);
    }

    [HttpPut("{id}/highlight/{type}")]
    public async Task<IActionResult> UpdateHighlightType([FromRoute] int id, [FromRoute] ProductHighlightType type)
    {
        var isUpdated = await _productService.ToggleStatus(id, type);
        if (isUpdated)
            return NoContent();
        else
            return NotFound();
    }

}
