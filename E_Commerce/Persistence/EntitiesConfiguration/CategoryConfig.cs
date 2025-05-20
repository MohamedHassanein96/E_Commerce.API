namespace E_Commerce.Persistence.EntitiesConfiguration
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.Property(x => x.Name).HasMaxLength(50);
            builder
             .HasOne(c => c.Company)
             .WithMany(co => co.Categories)
             .HasForeignKey(c => c.CompanyId)
             .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
