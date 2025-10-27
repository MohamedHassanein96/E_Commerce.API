namespace E_Commerce.Entities;

public class Product : ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public double? Rate { get; set; }
    public int AvailableStock { get; set; } // Stock فعلي للبيع
    public int ReservedStock { get; set; } = 0;
    public ICollection<ProductImage> ProductImages { get; set; } = [];
    public int? CategoryId { get; set; }
    public Category Category { get; set; } = default!; 
    public List<Review> Reviews { get; set; } = [];
    public int Version { get; set; } = 0;
    public bool IsDeleted { get; set; }
    public DateTime? DateDeleted { get; set; }
    public int StockForReservation => AvailableStock - ReservedStock; //  
    public ProductHighlightType HighlightType { get; set; } = ProductHighlightType.None;
    public string? DeletedBy { get ; set; } = string.Empty;
}
public enum ProductHighlightType
{
    None,
    Featured,
    TopRated,
    NewArrival
}
