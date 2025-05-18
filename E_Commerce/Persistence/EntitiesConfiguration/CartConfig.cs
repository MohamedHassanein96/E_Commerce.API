
namespace E_Commerce.Persistence.EntitiesConfiguration
{
    public class CartConfig : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(x => new { x.ProductId, x.ApplicationUserId });
        }
    }
}
