using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using MyShop.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Repositories;

public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCart
{

    private readonly ApplicationDbContext _context;
    public ShoppingCartRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public int DecreaseCount(ShoppingCart shoppingCart, int count)
    {
        shoppingCart.Count -= count;
        return shoppingCart.Count;
    }

    public int IncreaseCount(ShoppingCart shoppingCart, int count)
    {
        shoppingCart.Count += count;
        return shoppingCart.Count;
    }

    public void Update(ShoppingCart shoppingCart)
    {
        //var oldCategory = _context..FirstOrDefault(x => x.Id == shoppingCart.Id);
        //if (shoppingCart != null)
        //{
        //    oldCategory.Name = shoppingCart.Name;
        //    oldCategory.Description = category.Description;
        //    oldCategory.CreatedTime = DateTime.Now;
        //}
    }
}
