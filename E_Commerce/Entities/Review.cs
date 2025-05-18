public class Review
{
    public int Id { get; set; }
    public string UserReview { get; set; } = string.Empty;

   
    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;


    public int Stars { get; set; }
    public string Sentiment { get; set; } = string.Empty;
}
