using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using MyShop.Entities.ViewModels;
using MyShop.Web.Helpers;
using Stripe;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MyShop.Web.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    //[BindProperty]
    //public OrderViewModel OrderViewModel { get; set; }

    public OrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {

        IEnumerable<OrderHeader> orderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(includedWord: "applicationUser");
        return View(orderHeaders);
    }

    public IActionResult Details([FromRoute(Name = "id")] int id)
    {
        OrderViewModel orderViewModel = new OrderViewModel()
        {
            OrderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == id, includedWord: "applicationUser"),
            OrderDetails = _unitOfWork.OrderDetailsRepository.GetAll(x => x.OrderHeaderId == id, includedWord: "Product")
        };

        return View(orderViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateOrderDetails(OrderViewModel orderViewModel)
    {
        var orderFromDB = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == orderViewModel.OrderHeader.Id, includedWord: "applicationUser");

        orderFromDB.Name = orderViewModel.OrderHeader.Name;
        orderFromDB.Phone = orderViewModel.OrderHeader.Phone;
        orderFromDB.Address = orderViewModel.OrderHeader.Address;
        orderFromDB.City = orderViewModel.OrderHeader.City;

        if (orderViewModel.OrderHeader.Carrier != null)
        {
            orderFromDB.Carrier = orderViewModel.OrderHeader.Carrier;
        }

        if (orderViewModel.OrderHeader.TrackingNumber != null)
        {
            orderFromDB.TrackingNumber = orderViewModel.OrderHeader.TrackingNumber;
        }

        _unitOfWork.OrderHeaderRepository.Update(orderFromDB);
        _unitOfWork.complete();

        TempData["Update"] = "Item has updated Successfully";
        return RedirectToAction("Details", "Order", new { id = orderFromDB.Id });

    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StartProcessing(OrderViewModel orderViewModel)
    {
        _unitOfWork.OrderHeaderRepository.UpdateOrderStatus(orderViewModel.OrderHeader.Id, GlobalVariables.Processing, null);
        _unitOfWork.complete();



        TempData["Updated"] = "Updated";
        return RedirectToAction("Details", "Order", new { id = orderViewModel.OrderHeader.Id });

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StartShipping(OrderViewModel orderViewModel)
    {
        var orderFromDb = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == orderViewModel.OrderHeader.Id);
        orderFromDb.TrackingNumber = orderViewModel.OrderHeader.TrackingNumber;
        orderFromDb.Carrier = orderViewModel.OrderHeader.Carrier;
        orderFromDb.OrderStatus = GlobalVariables.Shipped;
        orderFromDb.ShippingDate = DateTime.Now;

        _unitOfWork.OrderHeaderRepository.Update(orderFromDb);
        _unitOfWork.complete();

        TempData["Updated"] = "Updated";
        return RedirectToAction("Details", "Order", new { id = orderViewModel.OrderHeader.Id });

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CancelOrder(OrderViewModel orderViewModel)
    {
        var orderFromDb = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == orderViewModel.OrderHeader.Id);
        if (orderFromDb.PaymentStatus == GlobalVariables.ConfirmedPaymentStatus)
        {
            var option = new RefundCreateOptions
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = orderFromDb.PaymentIntentId,
            };

            var service = new RefundService();
            Refund refund = service.Create(option);

            _unitOfWork.OrderHeaderRepository.UpdateOrderStatus(orderFromDb.Id, GlobalVariables.Cancelled, GlobalVariables.Refund);
        }
        else
        {
            _unitOfWork.OrderHeaderRepository.UpdateOrderStatus(orderFromDb.Id, GlobalVariables.Cancelled, GlobalVariables.Cancelled);

        }

        _unitOfWork.complete();

        TempData["Updated"] = "Updated";
        return RedirectToAction("Details", "Order", new { id = orderViewModel.OrderHeader.Id });

    }



}
