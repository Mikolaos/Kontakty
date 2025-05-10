using Kontakty.Models;
using Kontakty.Enums;

namespace Kontakty.Enums;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}
