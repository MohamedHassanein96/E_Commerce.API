namespace E_Commerce.Contracts.Cart
{
    public class DecrementRequestValidator :AbstractValidator<DecrementRequest>
    {
        public DecrementRequestValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().GreaterThan(0)
           .LessThanOrEqualTo(int.MaxValue);
         }
    }
}