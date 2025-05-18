namespace E_Commerce.Persistence.EntitiesConfiguration
{
    public class ImageConfig : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.Property(x => x.ImageName).HasMaxLength(250);
            builder.Property(x => x.ContentType).HasMaxLength(50);
            builder.Property(x => x.ImageExtension).HasMaxLength(10);
        }

    }
}
