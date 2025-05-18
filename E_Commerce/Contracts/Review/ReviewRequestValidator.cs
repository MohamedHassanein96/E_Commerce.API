namespace E_Commerce.Contracts.Review
{
    public class ReviewRequestValidator :AbstractValidator<ReviewRequest>
    {
        public ReviewRequestValidator()
        {
            RuleFor(x => x.ProductId)
            .NotEmpty().GreaterThan(0).WithMessage("ProductId must be greater than zero.");

            RuleFor(x => x.UserReview)
            .NotEmpty();

            RuleFor(x => x.Stars)
            .InclusiveBetween(0, 5);
        }
    }
}
