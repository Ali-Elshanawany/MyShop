using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using MyShop.Entities.ViewModels;
using MyShop.Web.Helpers;

namespace MyShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"Admin")]
public class ProductController : Controller
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string _imgPath;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
        _imgPath = $"{_webHostEnvironment.WebRootPath}/Images/Products";
    }

    public IActionResult Index()
    {
        IEnumerable<Product> products = _unitOfWork.product.GetAll(null, "Category");

        return View(products);
    }


    [HttpGet]
    public IActionResult Create()
    {
        ProductViewModel productViewModel = new ProductViewModel()
        {
            Product = new Product(),
            CategoryList = _unitOfWork.category.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            })
        };
        return View(productViewModel);
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Create(ProductViewModel productViewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                productViewModel.CategoryList = _unitOfWork.category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
                return View(productViewModel);
            }


            var image = await SaveImg.SaveCover(productViewModel.Image, _imgPath);

            Product product = new Product()
            {
                Name = productViewModel.Product.Name,
                Image = image,
                Description = productViewModel.Product.Description,
                CategoryId = productViewModel.Product.CategoryId,
                Price = productViewModel.Product.Price,
            };

            _unitOfWork.product.Add(product);
            _unitOfWork.complete();
            return RedirectToAction("Index");

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest(ex.Message);
        }
    }




    [HttpGet]
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
            return NotFound();

        var product = _unitOfWork.product.GetFirstOrDefault(x => x.Id == id);

        ProductViewModel productViewModel = new ProductViewModel()
        {
            Product = product,
            CategoryList = _unitOfWork.category.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            })
        };

        return View(productViewModel);

    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Edit(ProductViewModel newproductViewModel)
    {

        if (!ModelState.IsValid)
        {
            ProductViewModel productViewModel = new ProductViewModel()
            {
                Product = newproductViewModel.Product,
                CategoryList = _unitOfWork.category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(productViewModel);
        }

        if (newproductViewModel.Image != null)
        {
            var oldImg = Path.Combine(_imgPath, newproductViewModel.Product.Image);
            if (System.IO.File.Exists(oldImg))
            {
                System.IO.File.Delete(oldImg);
            }
            var image = await SaveImg.SaveCover(newproductViewModel.Image, _imgPath);
            newproductViewModel.Product.Image = image;
        }

        _unitOfWork.product.Update(newproductViewModel.Product);
        _unitOfWork.complete();
        TempData["Updated"] = "Updated";
        return RedirectToAction("Index");

    }

    [HttpGet]
    public IActionResult Remove(int? id)
    {
        if (id == null || id == 0)
            return NotFound();

        var product = _unitOfWork.product.GetFirstOrDefault(x => x.Id == id, includedWord: "Category");

        return View(product);

    }
    [ValidateAntiForgeryToken]
    [HttpPost]
    public IActionResult RemoveProduct(int? id)
    {

        var product = _unitOfWork.product.GetFirstOrDefault(x => x.Id == id);

        if (product == null)
            return NotFound();

        string path = Path.Combine(_imgPath, product.Image);


        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        _unitOfWork.product.Remove(product);
        _unitOfWork.complete();
        //ViewBag.Deleted = true;
        TempData["Deleted"] = "Deleted";
        return RedirectToAction("Index");

    }





}


