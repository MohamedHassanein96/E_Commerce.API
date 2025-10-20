namespace E_Commerce.Contracts.Cart
{
    public record PayResponse(string Status, string SessionId, string InvoiceUrl,string Url);
    
    
}
