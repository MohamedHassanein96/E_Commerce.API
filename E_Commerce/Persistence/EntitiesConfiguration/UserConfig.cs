namespace E_Commerce.Persistence.EntitiesConfiguration
{
    public class UserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.FirstName).HasMaxLength(100);
            builder.Property(x => x.LastName).HasMaxLength(100);

            builder.HasData(new ApplicationUser
            {
                Id = "6dc6528a-b280-4770-9eae-82671ee81ef7",
                FirstName = "Mohamed",
                LastName = "Hassanein",
                UserName = "Mohamed@E_Commerce.com",
                NormalizedUserName = "MOHAMED@E_COMMERCE.COM",
                Email = "Mohamed@E_Commerce.com",
                NormalizedEmail = "Mohamed@E_Commerce.com",
                SecurityStamp = "55BF92C9EF0249CDA210D85D1A851BC9",
                ConcurrencyStamp= "99d2bbc6-bc54-4248-a172-a77de3ae4430",
                PasswordHash = "AQAAAAIAAYagAAAAEOh+NqeiN546A10OVMz8PP5ov+8LHGp1j/4ISOir43labqJrQkQdZTekO3HJumb4QA==" //Tarek@4697

            });


        }
    }
}
