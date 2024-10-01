using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Entities.Models;

public class OrderDetails
{
    public int Id { get; set; }
    public int OrderHeaderId { get; set; }

    [ValidateNever]
    public OrderHeader OrderHeader { get; set; } = default!;

    public int ProductId { get; set; }

    [ValidateNever]
    public Product Product { get; set; } = default!;

    public int Count { get; set; }

    public decimal Price { get; set; }
}
