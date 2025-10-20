namespace E_Commerce.Entities
{
    public class Cart
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } 
        public Product Product { get; set; } = default!;
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; } = default!;
       
    }
}
