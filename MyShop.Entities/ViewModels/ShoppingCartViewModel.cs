

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MyShop.Entities.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyShop.Entities.ViewModels;

public class ShoppingCartViewModel
{
    public Product Product { get; set; } = default!;

    [Range(1, 100, ErrorMessage = "You Must enter a value between 1:100")]
    public int Count { get; set; }
}
