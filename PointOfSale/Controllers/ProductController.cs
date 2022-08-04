using PointOfSale.Helper;
using PointOfSale.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace POS.Controllers
{
    public class ProductController : Controller
    {
        PosEntities db = new PosEntities();

        [HttpPost]
        public JsonResult SaveProduct(Product cat)
        {
            bool isSuccess = true;
            if (cat.Id > 0)
            {
                db.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat.IsActive = true;
                db.Products.Add(cat);
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAllProduct()
        {
            var dataList = db.Products.Where(x => x.IsActive == true).ToList();
            var data = dataList.Select(x => new
            {
                Id = x.Id,
                ProductName = x.ProductName,
                ProductCode = x.ProductCode,
                UnitPrice = x.UnitPrice
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Product()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.Messages = TempData["SuccessMessage"];
            var dataList = db.Products.Where(x => x.IsActive == true);
            return View(dataList);
        }
        public ActionResult ProductCreate()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.Messages = TempData["SuccessMessage"];
            var dataList = db.Products.Where(x => x.IsActive == true);
            return View(dataList);
        }

        [HttpPost]
        public ActionResult ProductCreate(Product product)
        {
            var existingCategory = db.Products.FirstOrDefault(x => x.ProductName == product.ProductName);
            if (existingCategory != null)
            {
                if (existingCategory.IsActive == false)
                {
                    TempData["Message"] = $"{product.ProductName} product has been deleted, please contact admin";
                    return RedirectToAction("ProductCreate");
                }
                else
                {
                    TempData["Message"] = $"{product.ProductName} product already exists";
                    return RedirectToAction("ProductCreate");
                }
            }
            else
            {
                product.IsActive = true;
                db.Products.Add(product);
            }

            if (db.SaveChanges() > 0)
            {
                TempData["SuccessMessage"] = "Save Successfully";
                return RedirectToAction("ProductCreate");
            }
            else
            {
                TempData["Message"] = "Failed";
                return RedirectToAction("ProductCreate");
            }

        }

        public ActionResult ProductEdit(int id)
        {
            {
                ViewBag.Message = TempData["Message"];
                ViewBag.Product = db.Products.ToList();
                var pdt = db.Products.FirstOrDefault(x => x.Id == id);
                return View(pdt);
            }
        }

        [HttpPost]
        public ActionResult ProductEdit(int id, Product product)
        {
            if (product.Id > 0)
            {
                var existingCountry1 = db.Products.FirstOrDefault(x => x.ProductName == product.ProductName && x.Id != id);
                if (existingCountry1 != null)
                {
                    if (existingCountry1.IsActive == false)
                    {
                        TempData["Message"] = $"{product.ProductName} product has been deleted, please contact admin";
                        return RedirectToAction("ProductEdit");
                    }
                    else
                    {
                        TempData["Message"] = $"{product.ProductName} product already exists";
                        return RedirectToAction("ProductEdit");
                    }
                }

                else
                {
                    var existingCountry = db.Products.FirstOrDefault(x => x.Id == product.Id);
                    if (existingCountry != null)
                    {
                        existingCountry.ProductName = product.ProductName;
                        existingCountry.ProductCode = product.ProductCode;
                        existingCountry.UnitPrice = product.UnitPrice;
                        existingCountry.IsActive = true;
                        db.Entry(existingCountry).State = EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            TempData["SuccessMessage"] = "Edited Successfully";

                            return RedirectToAction("ProductCreate");
                        }
                        else
                        {
                            TempData["Message"] = "Failed";
                            return RedirectToAction("ProductCreate");
                        }
                    }
                    return RedirectToAction("ProductEdit");
                }
            }
            return View(product);
        }

        [HttpPost]
        public JsonResult DeleteProduct(int ProductId)
        {
            bool result = false;
            Product s = db.Products.Where(x => x.Id == ProductId).SingleOrDefault();
            if (s != null)
            {
                s.IsActive = false;
                db.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}