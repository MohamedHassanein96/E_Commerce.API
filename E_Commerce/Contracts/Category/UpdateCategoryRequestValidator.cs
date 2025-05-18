namespace E_Commerce.Contracts.Category
{
    public class UpdateCategoryRequestValidator :AbstractValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryRequestValidator()
        {
            RuleFor(x => x.Name).NotNull();
            RuleFor(x => x.Name).MinimumLength(3);
            RuleFor(x => x.Name).MaximumLength(50);
        }
    }
}
