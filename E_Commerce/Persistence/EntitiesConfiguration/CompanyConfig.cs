
namespace E_Commerce.Persistence.EntitiesConfiguration
{
    public class CompanyConfig : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasIndex(x => x.Id).IsUnique();
            builder.Property(x => x.Name).HasMaxLength(50);
            builder.Property(x => x.Description).HasMaxLength(150);
            builder.Property(x => x.Address).HasMaxLength(150);
        }
    }
}
