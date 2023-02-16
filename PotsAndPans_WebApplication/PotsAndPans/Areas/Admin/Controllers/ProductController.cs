using PotsAndPans.DataAccess;
using PotsAndPans.DataAccess.Repository.IRepository;
using PotsAndPans.Models.Models;
using PotsAndPans.Models.Models.ViewModels;
using PotsAndPans.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace PotsAndPans.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class ProductController : Controller
{
    //Get Access To Database
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnviroment;
    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnviroment)
    {
        _unitOfWork = unitOfWork;
        _hostEnviroment = hostEnviroment;   
    }

    //HomePg Categories 
    public IActionResult Index()
    {
        //List view of created Categories 
        return View();
    }
 
    //GET - EditPg
    public IActionResult Upsert(int? id)
    {
        ProductVM productVM = new()
        {
            Product = new(),
            CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }),
        };
        //Select List Items from Categorys
   

        //If id of category == null then Invalid  object not to be found 
        if (id == null || id == 0)
        {
            //Create Product
            //ViewBag.CategoryList = CategoryList;    
            return View(productVM);
        }
        else
        {
            productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            return View(productVM);
            //Update Product
        }
    }

    //Post - EditPg
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(ProductVM obj, IFormFile? file)
    {
        //CheckIf Validation meets standards proccess the data to be saved
        if (ModelState.IsValid)
        {
            string wwwRootPath = _hostEnviroment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\products");
                var extension = Path.GetExtension(file.FileName);

                if (obj.Product.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                obj.Product.ImageUrl = @"\images\products\" + fileName + extension;
            }
            if (obj.Product.Id == 0)
            {
                _unitOfWork.Product.Add(obj.Product);
            }
            else
            {
                _unitOfWork.Product.Update(obj.Product);

            }

            _unitOfWork.Save();
            TempData["success"] = "Product created successfully";
            return RedirectToAction("Index");
        }
        return View(obj);
    }

    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var productList = _unitOfWork.Product.GetAll(includeProperties:"Category"); 
        return Json(new { data = productList });
    }

    //Post - DeletePg
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new {success = false, message ="Error while deleteing"});
        }

        var oldImagePath = Path.Combine(_hostEnviroment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }

        _unitOfWork.Product.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });
    }
    #endregion  
}
