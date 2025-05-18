namespace E_Commerce.Entities
{
    public class ProductImage
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string ImageName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string ImageExtension { get; set; } = string.Empty;
        public Product Product { get; set; } =default!;
        public int ProductId { get; set; } 
    }
}
