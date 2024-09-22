using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using MyShop.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Repositories;

public class product : GenericRepository<Category>, Iproduct
{

    private readonly ApplicationDbContext _context;
    public product(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Category category)
    {
        var oldCategory = _context.Categories.FirstOrDefault(x => x.Id == category.Id);
        if (category != null)
        {
            oldCategory.Name = category.Name;
            oldCategory.Description = category.Description;
            oldCategory.CreatedTime = DateTime.Now;
        }
    }
}
