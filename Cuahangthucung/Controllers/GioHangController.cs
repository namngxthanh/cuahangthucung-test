using Cuahangthucung.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cuahangthucung.Controllers
{
    public class GioHangController : Controller
    {
        CuahangTCEntities db = new CuahangTCEntities();
        // GET: GioHang

        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;

            }
            return lstGioHang;
        }
        public ActionResult ThemGiohang(int ms, string url)
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
            }

            GioHang sp = lstGioHang.Find(n => n.iMaTC == ms);
            if (sp == null)
            {
                sp = new GioHang(ms);
                lstGioHang.Add(sp);
                Session["GioHang"] = lstGioHang;
                TempData["ThongBao"] = "Thêm sản phẩm vào giỏ hàng thành công!";
            }
            else
            {
                sp.iSoLuong++;
            }
            return Redirect(url);
        }
        //Tong so luong
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }
        //tong tien
        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.dThanhTien);
            }
            return dTongTien;
        }
        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "User");
            }
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"].ToString();
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
        public ActionResult GioHangPartial()
        {

            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();

            return PartialView();
        }
        //Xóa sản phẩm khỏi giỏ hàng
        public ActionResult XoaSPKhoiGioHang(int iMaTC)
        {
            //Lấy giỏ hàng

            List<GioHang> lstGioHang = LayGioHang();

            //Kiểm tra sản phẩm đã có trong giỏ hàng chưa
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iMaTC == iMaTC);

            //Xóa sản phẩm khỏi giỏ hàng
            if (sp != null)
            {
                lstGioHang.RemoveAll(n => n.iMaTC == iMaTC);
                if (lstGioHang.Count == 0)
                {
                    return RedirectToAction("Index", "User");
                }
            }

            //Cập nhật lại giỏ hàng
            return RedirectToAction("GioHang");
        }
        //Cập nhật giỏ hàng
        public ActionResult CapNhatGioHang(int iMaTC, FormCollection f)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iMaTC == iMaTC);

            // Nếu tồn tại thì cho sửa số lượng
            if (sp != null)
            {
                sp.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
                return RedirectToAction("GioHang");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }
        public ActionResult XoaGioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            // Kiểm tra đăng nhập chưa
            if (Session["Taikhoanuser"] == null || Session["Taikhoanuser"].ToString() == "")
            {
                // In giá trị Session để kiểm tra
                return RedirectToAction("DangNhap", "GioHang");
            }

            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "User");
            }

            // Lấy hàng từ Session
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();

            return View(lstGioHang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            // Thêm đơn hàng
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoanuser"];
            List<GioHang> lstGioHang = LayGioHang();
            ddh.MaKH = kh.MaKH;
            ddh.NgayDat = DateTime.Now;
            var NgayGiao = String.Format("{0:MM/dd/yyyy}", f["NgayGiao"]);
            ddh.NgayGiao = DateTime.Parse(NgayGiao);
            ddh.TinhTrangGiaoHang = 1;
            ddh.DaThanhToan = false;
            db.DONDATHANGs.Add(ddh);
            db.SaveChanges();

            // Thêm chi tiết đơn hàng
            foreach (var item in lstGioHang)
            {
                CHITIETDATHANG ctdh = new CHITIETDATHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaTC = item.iMaTC;
                ctdh.SoLuong = item.iSoLuong;
                ctdh.DonGia = (decimal)item.dDonGia;
                db.CHITIETDATHANGs.Add(ctdh);
            }
            db.SaveChanges();
            Session["GioHang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }
        public ActionResult XacNhanDonHang()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var sTenDN = collection["TenDN"];
            var sMatkhau = collection["MatKhau"];
            var url = collection["url"];
            if (String.IsNullOrEmpty(url))
            {
                url = "~/SachOnline/Index";
            }
            if (String.IsNullOrEmpty(sTenDN))
            {

                ViewData["Err1"] = "Bạn chưa nhập tên đăng nhập";
            }

            else if (String.IsNullOrEmpty(sMatkhau))
            {
                ViewData["Err2"] = "Phải nhập mật khẩu";
            }
            else
            {

                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.TaiKhoan == sTenDN && n.MatKhau == sMatkhau);
                if (kh != null)

                {

                    ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    Session["Taikhoanuser"] = kh;
                }
                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return RedirectToAction("DatHang", "GioHang");
        }

    }
}