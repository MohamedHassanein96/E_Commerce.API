namespace E_Commerce.Contracts.Common
{
    public class AllowedImagesSignatureValidator : AbstractValidator<IFormFile>
    {
        public AllowedImagesSignatureValidator()
        {
            RuleFor(X=>X).Must((request, context) =>
            {
                BinaryReader binary = new(request.OpenReadStream());
                var bytes = binary.ReadBytes(2);
                var fileSequenceHex = BitConverter.ToString(bytes);

                foreach (var signature in FileSettings.AllowedImagesSignatures)
                {
                    if (signature.Equals(fileSequenceHex, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                return false;

            }).WithMessage("Not allowed file content")
            .When(x => x is not null);
        }
    }
}
