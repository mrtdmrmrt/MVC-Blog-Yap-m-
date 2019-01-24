using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcBlog.Models;

namespace MvcBlog.Controllers
{
    public class AdminYorumController : Controller
    {
        private mvcblogDB db = new mvcblogDB();

        // GET: AdminYorum
        public ActionResult Index()
        {
            var yorums = db.Yorums.Include(y => y.Makale).Include(y => y.Uye);
            return View(yorums.OrderByDescending(y=>y.YorumID).ToList());
        }

        // GET: AdminYorum/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Yorum yorum = db.Yorums.Find(id);
            if (yorum == null)
            {
                return HttpNotFound();
            }
            return View(yorum);
        }

        // GET: AdminYorum/Create
        public ActionResult Create()
        {
            ViewBag.MakaleID = new SelectList(db.Makales, "MakaleID", "Baslık");
            ViewBag.UyeID = new SelectList(db.Uyes, "UyeID", "KullaniciAdi");
            return View();
        }

        // POST: AdminYorum/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "YorumID,Icerik,UyeID,MakaleID,Tarih")] Yorum yorum)
        {
            if (ModelState.IsValid)
            {
                db.Yorums.Add(yorum);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MakaleID = new SelectList(db.Makales, "MakaleID", "Baslık", yorum.MakaleID);
            ViewBag.UyeID = new SelectList(db.Uyes, "UyeID", "KullaniciAdi", yorum.UyeID);
            return View(yorum);
        }

        // GET: AdminYorum/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Yorum yorum = db.Yorums.Find(id);
            if (yorum == null)
            {
                return HttpNotFound();
            }
            ViewBag.MakaleID = new SelectList(db.Makales, "MakaleID", "Baslık", yorum.MakaleID);
            ViewBag.UyeID = new SelectList(db.Uyes, "UyeID", "KullaniciAdi", yorum.UyeID);
            return View(yorum);
        }

        // POST: AdminYorum/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "YorumID,Icerik,UyeID,MakaleID,Tarih")] Yorum yorum)
        {
            if (ModelState.IsValid)
            {
                db.Entry(yorum).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MakaleID = new SelectList(db.Makales, "MakaleID", "Baslık", yorum.MakaleID);
            ViewBag.UyeID = new SelectList(db.Uyes, "UyeID", "KullaniciAdi", yorum.UyeID);
            return View(yorum);
        }

        // GET: AdminYorum/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Yorum yorum = db.Yorums.Find(id);
            if (yorum == null)
            {
                return HttpNotFound();
            }
            return View(yorum);
        }

        // POST: AdminYorum/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Yorum yorum = db.Yorums.Find(id);
            db.Yorums.Remove(yorum);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
