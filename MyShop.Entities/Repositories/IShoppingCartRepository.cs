using MyShop.Entities.Models;

namespace MyShop.Entities.Repositories;

public interface IShoppingCart : IGenericRepository<ShoppingCart>
{
    void Update(ShoppingCart shoppingCart);

    int IncreaseCount(ShoppingCart shoppingCart, int count);
    int DecreaseCount(ShoppingCart shoppingCart, int count);

}
