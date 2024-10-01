using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyShop.Entities.Models;

public class ShoppingCart
{

    public int Id { get; set; }
    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    [ValidateNever]
    public Product Product { get; set; } = default!;

    [Range(1, 100, ErrorMessage = "You Must enter a value between 1:100")]
    public int Count { get; set; }

    public string ApplicationUserId { get; set; } = string.Empty;

    [ForeignKey("ApplicationUserId")]
    [ValidateNever]

    public ApplicationUser ApplicationUser { get; set; } = default!;

}
