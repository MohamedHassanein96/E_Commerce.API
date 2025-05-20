
namespace E_Commerce.Contracts.Image
{
    public class UpdateImagesRequestValidator :AbstractValidator<UpdateImagesRequest>
    {
        public UpdateImagesRequestValidator()
        {
            When(x => x.Images is not null && x.Images.Any(), () =>
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
