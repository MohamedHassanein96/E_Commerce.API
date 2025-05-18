namespace E_Commerce.Contracts.Cart
{
    public class DeleteRequestValidator :AbstractValidator<DeleteRequest>
    {
        public DeleteRequestValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().GreaterThan(0)
           .LessThanOrEqualTo(int.MaxValue);
         }
    }
}