namespace E_Commerce.Contracts.Common
{
    public class ImageNameValidator :AbstractValidator<IFormFile>
    {
        public ImageNameValidator()
        {
            RuleFor(x => x.FileName).Matches("^[A-Za-z0-9_\\-.]*$")
                .WithMessage("File name must not contain '/'.")
                .When(x => x is not null);
        }
    }
}
