
namespace E_Commerce.Contracts.Image
{
    public class UploadImagesRequestValidator :AbstractValidator<UploadImagesRequest>
    {
        public UploadImagesRequestValidator()
        {
            RuleForEach(x => x.Images)
                 .SetValidator(new ImageSizeValidator())
                 .SetValidator(new BlockedSignatureValidator())
                 .SetValidator(new ImageNameValidator())
                 .SetValidator(new AllowedImagesSignatureValidator());
                
        }
    }
}
