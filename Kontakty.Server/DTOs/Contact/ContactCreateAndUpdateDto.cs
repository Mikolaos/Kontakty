using System.ComponentModel.DataAnnotations;

namespace Kontakty.DTOs;

public class ContactCreateAndUpdateDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    [StrongPassword]
    public string Password { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }
    public int? SubCategoryId { get; set; }
    public string? SubCategoryName { get; set; }
    public string? CustomSubCategory { get; set; }

    [Phone]
    public string PhoneNumber { get; set; }

    [Required]
    public DateOnly DateOfBirth { get; set; }
}