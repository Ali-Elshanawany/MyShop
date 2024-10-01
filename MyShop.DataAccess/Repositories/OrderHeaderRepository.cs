using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using MyShop.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Repositories;

public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
{
    private readonly ApplicationDbContext _context;
    public OrderHeaderRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(OrderHeader orderHeader)
    {
        _context.OrderHeaders.Update(orderHeader);
    }

    public void UpdateOrderStatus(int id, string OrderStatus, string PaymentStatus)
    {
        var orderFromDb = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
        if (OrderStatus != null)
        {
            orderFromDb.OrderStatus = OrderStatus;
            orderFromDb.PaymentDate = DateTime.Now;

        }
        if (PaymentStatus != null)
        {
            orderFromDb.PaymentStatus = PaymentStatus;
        }

    }
}
