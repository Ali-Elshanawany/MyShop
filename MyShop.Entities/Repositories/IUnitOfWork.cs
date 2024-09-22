using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Entities.Repositories;

public interface IUnitOfWork : IDisposable
{

    Iproduct category { get; }
    IProductRepository product { get; }

    int complete();


}
