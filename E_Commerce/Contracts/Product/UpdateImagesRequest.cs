namespace E_Commerce.Contracts.Image
{
    public record UpdateImagesRequest(IFormFileCollection Images)
    {
        public IFormFileCollection ImagesNonNull => Images ?? new FormFileCollection();
    }
}
