using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBlog.Models;
using System.Web.Helpers;
using System.IO;

namespace MvcBlog.Controllers
{

    public class UyeController : Controller
    {
        mvcblogDB db = new mvcblogDB();
        // GET: Uye
        public ActionResult Index(int id)
        {
            var uye = db.Uyes.Where(u => u.UyeID == id).SingleOrDefault();
            if (Convert.ToInt32(Session["uyeId"]) != uye.UyeID)
            {
                return HttpNotFound();
            }
            return View(uye);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Uye uye, string Sifre)
        {
            var md5pass = Crypto.Hash(Sifre, "MD5");

            var login = db.Uyes.Where(u => u.KullaniciAdi == uye.KullaniciAdi).SingleOrDefault();
            if (login.KullaniciAdi == uye.KullaniciAdi && login.Email == uye.Email && login.Sifre == md5pass)//uye.Sifre
            {
                Session["uyeId"] = login.UyeID;
                Session["Kullaniciadi"] = login.KullaniciAdi;
                Session["YetkiId"] = login.YetkiID;
                return RedirectToAction("Index", "Home");

            }
            else
            {
                ViewBag.Uyari = "Kullanıcı Adı, Mail yada Şifrenizi Kontrol Ediniz!!!";
                return View();
            }

        }
        public ActionResult Logout()
        {
            Session["uyeId"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Uye uye, string Sifre, HttpPostedFileBase Foto)
        {
 
            var md5pass = Sifre;
            
            var login = db.Uyes.Where(u => u.KullaniciAdi == uye.KullaniciAdi).FirstOrDefault();
            if (ModelState.IsValid)
            {

                if (login == null)
                {

                    if (Foto != null)
                    {
                        WebImage img = new WebImage(Foto.InputStream);
                        FileInfo fotoinfo = new FileInfo(Foto.FileName);

                        string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                        img.Resize(150, 150);
                        img.Save("~/Uploads/UyeFoto/" + newfoto);
                        uye.Foto = "/Uploads/UyeFoto/" + newfoto;
                        uye.YetkiID = 2;
                        uye.Sifre = Crypto.Hash(md5pass, "MD5");
                        db.Uyes.Add(uye);
                        db.SaveChanges();
                        Session["uyeId"] = uye.UyeID;
                        Session["Kullaniciadi"] = uye.KullaniciAdi;
                        return RedirectToAction("Index", "Home");

                    }
                    else
                    {
                        ViewBag.Uyari = "Fotoğraf Seçiniz..!!!";

                        //uye.Foto = "/Uploads/UyeFoto/varsayilan.png";
                        //ModelState.AddModelError("Fotoğraf", "Fotoğraf Seçiniz..");

                        //WebImage img = new WebImage(uye.Foto);
                        //FileInfo fotoinfo = new FileInfo(uye.Foto);

                        //string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                        //img.Resize(150, 150);
                        //img.Save("~/Uploads/UyeFoto/varsayilan.png" + newfoto);

                    }

                }
                else
                {
                    ViewBag.Uyari = "Bu Kullanıcı Adı Kullanılmaktadır!!!";
                }

            }
            return View(uye);
        }
        public ActionResult Edit(int id)
        {
            var uye = db.Uyes.Where(u => u.UyeID == id).SingleOrDefault();
            if (Convert.ToInt32(Session["uyeId"]) != uye.UyeID)
            {
                return HttpNotFound();
            }
            return View(uye);
        }
        [HttpPost]
        public ActionResult Edit(Uye uye, string Sifre, int id, HttpPostedFileBase Foto)
        {
            if (ModelState.IsValid)
            {
                var md5pass = Sifre;
                var uyes = db.Uyes.Where(u => u.UyeID == id).SingleOrDefault();
                if (Foto != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(uye.Foto)))
                    {
                        System.IO.File.Delete(Server.MapPath(uyes.Foto));
                    }
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);

                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(150, 150);
                    img.Save("~/Uploads/UyeFoto/" + newfoto);
                    uyes.Foto = "/Uploads/UyeFoto/" + newfoto;
                }
                uyes.AdSoyad = uye.AdSoyad;
                uyes.KullaniciAdi = uye.KullaniciAdi;
                uyes.Sifre = Crypto.Hash(md5pass, "MD5");//uye.Sifre
                uyes.Email = uye.Email;
                db.SaveChanges();

                Session["Kullaniciadi"] = uye.KullaniciAdi;
                return RedirectToAction("Index", "Home", new { id = uye.UyeID });

            }
            return View();
        }
        public ActionResult UyeProfil(int id)
        {
            var uye = db.Uyes.Where(u => u.UyeID == id).SingleOrDefault();
            return View();
        }
    }
}