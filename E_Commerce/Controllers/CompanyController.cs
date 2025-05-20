namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController(IComapnyService comapnyService) : ControllerBase
    {
        private readonly IComapnyService _comapnyService = comapnyService;

        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] CompanyRequest request , CancellationToken cancellationToken)
        {
            var companyResponse = await _comapnyService.AddAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Get), new {id= companyResponse.Id},companyResponse);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute]int id, CancellationToken cancellationToken)
        {
            var companyResponse = await _comapnyService.GetAsync(id, cancellationToken);
            if (companyResponse is null)
            {
                return NotFound();
            }
            return Ok(companyResponse);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id,[FromBody]UpdateCompanyRequest request, CancellationToken cancellationToken)
        {
           var isUpdated= await _comapnyService.UpdateAsync(id, request ,cancellationToken);
            if (isUpdated)
                return NoContent();
            else
                return NotFound();
        }
        [HttpGet("")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var companyResponses = await _comapnyService.GetAllAsync(cancellationToken);
                return Ok(companyResponses);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id,CancellationToken cancellationToken)
        {
            var isDeleted = await _comapnyService.DeleteAsync(id,cancellationToken);
            if (isDeleted)
                return NoContent();
            else
                return NotFound();
        }
    }
}
