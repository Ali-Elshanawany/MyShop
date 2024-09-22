using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using MyShop.Web.Data;


namespace MyShop.DataAccess.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{

    private readonly ApplicationDbContext _context;
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Product product)
    {
        var oldProduct = _context.Products.FirstOrDefault(x => x.Id == product.Id);
        if (product != null)
        {
            oldProduct.Name = product.Name;
            oldProduct.Description = product.Description;
            oldProduct.Price = product.Price;
            oldProduct.Image = product.Image;
            oldProduct.CategoryId = product.CategoryId;

        }
    }
}
