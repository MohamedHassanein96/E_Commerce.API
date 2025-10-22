
namespace E_Commerce.Persistence.EntitiesConfiguration
{
    public class ReviewConfig : IEntityTypeConfiguration<Review>

    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.Property(r => r.Stars)
           .IsRequired();

            builder.Property(r => r.Comment)
                .HasMaxLength(1000);

            builder.HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Review

            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
