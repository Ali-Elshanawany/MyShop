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
    IShoppingCart shoppingCart { get; }
    IOrderHeaderRepository OrderHeaderRepository { get; }
    IOrderDetailsRepository OrderDetailsRepository { get; }
    IApplicationUserRepository ApplicationUserRepository { get; }

    int complete();


}
