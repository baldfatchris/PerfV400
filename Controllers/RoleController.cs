using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PerfV400.Models;
using System.Data.Objects.SqlClient;
using System.IO;

namespace PerfV400.Controllers
{
    public class RoleController : Controller
    {

        public const int PageSize = 20;

        private PerfV100Entities db = new PerfV100Entities();

        //
        // GET: /Role/

        public ActionResult Index()
        {
            var roles = db.Roles.Include(r => r.Genre).Include(r => r.Piece).Include(r => r.Type);
            return View(roles.ToList());
        }

        //
        // GET: /Role/Details/5

        public ActionResult Details(int id = 0)
        {
            Role role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }

            // sort out the return url
            ViewBag.ReturnUrl = Url.Action(string.Format("Details/{0}", id), "Role");

            // sort out the paging
            ViewBag.page = 0;
            ViewBag.PageSize = PageSize;

            // data for the Venue dropdown
            IEnumerable<SelectListItem> Venues = db.Venues
            .Where(v => db.PerformanceArtists.Where(pa => pa.PerformanceArtist_RoleId == id).Select(pa => pa.Performance.Event.Event_VenueId).Contains(v.Venue_Id))
            .OrderBy(g => g.Venue_Name)
            .Select(g => new SelectListItem
            {
                Value = SqlFunctions.StringConvert((double)g.Venue_Id),
                Text = g.Venue_Name
            })
            ;
            ViewBag.Venues = Venues;

            // data for the City dropdown
            IEnumerable<SelectListItem> cities = db.Venues
            .Where(v => db.PerformanceArtists.Where(pa => pa.PerformanceArtist_RoleId == id).Select(pa => pa.Performance.Event.Event_VenueId).Contains(v.Venue_Id))
            .OrderBy(v => v.Venue_City)
            .Select(v => new SelectListItem
            {
                Value = v.Venue_City,
                Text = v.Venue_City
            })
            .Distinct();
            ViewBag.Cities = cities;

            // data for the Country dropdown
            IEnumerable<SelectListItem> countries = db.CountryRegions
            .Where(c => db.PerformanceArtists.Where(pa => pa.PerformanceArtist_RoleId == id).Select(pa => pa.Performance.Event.Venue.Venue_CountryId).Contains(c.CountryRegion_Id))
            .OrderBy(c => c.CountryRegion_Name)
            .Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert((double)c.CountryRegion_Id),
                Text = c.CountryRegion_Name
            });
            ViewBag.Countries = countries;




            return View(role);
        }

 

        public FileContentResult GetRoleImage(int id)
        {

            Default Xdefault = db.Defaults.FirstOrDefault(d => d.Default_Type == "Male");
            if (Xdefault != null)
            {
                return File(Xdefault.Default_Photo, "JPEG");

            }
            else
            {
                return null;
            }

        }




        //
        // GET: /Role/Create

        public ActionResult Create()
        {
            ViewBag.Role_GenreId = new SelectList(db.Genres, "Genre_Id", "Genre_Name");
            ViewBag.Role_PieceId = new SelectList(db.Pieces, "Piece_Id", "Piece_Name");
            ViewBag.Role_TypeId = new SelectList(db.Types, "Type_Id", "Type_Name");
            return View();
        }

        //
        // POST: /Role/Create

        [HttpPost]
        public ActionResult Create(Role role)
        {
            if (ModelState.IsValid)
            {
                db.Roles.Add(role);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Role_GenreId = new SelectList(db.Genres, "Genre_Id", "Genre_Name", role.Role_GenreId);
            ViewBag.Role_PieceId = new SelectList(db.Pieces, "Piece_Id", "Piece_Name", role.Role_PieceId);
            ViewBag.Role_TypeId = new SelectList(db.Types, "Type_Id", "Type_Name", role.Role_TypeId);
            return View(role);
        }

        //
        // GET: /Role/EditRole/5
        [Authorize]
        public ActionResult EditRole(int id)
        {
            Role role = db.Roles.Single(e => e.Role_Id == id);

            ViewBag.Role_GenreId = new SelectList(db.Genres, "Genre_Id", "Genre_Name", role.Role_GenreId);
            ViewBag.Role_PieceId = new SelectList(db.Pieces, "Piece_Id", "Piece_Name", role.Role_PieceId);
            ViewBag.Role_TypeId = new SelectList(db.Types, "Type_Id", "Type_Name", role.Role_TypeId);

            return PartialView(role);

        }

        //
        // POST: /Role/EditRole/5

        [HttpPost]
        [Authorize]
        public ActionResult EditRole(Role role)
        {
            if (ModelState.IsValid)
            {
                db.Entry(role).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.Role_GenreId = new SelectList(db.Genres, "Genre_Id", "Genre_Name", role.Role_GenreId);
                ViewBag.Role_PieceId = new SelectList(db.Pieces, "Piece_Id", "Piece_Name", role.Role_PieceId);
                ViewBag.Role_TypeId = new SelectList(db.Types, "Type_Id", "Type_Name", role.Role_TypeId);

                return PartialView("DetailsRole", role);
            }
            else
            {
                return Content("Please review your form");
            }
        }

        //
        // GET: /Role/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Role role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // POST: /Role/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Role role = db.Roles.Find(id);
            db.Roles.Remove(role);
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