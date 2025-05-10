using Kontakty.Enums;

namespace Kontakty.Models;

public class ContactModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public string? CategoryName { get; set; }

    public int? SubCategoryId { get; set; }
    public SubCategory? SubCategory { get; set; }
    public string? SubCategoryName { get; set; }
    public string? CustomSubCategory { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
}