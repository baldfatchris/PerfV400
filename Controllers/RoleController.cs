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

            // sort out the paging
            ViewBag.page = 0;

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


            // set up the filters
            DateTime datetimefilter_from_Artist_Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            ViewData["filter_from_Artist_Date"] = DateTime.Now.ToLongDateString();


            // data for the Artists
            ViewBag.Artists = db.Artists
            .Where(a => db.PerformanceArtists
                .Where(pa => pa.PerformanceArtist_RoleId == id)
                .Where(pa => pa.Performance.Event.Event_Date >= datetimefilter_from_Artist_Date)
                .Select(pa => pa.PerformanceArtist_ArtistId).Contains(a.Artist_Id))
            .OrderBy(a => a.Artist_LastName).ThenBy(a => a.Artist_FirstName)
            .Take(60);


            return View(role);
        }

        public ActionResult MoreRoleArtists(
            string filter_Role_Id,
            string filter_search,
            string filter_Role_VenueId,
            string filter_Venue_City,
            string filter_Venue_CountryId,
            string filter_From_Artist_Date,
            string filter_To_Artist_Date,
            int page
            )
        {

            ViewBag.Role_Id = filter_Role_Id;

            // sort out the paging
            ViewBag.page = page;

            // set up the filters
            bool result;

            string strfilter_search = HttpUtility.UrlDecode((string)filter_search);

            int intfilter_Role_Id;
            result = int.TryParse(filter_Role_Id, out intfilter_Role_Id);

            int intfilter_Role_VenueId;
            result = int.TryParse(filter_Role_VenueId, out intfilter_Role_VenueId);

            int intfilter_Venue_CountryId;
            result = int.TryParse(filter_Venue_CountryId, out intfilter_Venue_CountryId);

            DateTime datetimefilter_From_Artist_Date;
            result = DateTime.TryParse(filter_From_Artist_Date, out datetimefilter_From_Artist_Date);
            if (datetimefilter_From_Artist_Date == null)
            {
                datetimefilter_From_Artist_Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }

            DateTime datetimefilter_To_Artist_Date;
            if (filter_To_Artist_Date != null && filter_To_Artist_Date != "")
            {
                result = DateTime.TryParse(filter_To_Artist_Date, out datetimefilter_To_Artist_Date);
            }
            else
            {
                datetimefilter_To_Artist_Date = new DateTime(2200, 1, 1);
            }


            // data for the Artists
            var artists = db.Artists
                .Where(a => db.PerformanceArtists
                .Where(pa => pa.PerformanceArtist_RoleId == intfilter_Role_Id)
                .Where(pa => strfilter_search == null || strfilter_search == "" || pa.Artist.Artist_FirstName.ToUpper().Contains(strfilter_search.ToUpper()) || pa.Artist.Artist_LastName.ToUpper().Contains(strfilter_search.ToUpper()))
                .Where(pa => intfilter_Role_VenueId == 0 || pa.Performance.Event.Event_VenueId == intfilter_Role_VenueId)
                .Where(pa => intfilter_Venue_CountryId == 0 || pa.Performance.Event.Venue.Venue_CountryId == intfilter_Venue_CountryId)
                .Where(pa => pa.Performance.Event.Event_Date >= datetimefilter_From_Artist_Date)
                .Where(pa => pa.Performance.Event.Event_Date <= datetimefilter_To_Artist_Date)
                .Select(pa => pa.PerformanceArtist_ArtistId).Contains(a.Artist_Id))
            .OrderBy(a => a.Artist_LastName).ThenBy(a => a.Artist_FirstName)
            .Skip(page * 60)
            .Take(60);



            ViewBag.filter_search = strfilter_search;
            ViewBag.filter_Role_VenueId = filter_Role_VenueId;
            ViewBag.filter_Venue_CountryId = filter_Venue_CountryId;
            ViewBag.filter_From_Artist_Date = filter_From_Artist_Date;
            ViewBag.filter_to_Artist_Date = filter_To_Artist_Date;

            ViewBag.recordCount = db.Artists
                .Where(a => db.PerformanceArtists
                .Where(pa => pa.PerformanceArtist_RoleId == intfilter_Role_Id)
                .Where(pa => strfilter_search == null || strfilter_search == "" || pa.Artist.Artist_FirstName.ToUpper().Contains(strfilter_search.ToUpper()) || pa.Artist.Artist_LastName.ToUpper().Contains(strfilter_search.ToUpper()))
                .Where(pa => intfilter_Role_VenueId == 0 || pa.Performance.Event.Event_VenueId == intfilter_Role_VenueId)
                .Where(pa => intfilter_Venue_CountryId == 0 || pa.Performance.Event.Venue.Venue_CountryId == intfilter_Venue_CountryId)
                .Where(pa => pa.Performance.Event.Event_Date >= datetimefilter_From_Artist_Date)
                .Where(pa => pa.Performance.Event.Event_Date <= datetimefilter_To_Artist_Date)
                .Select(pa => pa.PerformanceArtist_ArtistId).Contains(a.Artist_Id))
                .Count();

            return PartialView("_MoreRoleArtists", artists);
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