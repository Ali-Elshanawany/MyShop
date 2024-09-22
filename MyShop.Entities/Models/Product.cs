

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;

namespace MyShop.Entities.Models;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [ValidateNever]
    public string Image { get; set; } = string.Empty;

    public decimal Price { get; set; }

    [DisplayName("Category")]
    public int CategoryId { get; set; }

    [ValidateNever]

    public Category Category { get; set; } = default!;

}
