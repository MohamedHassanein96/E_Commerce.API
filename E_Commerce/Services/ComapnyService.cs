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
            if (company is null) 
                return null!;

            
            return company.Adapt<CompanyResponse>();
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

      
    }
}
