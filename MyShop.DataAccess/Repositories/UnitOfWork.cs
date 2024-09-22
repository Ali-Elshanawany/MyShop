using MyShop.Entities.Repositories;
using MyShop.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Repositories;

public class UnitOfWork : IUnitOfWork
{

    private readonly ApplicationDbContext _context;
    public Iproduct category { get; private set; }
    public IProductRepository product { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        category = new product(context);
        product = new ProductRepository(context);
    }


    public int complete()
    {
        return _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
