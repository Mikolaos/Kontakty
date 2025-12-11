using Kontakty.Enums;
using Kontakty.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kontakty.Data;

public class ApplicationDBContext : IdentityDbContext<AppUser>
{
     public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
     {
     }

     public DbSet<ContactModel> Contacts { get; set; }

     public DbSet<Category> Categories { get; set; }

     public DbSet<SubCategory> SubCategories { get; set; }
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