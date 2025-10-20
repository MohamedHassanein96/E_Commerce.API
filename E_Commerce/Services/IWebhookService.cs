namespace E_Commerce.Services
{
    public interface IWebhookService
    {
        Task MarkOrderAsPaidAsync(string stripeSessionId); 
        Task HandleWebhookAsync(string json, string stripeSignature);

    }
}
