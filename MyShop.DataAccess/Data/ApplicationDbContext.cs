using Microsoft.EntityFrameworkCore;
using MyShop.Entities.Models;

namespace MyShop.Web.Data;

public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

}
