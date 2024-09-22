using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyShop.Entities.Models;

namespace MyShop.Entities.ViewModels;

public class ProductViewModel
{
    [ValidateNever]
    public Product Product { get; set; } = default!;
    public IEnumerable<SelectListItem> CategoryList { get; set; } = new List<SelectListItem>();

    [ValidateNever]
    public IFormFile Image { get; set; } = default!;
}
