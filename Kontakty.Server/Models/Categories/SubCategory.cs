﻿using Kontakty.Models;

namespace Kontakty.Enums;

public class SubCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}


