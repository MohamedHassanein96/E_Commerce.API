namespace E_Commerce.Contracts.Review
{
    public class ReviewRequestValidator :AbstractValidator<ReviewRequest>
    {
        public ReviewRequestValidator()
        {
            RuleFor(x => x.ProductId)
            .NotEmpty().GreaterThan(0).WithMessage("ProductId must be greater than zero.");

            RuleFor(x => x.UserReview)
            .NotEmpty()
            .WithMessage("Please provide a review text.")
            .Must(review => !string.IsNullOrWhiteSpace(review))
            .WithMessage("Please provide a review text.");

            RuleFor(x => x.Stars)
            .InclusiveBetween(0, 5);
        }
    }
}
