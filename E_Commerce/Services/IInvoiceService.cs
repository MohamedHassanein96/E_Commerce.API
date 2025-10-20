namespace E_Commerce.Services
{
    public interface IInvoiceService
    {
        Task<string> GenerateInvoiceAsync(Order order, CancellationToken cancellationToken = default);
        Task<byte[]> GetInvoiceFileAsync(int orderId);

    }
}
