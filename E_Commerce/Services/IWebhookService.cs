namespace E_Commerce.Services
{
    public interface IWebhookService
    {
        Task MarkOrderAsPaidAsync(string stripeSessionId);
    }
}
