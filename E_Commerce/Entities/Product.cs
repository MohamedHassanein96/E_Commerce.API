namespace E_Commerce.Entities
{
    public class Product :ISoftDeletable
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public double? Rate { get; set; }
        public bool IsTopRated { get; set; }
        public bool IsFeatured { get; set; }
        public int Quantity { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; } = default!;
        public int? CategoryId { get; set; } 
        public Category Category { get; set; } = default!; 

        public List<Review> Reviews { get; set; } = [];
        public int Version { get; set; } = 0;

        public bool IsDeleted { get; set; }
        public DateTime? DateDeleted { get; set; }

        public void Delete()
        {
            IsDeleted = true;
            DateDeleted = DateTime.Now;
        }

        public void UndoDelete()
        {
            IsDeleted = false;
            DateDeleted = null;
        }
    }

}
