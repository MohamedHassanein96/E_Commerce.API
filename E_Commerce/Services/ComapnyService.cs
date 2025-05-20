namespace E_Commerce.Services
{
    public class ComapnyService(ApplicationDbContext context) : IComapnyService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<CompanyResponse> AddAsync(CompanyRequest request, CancellationToken cancellationToken = default)
        {
            var company = request.Adapt<Company>();

            await _context.AddAsync(company, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return (company.Adapt<CompanyResponse>());
        }
        public async Task<IEnumerable<CompanyResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
          return  await _context.Companies.ProjectToType<CompanyResponse>().AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<CompanyResponse> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
            return company is null ? null! : company.Adapt<CompanyResponse>();

        }
        public async Task<bool> UpdateAsync(int id, UpdateCompanyRequest request , CancellationToken cancellationToken = default)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
            if (company is null)
                return false ;

            
            company = request.Adapt(company);
            _context.Update(company);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var company = await _context.Companies.Include(c => c.Categories).ThenInclude(c => c.Products).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (company is null)
                return false;


            foreach (var category in company.Categories)
            {
                foreach (var product in category.Products)
                {

                    _context.Products.Remove(product);
                }
                _context.Categories.Remove(category);
            }
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

     
    }
}
