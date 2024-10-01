using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using MyShop.Entities.ViewModels;
using MyShop.Web.Helpers;
using Stripe.Checkout;
using System.Security.Claims;

namespace MyShop.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{

    private readonly IUnitOfWork _unitOfWork;
    public ShoppingCartViewModel shoppingCartViewModel { get; set; }

    public decimal TotalPrice { get; set; }

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        shoppingCartViewModel = new ShoppingCartViewModel
        {
            cartList = _unitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == claim.Value, includedWord: "Product"),
            OrderHeader = new()
        };

        foreach (var item in shoppingCartViewModel.cartList)
        {
            shoppingCartViewModel.TotalPrice += (item.Count * item.Product.Price);
        }


        return View(shoppingCartViewModel);
    }


    public IActionResult Plus([FromRoute(Name = "id")] int CartId)
    {
        var shoopingCart = _unitOfWork.shoppingCart.GetFirstOrDefault(x => x.Id == CartId);
        _unitOfWork.shoppingCart.IncreaseCount(shoopingCart, 1);
        _unitOfWork.complete();
        return RedirectToAction("Index");
    }
    public IActionResult Minus([FromRoute(Name = "id")] int CartId)
    {
        var shoopingCart = _unitOfWork.shoppingCart.GetFirstOrDefault(x => x.Id == CartId);
        if (shoopingCart.Count <= 1)
        {
            _unitOfWork.shoppingCart.Remove(shoopingCart);
        }
        else
        {
            _unitOfWork.shoppingCart.DecreaseCount(shoopingCart, 1);
        }
        _unitOfWork.complete();
        return RedirectToAction("Index");
    }

    public IActionResult Remove([FromRoute(Name = "id")] int CartId)
    {
        var shoopingCart = _unitOfWork.shoppingCart.GetFirstOrDefault(x => x.Id == CartId);
        _unitOfWork.shoppingCart.Remove(shoopingCart);
        _unitOfWork.complete();
        return RedirectToAction("Index");
    }



    [HttpGet]
    public IActionResult Summary()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        shoppingCartViewModel = new ShoppingCartViewModel
        {
            cartList = _unitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == claim.Value, includedWord: "Product"),
            OrderHeader = new OrderHeader()

        };

        shoppingCartViewModel.OrderHeader.applicationUser = _unitOfWork.ApplicationUserRepository.GetFirstOrDefault(x => x.Id == claim.Value);
        shoppingCartViewModel.OrderHeader.Name = shoppingCartViewModel.OrderHeader.applicationUser.Name;
        shoppingCartViewModel.OrderHeader.Address = shoppingCartViewModel.OrderHeader.applicationUser.Address;
        shoppingCartViewModel.OrderHeader.City = shoppingCartViewModel.OrderHeader.applicationUser.City;
        shoppingCartViewModel.OrderHeader.Phone = shoppingCartViewModel.OrderHeader.applicationUser.PhoneNumber;



        foreach (var item in shoppingCartViewModel.cartList)
        {
            shoppingCartViewModel.TotalPrice += (item.Count * item.Product.Price);
        }


        return View(shoppingCartViewModel);

    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Summary")]
    public IActionResult PostSummary(ShoppingCartViewModel shoppingCartViewModel)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        var user = _unitOfWork.ApplicationUserRepository.GetFirstOrDefault(x => x.Id == claim.Value);

        shoppingCartViewModel.cartList = _unitOfWork.shoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includedWord: "Product");
        shoppingCartViewModel.OrderHeader = new OrderHeader();

        shoppingCartViewModel.OrderHeader.OrderStatus = GlobalVariables.PendingOrderStatus;
        shoppingCartViewModel.OrderHeader.PaymentStatus = GlobalVariables.PendingPaymentStatus;
        shoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;
        shoppingCartViewModel.OrderHeader.ApplicationUserId = claim.Value;

        shoppingCartViewModel.OrderHeader.Name = user.Name;
        shoppingCartViewModel.OrderHeader.Address = user.Address;
        shoppingCartViewModel.OrderHeader.City = user.City;
        shoppingCartViewModel.OrderHeader.Phone = user.PhoneNumber;

        foreach (var item in shoppingCartViewModel.cartList)
        {
            shoppingCartViewModel.OrderHeader.TotalPrice += (item.Count * item.Product.Price);
        }

        _unitOfWork.OrderHeaderRepository.Add(shoppingCartViewModel.OrderHeader);
        _unitOfWork.complete();

        foreach (var item in shoppingCartViewModel.cartList)
        {

            OrderDetails orderDetails = new OrderDetails()
            {
                ProductId = item.ProductId,
                OrderHeaderId = shoppingCartViewModel.OrderHeader.Id,
                Price = item.Product.Price,
                Count = item.Count,
            };

            _unitOfWork.OrderDetailsRepository.Add(orderDetails);
            _unitOfWork.complete();
        }


        // Stripe Action
        var domain = "https://localhost:7008/";

        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={shoppingCartViewModel.OrderHeader.Id}",
            CancelUrl = domain + $"Customer/Cart/Index",
        };

        foreach (var item in shoppingCartViewModel.cartList)
        {

            var sessionLineoption = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Product.Price * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product.Name
                    },
                },
                Quantity = item.Count,
            };

            options.LineItems.Add(sessionLineoption);
        }
        var service = new SessionService();
        Session session = service.Create(options);

        shoppingCartViewModel.OrderHeader.SessionId = session.Id;

        _unitOfWork.complete();

        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);


        //_unitOfWork.shoppingCart.RemoveRange(shoppingCartViewModel.cartList);
        //_unitOfWork.complete();
        //return RedirectToAction("Index", "Home");
    }

    public IActionResult OrderConfirmation(int id)
    {

        OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == id);

        var service = new SessionService();
        Session session = service.Get(orderHeader.SessionId);

        if (session.PaymentStatus.ToLower() == GlobalVariables.Paid)
        {
            shoppingCartViewModel.OrderHeader.PaymentIntentId = session.PaymentIntentId;
            _unitOfWork.OrderHeaderRepository.UpdateOrderStatus(id, GlobalVariables.ConfirmedOrderStatus,
                                                                    GlobalVariables.ConfirmedPaymentStatus);
            _unitOfWork.complete();
        }

        List<ShoppingCart> shoppingCarts = _unitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

        _unitOfWork.shoppingCart.RemoveRange(shoppingCarts);
        _unitOfWork.complete();
        return View(id);
    }





}
