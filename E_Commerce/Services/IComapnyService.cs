namespace E_Commerce.Services
{
    public interface IComapnyService
    {
        Task<CompanyResponse>AddAsync(CompanyRequest request, CancellationToken cancellationToken = default);
        Task<CompanyResponse>GetAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(int id,UpdateCompanyRequest request ,CancellationToken cancellationToken = default);
        Task<IEnumerable<CompanyResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
