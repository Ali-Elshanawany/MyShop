using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using System.Security.Claims;

namespace MyShop.Web.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{

    private readonly IUnitOfWork _unitOfWork;

    public HomeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var products = _unitOfWork.product.GetAll();
        return View(products);
    }

    [HttpGet]
    public IActionResult Details([FromRoute(Name = "id")] int productId)
    {
        ShoppingCart shoppingCart = new ShoppingCart()
        {
            ProductId = productId,
            Product = _unitOfWork.product.GetFirstOrDefault(x => x.Id == productId, includedWord: "Category"),
            Count = 1
        };
        return View(shoppingCart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        shoppingCart.ApplicationUserId = claim.Value;
        shoppingCart.Id = 0;

        ShoppingCart cartObj = _unitOfWork.shoppingCart
            .GetFirstOrDefault(x => x.ApplicationUserId == claim.Value && x.ProductId == shoppingCart.ProductId);

        if (cartObj != null)
        {
            _unitOfWork.shoppingCart.IncreaseCount(cartObj, shoppingCart.Count);
        }
        else
        {

            _unitOfWork.shoppingCart.Add(shoppingCart);
        }

        _unitOfWork.complete();


        return RedirectToAction("Index");
    }
}
