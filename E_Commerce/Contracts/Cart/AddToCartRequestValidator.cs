namespace E_Commerce.Contracts.Cart
{
    public class AddToCartRequestValidator :AbstractValidator<AddToCartRequest>
    {
        public AddToCartRequestValidator()
        {
       

            RuleFor(x => x.ProductId)
            .NotEmpty().GreaterThan(0).WithMessage("ProductId must be greater than zero.");

            RuleFor(x => x.Count)
             .NotEmpty().GreaterThan(0).WithMessage("Count must be at least 1.");
        }
    }
}
