namespace E_Commerce.Contracts.Cart
{
    public class IncrementRequestValidator : AbstractValidator<IncrementRequest>
    {
        public IncrementRequestValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty()
                .GreaterThan(0)
                .LessThanOrEqualTo(int.MaxValue);
        }
    }
}
