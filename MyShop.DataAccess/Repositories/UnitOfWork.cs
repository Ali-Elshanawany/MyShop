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
    public ICategoryRepository CategoryRepository { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        CategoryRepository = new CategoryRepository(context);
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
