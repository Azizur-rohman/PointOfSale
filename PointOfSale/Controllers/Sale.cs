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
    public class SaleController : Controller
    {
        PosEntities db = new PosEntities();
        // GET: Sale
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetAllProduct()
        {
            var dataList = (from prd in db.Products
                            select new
                            {
                                Id = prd.Id,
                                Name = prd.ProductName,
                                Status = prd.IsActive,
                                SalesPrice = prd.UnitPrice
                            }).ToList();


            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Invoice()
        {
            ViewBag.InvoiceNum = Guid.NewGuid();
            return View();
        }
        [HttpPost]
        public JsonResult SaveInvoiceSale(SalesMaster salesMaster, List<SalesDetail> salesDetails)
        {
            PointOfSale.Helper.AppHelper.ReturnMessage retMessage = new AppHelper.ReturnMessage();
            retMessage.IsSuccess = true;

            foreach (var item in salesDetails)
            {
                salesMaster.SalesDetails.Add(new SalesDetail { ProductId = item.Id, TotalPrice = item.TotalPrice, Quantity = item.Quantity, IsActive = true,
            });
            }
            salesMaster.IsActive = true;
            db.SalesMasters.Add(salesMaster);
            retMessage.Messagae = "Save Success!";
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                retMessage.IsSuccess = false;
            }

            return Json(retMessage, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult EditInvoiceSale(SalesMaster salesMaster, List<SalesDetail> salesDetails, List<int> deleted)
        {
            PointOfSale.Helper.AppHelper.ReturnMessage retMessage = new AppHelper.ReturnMessage();
            retMessage.IsSuccess = true;

            if (deleted != null)
            {
                foreach (var item in deleted)
                {
                    var data = db.SalesDetails.Where(x => x.Id == item).SingleOrDefault(); ;
                    db.SalesDetails.Remove(data);
                }
            }

            foreach (var item in salesDetails)
            {
                if (item.Id > 0)
                {
                    SalesDetail s = db.SalesDetails.Where(x => x.Id == item.Id).SingleOrDefault();
                    s.TotalPrice = item.TotalPrice;
                    s.ProductId = item.ProductId;
                    s.Quantity = item.Quantity;
                    s.IsActive = true;
                    s.SalesMaster.IsActive = true;
                    s.SalesMaster.Date = salesMaster.Date;
                    s.SalesMaster.TotalQuantity = salesMaster.TotalQuantity;
                    s.SalesMaster.TotalPrice = salesMaster.TotalPrice;
                    db.Entry(s).State = EntityState.Modified;
                    retMessage.Messagae = "Update Success!";
                }
                else
                {
                   SalesDetail s = new SalesDetail { ProductId = item.ProductId, Id = salesMaster.Id, TotalPrice = item.TotalPrice, Quantity = item.Quantity, IsActive = true,
                    };
                    db.SalesDetails.Add(s);
                    retMessage.Messagae = "Save Success!";
                }
            }


            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                retMessage.IsSuccess = false;
            }

            return Json(retMessage, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InvoiceList()
        {
            var dataList = db.SalesMasters.Where(x => x.IsActive == true);
            return View(dataList);
        }
        [HttpGet]
        public JsonResult GetAllSales()
        {
            var dataList = db.SalesMasters.ToList();
            var modefiedData = dataList.Select(x => new
            {
                SaleId = x.Id,
                OrderDate = x.Date,
                TotalAmout = x.TotalPrice
            }).ToList();
            return Json(modefiedData, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetInvoiceBySalesId(int saleId)
        {
            List<SalesMaster> dataList = (from sd in db.SalesDetails.ToList()
                                   join s in db.SalesMasters on sd.Id equals s.Id
                                   where sd.Id == saleId
                                   select new SalesMaster
                                   {
                                   }).ToList();
            return Json(dataList, JsonRequestBehavior.AllowGet);
        }
    }
}