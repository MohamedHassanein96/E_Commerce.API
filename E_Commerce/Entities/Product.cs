namespace E_Commerce.Entities;

public class Product : ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public double? Rate { get; set; }
    public int AvailableStock { get; set; } // Stock فعلي للبيع
    public int ReservedStock { get; set; }  // Stock محجوز في Cart
    public ICollection<ProductImage> ProductImages { get; set; } = default!;
    public int? CategoryId { get; set; }
    public Category Category { get; set; } = default!;
    public List<Review> Reviews { get; set; } = [];
    public int Version { get; set; } = 0;
    public bool IsDeleted { get; set; }
    public DateTime? DateDeleted { get; set; }
    public int StockForReservation => AvailableStock - ReservedStock;
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
    public ProductHighlightType HighlightType { get; set; } = ProductHighlightType.None;

}
public enum ProductHighlightType
{
    None,
    Featured,
    TopRated,
    NewArrival
}
