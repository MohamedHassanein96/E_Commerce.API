using E_Commerce;
using E_Commerce.Authentication;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using Stripe;
using System.Text;
using InvoiceService = E_Commerce.Services.InvoiceService;
using ProductService = E_Commerce.Services.ProductService;
using ReviewService = E_Commerce.Services.ReviewService;


public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        QuestPDF.Settings.License = LicenseType.Community;

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        // Database connection string
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string is not found");

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        // Identity setup
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        // Scoped services
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<IWebhookService, WebhookService>();
        builder.Services.AddScoped<IJwtProvider, JwtProvider>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<IInvoiceService, InvoiceService>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();


        builder.Services.AddScoped<ValidateUserExistsFilter>();





        // Fluent validation
        builder.Services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Mapping
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());
        builder.Services.AddSingleton<IMapper>(new Mapper(mappingConfig));


        builder.Services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();
        var jwtSettings = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();


        // Authentication setup
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
               .AddJwtBearer(o =>
               {
                   o.SaveToken = true;
                   //o.MapInboundClaims = false; 
                   o.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                       ValidIssuer = jwtSettings?.Issuer,
                       ValidAudience = jwtSettings?.Audience
                   };
               });


        // Stripe setup
        StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

       
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapStaticAssets();

        // Run the application
        app.Run();
    }
}