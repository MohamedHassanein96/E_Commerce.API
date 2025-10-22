public class Review : ISoftDeletable
{
    public int Id { get; set; }
    public string UserReview { get; set; } = string.Empty;


    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;


    public int Stars { get; set; } = 0;
    public string Sentiment { get; set; } = string.Empty; // Review

    public string Comment { get; set; } = string.Empty;

    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!; // علاقة بـ Identity User

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
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
