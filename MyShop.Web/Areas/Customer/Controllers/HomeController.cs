using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using MyShop.Entities.Repositories;
using MyShop.Entities.ViewModels;

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

    public IActionResult Details(int id)
    {
        ShoppingCartViewModel shoppingCart = new ShoppingCartViewModel()
        {
            Product = _unitOfWork.product.GetFirstOrDefault(x => x.Id == id, includedWord: "Category"),
            Count = 1
        };
        return View(shoppingCart);
    }
}
