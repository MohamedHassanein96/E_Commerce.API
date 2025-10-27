namespace E_Commerce.Persistence.EntitiesConfiguration
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.HasIndex(x => x.Name).IsUnique();
            builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
           
        }
    }
}
