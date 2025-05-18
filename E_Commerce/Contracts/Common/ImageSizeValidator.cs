namespace E_Commerce.Contracts.Common
{
    public class ImageSizeValidator : AbstractValidator<IFormFile>
    {
        public ImageSizeValidator()
        {
            RuleFor(x=>x).Must((request,context)=>request.Length <= FileSettings.MaxFileSizeInBytes)
            .WithMessage($"Max file size is {FileSettings.MaxFileSizeInMB} MB.")
            .When(x => x is not null);
        }
    }
}
