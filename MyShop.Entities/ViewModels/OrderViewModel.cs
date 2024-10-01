using MyShop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Entities.ViewModels;

public class OrderViewModel
{

    public OrderHeader OrderHeader { get; set; } = default!;

    public IEnumerable<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
}
