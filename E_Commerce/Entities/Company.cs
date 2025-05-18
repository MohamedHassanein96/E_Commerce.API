namespace E_Commerce.Entities
{
    public class Company
    {
        public int Id { get; set; }
 
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public ICollection<Category> Categories { get; set; } = default!;
    }
}
