using PotsAndPans.DataAccess;
using PotsAndPans.DataAccess.Repository.IRepository;
using PotsAndPans.Models;
using PotsAndPans.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PotsAndPans.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin )]
public class CategoryController : Controller
{
    //Get Access To Database
    private readonly IUnitOfWork _unitOfWork;

    public CategoryController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    //HomePg Categories 
    public IActionResult Index()
    {
        //List view of created Categories 
        IEnumerable<Category> objCategoriesList = _unitOfWork.Category.GetAll();
        return View(objCategoriesList);
    }

    //GET - CreatePg
    public IActionResult Create()
    {
        return View();
    }

    //POST - Create Category Object Item
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Category Obj)
    {
        //Custom Validation - Object Name cannot be the same as Order Number 
        //if (Obj.Name == Obj.DisplayOrderNumber.ToString())
        //{
        //    ModelState.AddModelError("name", "The Display Order Number cannot match the Name!!");
        //}
        //If Validation meets standards proccess the data to be saved
        if (ModelState.IsValid)
        {
            _unitOfWork.Category.Add(Obj);
            _unitOfWork.Save();
            TempData["success"] = "Category created successfully";
            return RedirectToAction("Index");
        }

        return View(Obj);
    }

    //GET - EditPg
    public IActionResult Edit(int? id)
    {
        //If id of category == null then Invalid  object not to be found 
        if (id == null || id == 0)
        {
            return NotFound();
        }

        //Extract Category selected by Id 
        //var categoryFromDb = _db.Categories.Find(id);
        var categoryFromDbFirst = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
        //var categoryFromDbFirst = _db.Categories.SingleOrDefault(u => u.Id == id);

        if (categoryFromDbFirst == null)
        {
            return NotFound();
        }

        return View(categoryFromDbFirst);
    }

    //Post - EditPg
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Category Obj)
    {
        //Custom Validation - Object Name cannot be the same as Order Number 
        //if (Obj.Name == Obj.DisplayOrderNumber.ToString())
        //{
        //    ModelState.AddModelError("name", "The Display Order Number cannot match the Name!!");
        //}
        //CheckIf Validation meets standards proccess the data to be saved
        if (ModelState.IsValid)
        {
            _unitOfWork.Category.Update(Obj);
            _unitOfWork.Save();
            TempData["success"] = "Category updated successfully";
            return RedirectToAction("Index");
        }

        return View(Obj);
    }

    //GET - DeletePg
    public IActionResult Delete(int? id)
    {
        //If id of category == null then Invalid  object not to be found 
        if (id == null || id == 0)
        {
            return NotFound();
        }

        //Extract Category selected by Id 
        //var categoryFromDb = _db.Categories.Find(id);
        var categoryFromDbFirst = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
        //var categoryFromDbFirst = _db.Categories.SingleOrDefault(u => u.Id == id);

        if (categoryFromDbFirst == null)
        {
            return NotFound();
        }

        return View(categoryFromDbFirst);
    }

    //Post - EditPg
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(int? id)
    {
        var obj = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return NotFound();
        }
        _unitOfWork.Category.Remove(obj);
        _unitOfWork.Save();
        TempData["success"] = "Category deleted successfully";
        return RedirectToAction("Index");
    }
}
