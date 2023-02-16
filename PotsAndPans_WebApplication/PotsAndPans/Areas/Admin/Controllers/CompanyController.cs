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
public class CompanyController : Controller
{
    //Get Access To Database
    private readonly IUnitOfWork _unitOfWork;
    public CompanyController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View();
    }
 
    //GET
    public IActionResult Upsert(int? id)
    {
        Company company = new();
    
        if (id == null || id == 0)
        {
      
            return View(company);
        }
        else
        {
            company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
            return View(company);
        }
    }

    //Post 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(Company obj)
    {
        if (ModelState.IsValid)
        {
            if (obj.Id == 0)
            {
                _unitOfWork.Company.Add(obj);
                TempData["success"] = "Company created successfully";
            }
            else
            {
                _unitOfWork.Company.Update(obj);
                TempData["success"] = "Company updated successfully";
            }

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        return View(obj);
    }

    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var companyList = _unitOfWork.Company.GetAll(); 
        return Json(new { data = companyList });
    }

    //Post - DeletePg
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new {success = false, message ="Error while deleteing"});
        }

      

        _unitOfWork.Company.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });
    }
    #endregion  
}
