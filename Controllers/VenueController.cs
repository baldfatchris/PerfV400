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
    public class VenueController : Controller
    {

        public const int PageSize = 20; 
        
        private PerfV100Entities db = new PerfV100Entities();

        //
        // GET: /Venue/

        public ActionResult Index()
        {
            // sort out the paging
            ViewBag.page = 0;
            ViewBag.PageSize = PageSize;

            // data for the Country dropdown
            IEnumerable<SelectListItem> countries = db.CountryRegions
            .Where(c => db.Venues.Select(a => a.Venue_CountryId).Contains(c.CountryRegion_Id))
            .Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert((double)c.CountryRegion_Id),
                Text = c.CountryRegion_Name
            });
            ViewBag.Countries = countries;

            // data for the Venues            
            var venues = db.Venues
                            .OrderByDescending(v => v.Events.Count)
                            .Take(PageSize+1);

            return View(venues.ToList());
        }


        public ActionResult MoreVenues(
            string filter_search,
            string filter_Venue_CountryId,
            int page
            )
        {

            // sort out the paging
            ViewBag.page = page;
            ViewBag.PageSize = PageSize;

            // set up the filters
            bool result;

            string strfilter_search = HttpUtility.UrlDecode((string)filter_search);

            int intfilter_Venue_CountryId;
            result = int.TryParse(filter_Venue_CountryId, out intfilter_Venue_CountryId);

            // data for the venues
            var venues = db.Venues
                .OrderByDescending(v => v.Events.Count)
                .Where(c => filter_search == null || filter_search == "" || c.Venue_Name.ToUpper().Contains(filter_search.ToUpper()))
                .Where(c => intfilter_Venue_CountryId == 0 || c.Venue_CountryId == intfilter_Venue_CountryId)
                .Skip(page * 60)
                .Take(PageSize+1);



            ViewBag.filter_search = strfilter_search;
            ViewBag.filter_Venue_CountryId = filter_Venue_CountryId;


            return PartialView("_MoreVenues", venues);

        }





        public FileContentResult GetVenueImage(int id)
        {
            Venue XVenue = db.Venues.FirstOrDefault(e => e.Venue_Id == id);
            if (XVenue != null && XVenue.Venue_Photo != null)
            {

                if (XVenue.Venue_PhotoMimeType != null)
                {
                    return File(XVenue.Venue_Photo, XVenue.Venue_PhotoMimeType);
                }
                else
                {
                    return File(XVenue.Venue_Photo, "JPEG");
                }

            }
            else
            {
                return null;
            }
        }




        //
        // GET: /Venue/Details/5

        public ActionResult Details(int id = 0)
        {
            Venue venue = db.Venues.Single(v => v.Venue_Id == id);

            // sort out the return url
            ViewBag.ReturnUrl = Url.Action(string.Format("Details/{0}", id), "Venue");

            // sort out the paging
            ViewBag.page = 0;
            ViewBag.PageSize = PageSize;
            
            // data for the Genre dropdown
            IEnumerable<SelectListItem> genres = db.Genres
            .Where(g => db.PerformanceArtists.Where(pa => pa.PerformanceArtist_ArtistId == id).Select(pa => pa.Performance.Event.Event_GenreId).Contains(g.Genre_Id))
            .OrderBy(g => g.Genre_Name)
            .Select(g => new SelectListItem
            {
                Value = SqlFunctions.StringConvert((double)g.Genre_Id),
                Text = g.Genre_Name
            })
            ;
            ViewBag.Genres = genres;

            // data for the Piece dropdown
            IEnumerable<SelectListItem> pieces = db.Pieces
            .Where(p => db.PerformanceArtists.Where(pa => pa.PerformanceArtist_ArtistId == id).Select(pa => pa.Performance.Performance_PieceId).Contains(p.Piece_Id))
            .OrderBy(p => p.Piece_Name)
            .Select(p => new SelectListItem
            {
                Value = SqlFunctions.StringConvert((double)p.Piece_Id),
                Text = p.Piece_Name
            })
            ;
            ViewBag.Pieces = pieces;


            // set up the filters
            DateTime datetimefilter_from_Event_Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            ViewData["filter_from_Event_Date"] = DateTime.Now.ToLongDateString();

            // data for the events
            ViewBag.events = db.Events
            .Where(e => e.Event_VenueId == id)
            .Where(c => c.Event_Date >= datetimefilter_from_Event_Date)
            .OrderBy(c => c.Event_Date)
            .Take(PageSize+1);

            
            return View(venue);
        }


        //
        // GET: /Venue/MoreVenueEvents/5


        public ActionResult MoreVenueEvents(
            string filter_Venue_Id,
            string filter_search,
            string filter_Event_GenreId,
            string filter_Performance_Piece,
            string filter_From_Event_Date,
            string filter_To_Event_Date,
            int page
            )
        {

            ViewBag.Venue_Id = filter_Venue_Id;

            // sort out the paging
            ViewBag.page = page;
            ViewBag.PageSize = PageSize;

            // set up the filters
            bool result;

            string strfilter_search = HttpUtility.UrlDecode((string)filter_search);

            int intfilter_Venue_Id;
            result = int.TryParse(filter_Venue_Id, out intfilter_Venue_Id);

            int intfilter_Event_GenreId;
            result = int.TryParse(filter_Event_GenreId, out intfilter_Event_GenreId);

            DateTime datetimefilter_From_Event_Date;
            result = DateTime.TryParse(filter_From_Event_Date, out datetimefilter_From_Event_Date);
            if (datetimefilter_From_Event_Date == null)
            {
                datetimefilter_From_Event_Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }

            DateTime datetimefilter_To_Event_Date;
            if (filter_To_Event_Date != null && filter_To_Event_Date != "")
            {
                result = DateTime.TryParse(filter_To_Event_Date, out datetimefilter_To_Event_Date);
            }
            else
            {
                datetimefilter_To_Event_Date = new DateTime(2200, 1, 1);
            }


            // data for the events
            var events = db.Events
                .OrderBy(e => e.Event_Date)
                .Where(e => e.Event_VenueId == intfilter_Venue_Id)
                .Where(e => strfilter_search == null || strfilter_search == "" || e.Event_Name.ToUpper().Contains(strfilter_search.ToUpper()) || e.Event_Description.ToUpper().Contains(strfilter_search.ToUpper()))
                .Where(e => e.Event_Date >= datetimefilter_From_Event_Date)
                .Where(e => e.Event_Date <= datetimefilter_To_Event_Date)
                .Where(e => intfilter_Event_GenreId == 0 || e.Event_GenreId == intfilter_Event_GenreId)
                .Where(e => filter_Performance_Piece == null || filter_Performance_Piece == "" || e.Performances.Select(p => p.Piece.Piece_Name).Contains(filter_Performance_Piece))
                .Skip(page * 60)
                .Take(PageSize+1);



            ViewBag.filter_search = strfilter_search;
            ViewBag.filter_Event_GenreId = filter_Event_GenreId;
            ViewBag.filter_Performance_Piece = filter_Performance_Piece;
            ViewBag.filter_from_Event_Date = filter_From_Event_Date;
            ViewBag.filter_to_Event_Date = filter_To_Event_Date;

            return PartialView("_MoreVenueEvents", events);
        }





        //
        // GET: /Venue/Create

        public ActionResult Create()
        {
            ViewBag.Venue_CountryId = new SelectList(db.CountryRegions.OrderBy(c => c.CountryRegion_Name), "CountryRegion_Id", "CountryRegion_Code");
            ViewBag.Venue_StateProvinceId = new SelectList(db.StateProvinces.OrderBy(s => s.StateProvince_Name), "StateProvince_Id", "StateProvince_Code");
            return View();
        }

        //
        // POST: /Venue/Create

        [HttpPost]
        public ActionResult Create(Venue venue)
        {
            if (ModelState.IsValid)
            {
                db.Venues.Add(venue);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Venue_CountryId = new SelectList(db.CountryRegions.OrderBy(c => c.CountryRegion_Name), "CountryRegion_Id", "CountryRegion_Code", venue.Venue_CountryId);
            ViewBag.Venue_StateProvinceId = new SelectList(db.StateProvinces.OrderBy(s => s.StateProvince_Name), "StateProvince_Id", "StateProvince_Code", venue.Venue_StateProvinceId);
            return View(venue);
        }

        //
        // GET: /Venue/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Venue venue = db.Venues.Find(id);
            if (venue == null)
            {
                return HttpNotFound();
            }
            ViewBag.Venue_CountryId = new SelectList(db.CountryRegions.OrderBy(c => c.CountryRegion_Name), "CountryRegion_Id", "CountryRegion_Name", venue.Venue_CountryId);
            ViewBag.Venue_StateProvinceId = new SelectList(db.StateProvinces.OrderBy(s => s.StateProvince_Name), "StateProvince_Id", "StateProvince_Code", venue.Venue_StateProvinceId);
            return View(venue);
        }

        //
        // POST: /Venue/Edit/5

        [HttpPost]
        public ActionResult Edit(Venue venue)
        {
            if (ModelState.IsValid)
            {
                db.Entry(venue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Venue_CountryId = new SelectList(db.CountryRegions.OrderBy(c => c.CountryRegion_Name), "CountryRegion_Id", "CountryRegion_Code", venue.Venue_CountryId);
            ViewBag.Venue_StateProvinceId = new SelectList(db.StateProvinces.OrderBy(s => s.StateProvince_Name), "StateProvince_Id", "StateProvince_Code", venue.Venue_StateProvinceId);
            return View(venue);
        }



        //
        // GET: /Venue/EditVenue/5
        [Authorize]
        public ActionResult EditVenue(int id)
        {
            Venue venue = db.Venues.Single(e => e.Venue_Id == id);
            ViewBag.Venue_CountryId = new SelectList(db.CountryRegions.OrderBy(c => c.CountryRegion_Name), "CountryRegion_Id", "CountryRegion_Name", venue.Venue_CountryId);
            ViewBag.Venue_StateProvinceId = new SelectList(db.StateProvinces.OrderBy(s => s.StateProvince_Name), "StateProvince_Id", "StateProvince_Name", venue.Venue_StateProvinceId);
            return PartialView(venue);
        }

        //
        // POST: /Venue/EditVenue/5

        [HttpPost]
        [Authorize]
        public ActionResult EditVenue(Venue venue)
        {
            if (ModelState.IsValid)
            {
                db.Venues.Attach(venue);
                db.Entry(venue).State = EntityState.Modified;
                db.SaveChanges();

                
                return PartialView("DetailsVenue", venue);
            }
            else
            {
                return Content("Please review your form");
            }
        }





        public class WrappedJsonResult : JsonResult
        {

            public override void ExecuteResult(ControllerContext context)
            {
                context.HttpContext.Response.Write("<html><body><textarea id=\"jsonResult\" name=\"jsonResult\">");
                base.ExecuteResult(context);
                context.HttpContext.Response.Write("</textarea></body></html>");
                context.HttpContext.Response.ContentType = "text/html";
            }

        }

        [HttpPost]
        [Authorize]
        public WrappedJsonResult EditVenuePhoto(HttpPostedFileWrapper imageFile, int Venue_Id)
        {
            if (imageFile == null || imageFile.ContentLength == 0)
            {
                return new WrappedJsonResult
                {
                    Data = new
                    {
                        IsValid = false,
                        Message = "No file was uploaded.",
                        ImagePath = string.Empty
                    }
                };
            }



            string mimeType = imageFile.ContentType;
            Stream fileStream = imageFile.InputStream;
            string fileName = Path.GetFileName(imageFile.FileName);
            int fileLength = imageFile.ContentLength;
            byte[] fileData = new byte[fileLength];
            fileStream.Read(fileData, 0, fileLength);

            Venue venue = db.Venues.FirstOrDefault(v => v.Venue_Id == Venue_Id);
            venue.Venue_Photo = fileData;
            venue.Venue_PhotoMimeType = mimeType;
            venue.Venue_PhotoFileName = fileName;

            //db.Venues.Attach(venue);
            db.Entry(venue).State = EntityState.Modified;
            db.SaveChanges();


            var fileNameZ = String.Format("{0}.jpg", Guid.NewGuid().ToString());
            var imagePath = Path.Combine(Server.MapPath(Url.Content("~/Uploads")), fileNameZ);

            imageFile.SaveAs(imagePath);

            return new WrappedJsonResult
            {
                Data = new
                {
                    IsValid = true,
                    Message = string.Empty,
                    ImagePath = Url.Content(String.Format("~/Uploads/{0}", fileNameZ))
                }
            };
        }









        //
        // GET: /Venue/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Venue venue = db.Venues.Find(id);
            if (venue == null)
            {
                return HttpNotFound();
            }
            return View(venue);
        }

        //
        // POST: /Venue/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Venue venue = db.Venues.Find(id);
            db.Venues.Remove(venue);
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