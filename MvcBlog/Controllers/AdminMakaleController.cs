using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBlog.Models;
using System.Web.Helpers;
using System.IO;
using PagedList;
using PagedList.Mvc;

namespace MvcBlog.Controllers
{
    public class AdminMakaleController : Controller
    {
        mvcblogDB db = new mvcblogDB();
        // GET: AdminMakale
        public ActionResult MakaleIndex(int Page=1)
        {
            var makales = db.Makales.OrderByDescending(m=>m.MakaleID).ToPagedList(Page,10);
            return View(makales);
        }

        // GET: AdminMakale/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminMakale/Create
        public ActionResult Create()
        {
            ViewBag.KategoriID = new SelectList(db.Kategoris, "KategoriID", "KategoriAdi");
            return View();
        }

        // POST: AdminMakale/Create
        [HttpPost]
        public ActionResult Create(Makale Makale,string Etiketler,HttpPostedFileBase Foto)
        {
            if(ModelState.IsValid)
            {
                if (Foto != null)
                {
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);

                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/MakaleFoto/" + newfoto);
                    Makale.Foto = "/Uploads/MakaleFoto/" + newfoto;
                    

                }
                if (Etiketler != null)
                {
                    string[] etiketdizi = Etiketler.Split(',');
                    foreach (var i in etiketdizi)
                    {
                        var yenietiket = new Etiket { EtiketAdi = i };
                        db.Etikets.Add(yenietiket);
                        Makale.Etikets.Add(yenietiket);
                    }
                }
                Makale.UyeID =Convert.ToInt32( Session["uyeId"]);
                Makale.Okunma = 1;
                db.Makales.Add(Makale);
                db.SaveChanges();

                return RedirectToAction("MakaleIndex");
            }
            return View(Makale);
            
        }

        // GET: AdminMakale/Edit/5
        public ActionResult Edit(int id)
        {
            var makale = db.Makales.Where(m => m.MakaleID == id).SingleOrDefault();
            if(makale==null)
            {
                return HttpNotFound();
            }
            ViewBag.KategoriID = new SelectList(db.Kategoris, "KategoriID", "KategoriAdi", makale.KategoriID);
            return View(makale);
        }

        // POST: AdminMakale/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, HttpPostedFileBase Foto, Makale makale)
        {
            try
            {
                var makales = db.Makales.Where(m => m.MakaleID == id).SingleOrDefault();
                if(Foto != null)
                {
                    if(System.IO.File.Exists(Server.MapPath(makales.Foto)))
                    {
                        System.IO.File.Delete(Server.MapPath(makales.Foto));
                    }
                   
                    makales.Baslık = makale.Baslık;
                    makales.Icerik = makale.Icerik;
                    makales.UyeID = makale.UyeID;
                    makales.KategoriID = makale.KategoriID;
                    db.SaveChanges();
                    return RedirectToAction("MakaleIndex");
                }
                WebImage img = new WebImage(Foto.InputStream);
                FileInfo fotoinfo = new FileInfo(Foto.FileName);

                string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                img.Resize(800, 350);
                img.Save("~/Uploads/MakaleFoto/" + newfoto);
                makales.Foto = "/Uploads/MakaleFoto/" + newfoto;
                return View();
                
            }
            catch
            {
                ViewBag.KategoriID = new SelectList(db.Kategoris, "KategoriID", "KategoriAdi", makale.KategoriID);
                return View(makale);
            }
        }

        // GET: AdminMakale/Delete/5
        public ActionResult Delete(int id)
        {
            var makale = db.Makales.Where(m => m.MakaleID == id).SingleOrDefault();
            if(makale== null)
            {
                return HttpNotFound();
            }
            return View(makale);
        }

        // POST: AdminMakale/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var makales = db.Makales.Where(m => m.MakaleID == id).SingleOrDefault();
                if (makales == null)
                {
                    return HttpNotFound();
                }
                if (System.IO.File.Exists(Server.MapPath(makales.Foto)))
                {
                    System.IO.File.Delete(Server.MapPath(makales.Foto));
                }
                foreach (var i in makales.Yorums.ToList())
                {
                    db.Yorums.Remove(i);
                }
                foreach (var i in makales.Etikets.ToList())
                {
                    db.Etikets.Remove(i);
                }
                db.Makales.Remove(makales);
                db.SaveChanges();

                return RedirectToAction("MakaleIndex");
            }
            catch
            {
                return View();
            }
        }
    }
}
