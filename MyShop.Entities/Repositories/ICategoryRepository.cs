using MyShop.Entities.Models;

namespace MyShop.Entities.Repositories;

public interface Iproduct : IGenericRepository<Category>
{
    void Update(Category category);
}
