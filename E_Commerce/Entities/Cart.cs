namespace E_Commerce.Entities
{
    public class Cart
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public Product Product { get; set; } = default!;
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; } = default!;
       
    }
}
