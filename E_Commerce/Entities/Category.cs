namespace E_Commerce.Entities
{
    public class Category : ISoftDeletable
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public ICollection<Product> Products { get; set; } = [];
        public bool IsDeleted { get ; set ; }
        public DateTime? DateDeleted { get ; set ; }
        public string? DeletedBy { get; set; } = string.Empty;
    }
}
