namespace E_Commerce.Contracts.Product
{
    public class UpdateProductRequestValidator :AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(3);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Price).NotEmpty();
            RuleFor(x => x.Quantity).NotEmpty();

            When(x => x.Images != null && x.Images.Any(), () =>
            {
                RuleForEach(x => x.Images!)
                    .SetValidator(new ImageSizeValidator())
                    .SetValidator(new BlockedSignatureValidator())
                    .SetValidator(new ImageNameValidator())
                    .SetValidator(new AllowedImagesSignatureValidator());
            });
        }
    }
}
