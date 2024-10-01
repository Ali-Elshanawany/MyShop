using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MyShop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Entities.ViewModels;

public class ShoppingCartViewModel
{

    public IEnumerable<ShoppingCart> cartList { get; set; } = new List<ShoppingCart>();

    [ValidateNever]
    public OrderHeader OrderHeader { get; set; } = default!;

    public decimal TotalPrice { get; set; }

}
