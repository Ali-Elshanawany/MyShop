using Microsoft.AspNetCore.Mvc;
using MyShop.Web.Data;
using MyShop.Web.Models;

namespace MyShop.Web.Controllers;

public class CategoryController : Controller
{

    private readonly ApplicationDbContext _context;

    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        IEnumerable<Category> categories = _context.Categories.ToList();

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
        _context.Categories.Add(category);
        _context.SaveChanges();
        //ViewBag.Created = true;
        TempData["Created"] = "Created";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
            return NotFound();

        var category = _context.Categories.FirstOrDefault(c => c.Id == id);

        return View(category);

    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public IActionResult Edit(Category category)
    {

        _context.Categories.Update(category);
        _context.SaveChanges();
        TempData["Updated"] = "Updated";
        return RedirectToAction("Index");

    }

    [HttpGet]
    public IActionResult Remove(int? id)
    {
        if (id == null || id == 0)
            return NotFound();

        var category = _context.Categories.FirstOrDefault(c => c.Id == id);

        return View(category);

    }
    [ValidateAntiForgeryToken]
    [HttpPost]
    public IActionResult RemoveCategory(int? id)
    {

        var category = _context.Categories.FirstOrDefault(c => c.Id == id);

        if (category == null)
            return NotFound();

        _context.Categories.Remove(category);
        _context.SaveChanges();
        //ViewBag.Deleted = true;
        TempData["Deleted"] = "Deleted";
        return RedirectToAction("Index");

    }





}
