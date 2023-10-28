using Cuahangthucung.Models;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cuahangthucung.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        private CuahangTCEntities db = new CuahangTCEntities();
        private List<THUCUNG> Laypetmoi(int count)
        {
            return db.THUCUNGs.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }
        public ActionResult Index(int ? page )
        {
            if (Session["Taikhoanuser"] == null)
                return RedirectToAction("Login", "Admin");
            int pageNumber = (page ?? 1);
            int pageSize = 2;
            //return View(db.Books.ToList());
            return View(db.THUCUNGs.ToList().OrderBy(keySelector: n => n.MaTC).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Details(int id)
        {
            var Thucung = from s in db.THUCUNGs
                       where s.MaTC == id
                       select s;
            return View(Thucung.Single());
        }
    }
}