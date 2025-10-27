public class Review 
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;
    public int Stars { get; set; } = 0;
    public string Comment { get; set; } = string.Empty;
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!; 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
   
}
