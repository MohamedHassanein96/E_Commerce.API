
namespace E_Commerce.Entities
{
    public class Category : ISoftDeletable
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;

        public int? CompanyId { get; set; }
        public Company Company { get; set; } = default!;
        public ICollection<Product> Products { get; set; } = default!;
        public bool IsDeleted { get ; set ; }
        public DateTime? DateDeleted { get ; set ; }
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
