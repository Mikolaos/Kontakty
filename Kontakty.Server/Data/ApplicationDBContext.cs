using Kontakty.Enums;
using Kontakty.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kontakty.Data;

/// <summary>
/// Database context class that inherits from IdentityDbContext to handle both
/// application data and identity management
/// </summary>
public class ApplicationDBContext : IdentityDbContext<AppUser>
{
     /// <summary>
     /// Constructor that accepts database configuration options
     /// </summary>
     /// <param name="dbContextOptions">Database configuration options</param>
     public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
     {
     }

     /// <summary>
     /// DbSet for managing contact records
     /// </summary>
     public DbSet<ContactModel> Contacts { get; set; }

     /// <summary>
     /// DbSet for managing contact categories
     /// </summary>
     public DbSet<Category> Categories { get; set; }

     /// <summary>
     /// DbSet for managing contact subcategories
     /// </summary>
     public DbSet<SubCategory> SubCategories { get; set; }

     /// <summary>
     /// Configures the database model and relationships
     /// </summary>
     /// <param name="modelBuilder">Model builder instance for configuring the database</param>
     protected override void OnModelCreating(ModelBuilder modelBuilder)
     {
          // Call base configuration for identity tables
          base.OnModelCreating(modelBuilder);

          // Define default roles for the application
          List<IdentityRole> roles = new List<IdentityRole>()
          {
               new IdentityRole
               {
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR"
               },
               new IdentityRole
               {
                    Name = "User",
                    NormalizedName = "USER"
               }
          };
          
          // Seed the default roles
          modelBuilder.Entity<IdentityRole>().HasData(roles);

          // Configure one-to-many relationship between Category and SubCategory
          // with cascade delete behavior
          modelBuilder.Entity<Category>()
               .HasMany(c => c.SubCategories)
               .WithOne(s => s.Category)
               .HasForeignKey(s => s.CategoryId)
               .OnDelete(DeleteBehavior.Cascade);

          // Configure unique constraint on Contact's email
          modelBuilder.Entity<ContactModel>()
               .HasIndex(c => c.Email)
               .IsUnique();
     }
}