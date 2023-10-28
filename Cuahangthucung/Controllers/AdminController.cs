using Cuahangthucung.Models;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Net;
using System.Data.Entity;
using static System.Net.Mime.MediaTypeNames;

namespace Cuahangthucung.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
      private CuahangTCEntities db = new CuahangTCEntities();
        public ActionResult Index()
        {
            //if (Session["TenDN"] != null)
                return View();
            //else
              //  return RedirectToAction("Login");

        }
        public ActionResult Pet(int? page)
        {
              if (Session["TaikhoanAdmin"] == null)
              return RedirectToAction("Login", "Admin");
             int pageNumber = (page ?? 1);
             int pageSize = 3;
            //return View(db.THUCUNGs.ToList());
            return View(db.THUCUNGs.ToList().OrderBy(n => n.MaTC).ToPagedList(pageNumber, pageSize));
           
        }
     
        
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn) == true)
            {
                ViewBag.Loi1 = "Hãy nhập username";
            }
            if (String.IsNullOrEmpty(matkhau) == true)
            {
                ViewBag.Loi2 = "Hãy nhạp password";
            }
            else
            {
                ADMIN ad = db.ADMINs.SingleOrDefault(n => n.TenDN == tendn && n.MatKhau == matkhau);
                KHACHHANG us = db.KHACHHANGs.SingleOrDefault(n => n.TaiKhoan == tendn && n.MatKhau == matkhau);
                if (ad != null)
                {
                    Session["TaikhoanAdmin"] = ad;
                    return RedirectToAction("Pet", "Admin");
                }
                else if (us != null)
                {
                    Session["TaikhoanUser"] = us;
                    return RedirectToAction("Index", "User");
                }
                else { ViewBag.Thongbao = "Username hoặc Password không đúng"; }
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Admin");
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.MaNCC = new SelectList(db.NHACUNGCAPs.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            ViewBag.MaLoaiTC = new SelectList(db.LOAITHUCUNGs.ToList().OrderBy(n => n.TenLoaiTC), "MaLoaiTC", "TenLoaiTC");
           //ViewBag.MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC");
            //ViewBag.MaTC = new SelectList(db.LOAITHUCUNGs, "MaTC", "TenTC");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(THUCUNG thucung, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid == true)
            {
                try
                {
                    if (Image.ContentLength > 0)
                    {
                        string filename = Path.GetFileName(Image.FileName);
                        string path = Path.Combine(Server.MapPath("~/Image"), filename);
                        Image.SaveAs(path);
                        thucung.AnhBia = filename;
                    }
                    db.THUCUNGs.Add(thucung);
                    db.SaveChanges();
                    return RedirectToAction("Pet");
                }
                catch { ViewBag.Mesage = "Không thành công"; }
            }

           // ViewBag.MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC", thucung.MaNCC);
            //ViewBag.MaloaiTC = new SelectList(db.LOAITHUCUNGs, "MaLoaiTC", "TenLoaiTC", thucung.MaLoaiTC);
            ViewBag.MaNCC = new SelectList(db.NHACUNGCAPs.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            ViewBag.MaLoaiTC = new SelectList(db.LOAITHUCUNGs.ToList().OrderBy(n => n.TenLoaiTC), "MaLoaiTC", "TenLoaiTC");
            return View(thucung);
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            THUCUNG thucung = db.THUCUNGs.Find(id);
            if (thucung == null)
            {
                return HttpNotFound();
            }
            return View(thucung);
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            THUCUNG thucung = db.THUCUNGs.Find(id);
            if (thucung == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC", thucung.MaNCC);
            ViewBag.MaLoaiTC = new SelectList(db.LOAITHUCUNGs, "MaLoaiTC", "TenLoaiTC", thucung.MaLoaiTC);
            return View(thucung);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(THUCUNG thucung, HttpPostedFileBase Images, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Images != null)
                    {
                        string _FileName = Path.GetFileName(Images.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Image"), _FileName);
                        Images.SaveAs(_path);
                        thucung.AnhBia = _FileName;
                        // get Path of old image for deleting it
                        _path = Path.Combine(Server.MapPath("~/Sach"), form["oldimage"]);
                        if (System.IO.File.Exists(_path))
                            System.IO.File.Delete(_path);

                    }
                    else
                        thucung.AnhBia = form["oldimage"];
                    db.Entry(thucung).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.Message = "không thành công!!";
                }

                return RedirectToAction("pet");
            }
            ViewBag.MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC", thucung.MaNCC);
            ViewBag.MaLoaiTC = new SelectList(db.LOAITHUCUNGs, "MaLoaiTC", "TenLoaiTC", thucung.MaLoaiTC);
            return View(thucung);
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            THUCUNG thucung = db.THUCUNGs.SingleOrDefault(n => n.MaTC == id);
            ViewBag.BooID = thucung.MaTC;
            if (thucung == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(thucung);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult XacNhanxoa(int id)
        {
            THUCUNG thucung = db.THUCUNGs.SingleOrDefault(n => n.MaTC == id);
            ViewBag.BooID = thucung.MaTC;
            if (thucung == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.THUCUNGs.Remove(thucung);
            db.SaveChanges();
            return RedirectToAction("Pet");
        }
    }
}