using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PerfV400.Models;

namespace PerfV400.Controllers
{
    public class UserController : Controller
    {
        private PerfV100Entities db = new PerfV100Entities();

        //
        // GET: /User/

        [Authorize(Roles="Administrator")]
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Artist).Include(u => u.CountryRegion).Include(u => u.Language);
            return View(users.ToList());
        }

        //
        // GET: /User/Details/5

        [Authorize(Roles = "Administrator")]
        public ActionResult Details(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // GET: /User/Create

        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            ViewBag.User_ArtistId = new SelectList(db.Artists, "Artist_Id", "Artist_FirstName");
            ViewBag.User_CountryId = new SelectList(db.CountryRegions, "CountryRegion_Id", "CountryRegion_Code");
            ViewBag.User_LanguageId = new SelectList(db.Languages, "Language_Id", "Language_Name");
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.User_ArtistId = new SelectList(db.Artists, "Artist_Id", "Artist_FirstName", user.User_ArtistId);
            ViewBag.User_CountryId = new SelectList(db.CountryRegions, "CountryRegion_Id", "CountryRegion_Code", user.User_CountryId);
            ViewBag.User_LanguageId = new SelectList(db.Languages, "Language_Id", "Language_Name", user.User_LanguageId);
            return View(user);
        }

        //
        // GET: /User/Edit/5

        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.User_ArtistId = new SelectList(db.Artists, "Artist_Id", "Artist_FirstName", user.User_ArtistId);
            ViewBag.User_CountryId = new SelectList(db.CountryRegions, "CountryRegion_Id", "CountryRegion_Code", user.User_CountryId);
            ViewBag.User_LanguageId = new SelectList(db.Languages, "Language_Id", "Language_Name", user.User_LanguageId);
            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.User_ArtistId = new SelectList(db.Artists, "Artist_Id", "Artist_FirstName", user.User_ArtistId);
            ViewBag.User_CountryId = new SelectList(db.CountryRegions, "CountryRegion_Id", "CountryRegion_Code", user.User_CountryId);
            ViewBag.User_LanguageId = new SelectList(db.Languages, "Language_Id", "Language_Name", user.User_LanguageId);
            return View(user);
        }

        //
        // GET: /User/Delete/5

        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}