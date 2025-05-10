using Kontakty.Data;
using Kontakty.Enums;
using Kontakty.Interfaces;
using Microsoft.EntityFrameworkCore;
using Kontakty.Models;
using Kontakty.Repository;
using Kontakty.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Add required services and dependencies
var builder = WebApplication.CreateBuilder(args);

// Add MVC Controllers and API Explorer for endpoint documentation
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT authentication
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(option =>
{
    // Basic Swagger documentation setup
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    
    // Configure JWT authentication scheme for Swagger
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    
    // Add JWT security requirement to Swagger
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Configure Entity Framework with in-memory database
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseInMemoryDatabase("Kontakty");
});

// Configure Identity with password requirements
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDBContext>();

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    // Set JWT as default authentication scheme
    options.DefaultAuthenticateScheme =
        options.DefaultChallengeScheme =
            options.DefaultForbidScheme =
                options.DefaultScheme =
                    options.DefaultSignInScheme =
                        options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Configure JWT token validation parameters
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        )
    };
});

// Register repositories and services
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Configure CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Initialize sample data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Create default roles if they don't exist
    string[] roles = new[] { "Administrator", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Create default categories
    var sluzbowy = new Category { Name = "Służbowy" };
    var prywatny = new Category { Name = "Prywatny" };
    var inny = new Category { Name = "Inny" };

    if (!db.Categories.Any())
    {
        // Add categories and save to generate IDs
        db.Categories.AddRange(sluzbowy, prywatny, inny);
        await db.SaveChangesAsync();

        // Add subcategories for business contacts
        db.SubCategories.AddRange(
            new SubCategory { Name = "Szef", CategoryId = sluzbowy.Id },
            new SubCategory { Name = "Klient", CategoryId = sluzbowy.Id }
        );

        await db.SaveChangesAsync();
    }

    // Add sample contacts
    db.Contacts.AddRange(
        // Business contact example
        new ContactModel
        {
            Name = "Jan",
            LastName = "Kowalski",
            Email = "jan.kowalski@example.com",
            Password = "haslo123",
            CategoryId = sluzbowy.Id,
            CategoryName = sluzbowy.Name,
            Category = sluzbowy,
            SubCategoryId = 2,
            SubCategory = db.SubCategories.FirstOrDefault(sc => sc.Id == 2),
            SubCategoryName = db.SubCategories.FirstOrDefault(sc => sc.Id == 2)?.Name,
            PhoneNumber = "123-456-789",
            DateOfBirth = new DateOnly(1990, 5, 1)
        },
        // Private contact example
        new ContactModel
        {
            Name = "Anna",
            LastName = "Nowak",
            Email = "anna.nowak@example.com",
            Password = "tajnehaslo",
            CategoryId = prywatny.Id,
            CategoryName = prywatny.Name,
            Category = prywatny,
            CustomSubCategory = "Znajomi",
            PhoneNumber = "987-654-321",
            DateOfBirth = new DateOnly(1985, 3, 15)
        }
    );

    db.SaveChanges();
    db.Database.EnsureCreated();
}

// Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowReactApp");

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Map controllers
app.MapControllers();

// Run the application
app.Run();