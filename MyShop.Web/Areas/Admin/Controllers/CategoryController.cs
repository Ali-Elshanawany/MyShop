using Microsoft.AspNetCore.Mvc;
using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using MyShop.Web.Data;

namespace MyShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{

    private readonly IUnitOfWork _unitOfWork;

    public CategoryController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<Category> categories = _unitOfWork.CategoryRepository.GetAll();

        return View(categories);
    }


    [HttpGet]
    public IActionResult Create()
    {

        return View();
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public IActionResult Create(Category category)
    {
        if (!ModelState.IsValid)
        {
            return View(category);
        }
        _unitOfWork.CategoryRepository.Add(category);
        _unitOfWork.complete();
        //ViewBag.Created = true;
        TempData["Created"] = "Created";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
            return NotFound();

        var category = _unitOfWork.CategoryRepository.GetFirstOrDefault(x => x.Id == id);

        return View(category);

    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public IActionResult Edit(Category category)
    {

        _unitOfWork.CategoryRepository.Update(category);
        _unitOfWork.complete();
        TempData["Updated"] = "Updated";
        return RedirectToAction("Index");

    }

    [HttpGet]
    public IActionResult Remove(int? id)
    {
        if (id == null || id == 0)
            return NotFound();

        var category = _unitOfWork.CategoryRepository.GetFirstOrDefault(x => x.Id == id);

        return View(category);

    }
    [ValidateAntiForgeryToken]
    [HttpPost]
    public IActionResult RemoveCategory(int? id)
    {

        var category = _unitOfWork.CategoryRepository.GetFirstOrDefault(x => x.Id == id);

        if (category == null)
            return NotFound();

        _unitOfWork.CategoryRepository.Remove(category);
        _unitOfWork.complete();
        //ViewBag.Deleted = true;
        TempData["Deleted"] = "Deleted";
        return RedirectToAction("Index");

    }





}
