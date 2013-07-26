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
using System.Web.Security;

namespace PerfV400.Controllers
{
    public class ArtistController : Controller
    {

        public const int PageSize = 20;

        private PerfV100Entities db = new PerfV100Entities();

        public FileContentResult GetArtistImage(int id)
        {
            Artist artist = db.Artists.FirstOrDefault(e => e.Artist_Id == id);
            if (artist != null && artist.Artist_Photo != null)
            {
                if (artist.Artist_PhotoMimeType != null)
                {
                    return File(artist.Artist_Photo, artist.Artist_PhotoMimeType);
                }
                else 
                {
                    return File(artist.Artist_Photo, "JPEG");
                }
            }
            else
            {
                return null;
            }
        }


        //
        // GET: /Artist/

        public ActionResult Index()
        {
            // sort out the paging
            ViewBag.page = 0;
            ViewBag.PageSize = PageSize;

            // data for the Type dropdown
            IEnumerable<SelectListItem> types = db.Types
            .Where(t => db.Artists.Select(a => a.Artist_TypeId).Contains(t.Type_Id))
            .Select(t => new SelectListItem
            {
                Value = SqlFunctions.StringConvert((double)t.Type_Id),
                Text = t.Type_Name
            });
            ViewBag.Types = types;


            // data for the Country dropdown
            IEnumerable<SelectListItem> countries = db.CountryRegions
            .Where(c => db.Artists.Select(a => a.Artist_CountryId).Contains(c.CountryRegion_Id))
            .Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert((double)c.CountryRegion_Id),
                Text = c.CountryRegion_Name
            });
            ViewBag.Countries = countries;

            // data for the artists
            var artists = db.Artists
                .OrderByDescending(c => c.PerformanceArtists.Count)
                .Take(PageSize + 1);

            return View(artists.ToList());
        }

        public ActionResult MoreArtists(
             string filter_search,
             string filter_Artist_TypeId,
             string filter_Artist_CountryId,
             int page
             )
        {

            // sort out the paging
            ViewBag.page = page;
            ViewBag.PageSize = PageSize;

            // set up the filters
            bool result;

            string strfilter_search = HttpUtility.UrlDecode((string)filter_search);

            int intfilter_Artist_TypeId;
            result = int.TryParse(filter_Artist_TypeId, out intfilter_Artist_TypeId);

            int intfilter_Artist_CountryId;
            result = int.TryParse(filter_Artist_CountryId, out intfilter_Artist_CountryId);

            // data for the artists
            var artists = db.Artists
                .OrderByDescending(c => c.PerformanceArtists.Count)
                .Where(c => filter_search == null || filter_search == "" || c.Artist_FirstName.ToUpper().Contains(filter_search.ToUpper()) || c.Artist_LastName.ToUpper().Contains(filter_search.ToUpper()))
                .Where(c => intfilter_Artist_TypeId == 0 || c.Artist_TypeId == intfilter_Artist_TypeId)
                .Where(c => intfilter_Artist_CountryId == 0 || c.Artist_CountryId == intfilter_Artist_CountryId)
                .Skip(page * PageSize)
                .Take(PageSize+1);


            ViewBag.filter_search = strfilter_search;
            ViewBag.filter_Artist_TypeId = filter_Artist_TypeId;
            ViewBag.filter_Artist_CountryId = filter_Artist_CountryId;

            return PartialView("_MoreArtists", artists);

        }


        //
        // GET: /Artist/Details/5

        public ActionResult Details(int id)
        {

            Artist artist = db.Artists.Find(id);
            if (artist == null)
            {
                return HttpNotFound();
            }

            // sort out the return url
            ViewBag.ReturnUrl = Url.Action(string.Format("Details/{0}", id), "Artist");

            // Am I the artist?
            ViewBag.AmITheArtist = false;
            if (Request.IsAuthenticated && artist.Artist_UserId == (int)Session["UserID"])
            {
                ViewBag.AmITheArtist = true;
            }

            // Could I be the artist?
            ViewBag.CouldIBeTheArtist = false;
            if (Request.IsAuthenticated && artist.Artist_UserId == null)
            {
                ViewBag.CouldIBeTheArtist = true;
            }



            ViewBag.Type = db.Types.FirstOrDefault(t => t.Type_Id == artist.Artist_TypeId);
            ViewBag.Agent = db.Agents.FirstOrDefault(a => a.Agent_Id == artist.Artist_AgentId);
            ViewBag.CountryRegion = db.CountryRegions.FirstOrDefault(c => c.CountryRegion_Id == artist.Artist_CountryId);
            ViewBag.StateProvince = db.StateProvinces.FirstOrDefault(c => c.StateProvince_Id == artist.Artist_StateProvinceId);


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

            // data for the Venue dropdown
            IEnumerable<SelectListItem> venues = db.Venues
            .Where(v => db.PerformanceArtists.Where(pa => pa.PerformanceArtist_ArtistId == id).Select(pa => pa.Performance.Event.Event_VenueId).Contains(v.Venue_Id))
            .OrderBy(v => v.Venue_Name)
            .Select(v => new SelectListItem
            {
                Value = SqlFunctions.StringConvert((double)v.Venue_Id),
                Text = v.Venue_Name
            });
            ViewBag.Venues = venues;

            // data for the City dropdown
            IEnumerable<SelectListItem> cities = db.Venues
            .Where(v => db.PerformanceArtists.Where(pa => pa.PerformanceArtist_ArtistId == id).Select(pa => pa.Performance.Event.Event_VenueId).Contains(v.Venue_Id))
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
            .Where(c => db.PerformanceArtists.Where(pa => pa.PerformanceArtist_ArtistId == id).Select(pa => pa.Performance.Event.Venue.Venue_CountryId).Contains(c.CountryRegion_Id))
            .OrderBy(c => c.CountryRegion_Name)
            .Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert((double)c.CountryRegion_Id),
                Text = c.CountryRegion_Name
            });
            ViewBag.Countries = countries;


            // set up the filters
            DateTime datetimefilter_from_Event_Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            ViewData["filter_from_Event_Date"] = DateTime.Now.ToLongDateString();

            // data for the events
            ViewBag.events = db.Events
            .Where(e => db.PerformanceArtists.Where(pa => pa.PerformanceArtist_ArtistId == id).Select(pa => pa.Performance.Performance_EventId).Contains(e.Event_Id))
            .Where(c => c.Event_Date >= datetimefilter_from_Event_Date)
            .OrderBy(c => c.Event_Date)
            .Take(PageSize+1);

            return View(artist);
        }

        //
        // GET: /Artist/MoreArtistEvents/5


        public ActionResult MoreArtistEvents(
            string filter_Artist_Id,
            string filter_search,
            string filter_Event_GenreId,
            string filter_Performance_Piece,
            string filter_Event_VenueId,
            string filter_Venue_City,
            string filter_Venue_CountryId,
            string filter_From_Event_Date,
            string filter_To_Event_Date,
            int page
            )
        {

            ViewBag.Artist_Id = filter_Artist_Id;

            // sort out the paging
            ViewBag.page = page;
            ViewBag.PageSize = PageSize;

            // set up the filters
            bool result;

            string strfilter_search = HttpUtility.UrlDecode((string)filter_search);

            int intfilter_Artist_Id;
            result = int.TryParse(filter_Artist_Id, out intfilter_Artist_Id);

            int intfilter_Event_GenreId;
            result = int.TryParse(filter_Event_GenreId, out intfilter_Event_GenreId);

            int intfilter_Event_VenueId;
            result = int.TryParse(filter_Event_VenueId, out intfilter_Event_VenueId);

            int intfilter_Venue_CountryId;
            result = int.TryParse(filter_Venue_CountryId, out intfilter_Venue_CountryId);

            string strfilter_Venue_City = HttpUtility.UrlDecode((string)filter_Venue_City);

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
                .Where(e => db.PerformanceArtists.Where(pa => pa.PerformanceArtist_ArtistId == intfilter_Artist_Id).Select(pa => pa.Performance.Performance_EventId).Contains(e.Event_Id))
                .Where(e => strfilter_search == null || strfilter_search == "" || e.Event_Name.ToUpper().Contains(strfilter_search.ToUpper()) || e.Event_Description.ToUpper().Contains(strfilter_search.ToUpper()))
                .Where(e => e.Event_Date >= datetimefilter_From_Event_Date)
                .Where(e => e.Event_Date <= datetimefilter_To_Event_Date)
                .Where(e => intfilter_Event_GenreId == 0 || e.Event_GenreId == intfilter_Event_GenreId)
                .Where(e => filter_Performance_Piece == null || filter_Performance_Piece == "" || e.Performances.Select(p => p.Piece.Piece_Name).Contains(filter_Performance_Piece))
                .Where(e => intfilter_Event_VenueId == 0 || e.Event_VenueId == intfilter_Event_VenueId)
                .Where(e => filter_Venue_City == null || strfilter_Venue_City == "" || e.Venue.Venue_City == strfilter_Venue_City)
                .Where(e => intfilter_Venue_CountryId == 0 || e.Venue.Venue_CountryId == intfilter_Venue_CountryId)
                .Skip(page * PageSize)
                .Take(PageSize+1);



            ViewBag.filter_search = strfilter_search;
            ViewBag.filter_Event_GenreId = filter_Event_GenreId;
            ViewBag.filter_Performance_Piece = filter_Performance_Piece;
            ViewBag.filter_Event_VenueId = filter_Event_VenueId;
            ViewBag.filter_Venue_City = strfilter_Venue_City;
            ViewBag.filter_Venue_CountryId = filter_Venue_CountryId;
            ViewBag.filter_from_Event_Date = filter_From_Event_Date;
            ViewBag.filter_to_Event_Date = filter_To_Event_Date;

            return PartialView("_MoreArtistEvents", events);
        }




        //
        // GET: /Artist/Create

        public ActionResult Create()
        {
            ViewBag.Artist_AgentId = new SelectList(db.Agents.OrderBy(a => a.Agent_Name), "Agent_Id", "Agent_Name");
            ViewBag.Artist_CountryId = new SelectList(db.CountryRegions.OrderBy(c => c.CountryRegion_Name), "CountryRegion_Id", "CountryRegion_Code");
            ViewBag.Artist_StateProvinceId = new SelectList(db.StateProvinces.OrderBy(s => s.StateProvince_Name), "StateProvince_Id", "StateProvince_Code");
            ViewBag.Artist_TypeId = new SelectList(db.Types.OrderBy(t => t.Type_Name), "Type_Id", "Type_Name");
            return View();
        }

        //
        // POST: /Artist/Create

        [HttpPost]
        public ActionResult Create(Artist artist)
        {
            if (ModelState.IsValid)
            {
                db.Artists.Add(artist);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Artist_AgentId = new SelectList(db.Agents.OrderBy(a => a.Agent_Name), "Agent_Id", "Agent_Name", artist.Artist_AgentId);
            ViewBag.Artist_CountryId = new SelectList(db.CountryRegions.OrderBy(c => c.CountryRegion_Name), "CountryRegion_Id", "CountryRegion_Name", artist.Artist_CountryId);
            ViewBag.Artist_StateProvinceId = new SelectList(db.StateProvinces.OrderBy(s => s.StateProvince_Name), "StateProvince_Id", "StateProvince_Name", artist.Artist_StateProvinceId);
            ViewBag.Artist_TypeId = new SelectList(db.Types.OrderBy(t => t.Type_Name), "Type_Id", "Type_Name", artist.Artist_TypeId);
            return View(artist);
        }

        //
        // GET: /Artist/EditArtist/5

        public ActionResult EditArtist(int id = 0)
        {
            Artist artist = db.Artists.Find(id);
            if (artist == null)
            {
                return HttpNotFound();
            }
            ViewBag.Artist_AgentId = new SelectList(db.Agents.OrderBy(a => a.Agent_Name), "Agent_Id", "Agent_Name", artist.Artist_AgentId);
            ViewBag.Artist_CountryId = new SelectList(db.CountryRegions.OrderBy(c => c.CountryRegion_Name), "CountryRegion_Id", "CountryRegion_Name", artist.Artist_CountryId);
            ViewBag.Artist_StateProvinceId = new SelectList(db.StateProvinces.OrderBy(s => s.StateProvince_Name), "StateProvince_Id", "StateProvince_Name", artist.Artist_StateProvinceId);
            ViewBag.Artist_TypeId = new SelectList(db.Types.OrderBy(t => t.Type_Name), "Type_Id", "Type_Name", artist.Artist_TypeId);
            return PartialView(artist);
        }

        //
        // POST: /Artist/EditArtist/5

        [HttpPost]
        public ActionResult EditArtist(Artist artist)
        {
            if (ModelState.IsValid)
            {
                db.Entry(artist).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.Type = db.Types.FirstOrDefault(t => t.Type_Id == artist.Artist_TypeId);
                ViewBag.Agent = db.Agents.FirstOrDefault(a => a.Agent_Id == artist.Artist_AgentId);
                ViewBag.CountryRegion = db.CountryRegions.FirstOrDefault(c => c.CountryRegion_Id == artist.Artist_CountryId);
                ViewBag.StateProvince = db.StateProvinces.FirstOrDefault(c => c.StateProvince_Id == artist.Artist_StateProvinceId);

                // Am I the artist?
                ViewBag.AmITheArtist = false;
                if (Request.IsAuthenticated && artist.Artist_UserId == (int)Session["UserID"])
                {
                    ViewBag.AmITheArtist = true;
                }

                // Could I be the artist?
                ViewBag.CouldIBeTheArtist = false;
                if (Request.IsAuthenticated && artist.Artist_UserId == null)
                {
                    ViewBag.CouldIBeTheArtist = true;
                }


                return PartialView("DetailsArtist", artist);
            }
            else
            {
                return Content("Please review your form");
            }
        }


        //
        // GET: /Artist/ThisIsMe/5

        public ActionResult ThisIsMe(int id = 0)
        {
            Artist artist = db.Artists.Find(id);
            if (artist == null)
            {
                return HttpNotFound();
            }

            // Retrieve the User Id
            if (Session["UserID"] != null)
            {
                ViewBag.UserId = Session["UserID"];
            }

            ViewBag.Artist_AgentId = new SelectList(db.Agents.OrderBy(a => a.Agent_Name), "Agent_Id", "Agent_Name", artist.Artist_AgentId);
            ViewBag.Artist_CountryId = new SelectList(db.CountryRegions.OrderBy(c => c.CountryRegion_Name), "CountryRegion_Id", "CountryRegion_Name", artist.Artist_CountryId);
            ViewBag.Artist_StateProvinceId = new SelectList(db.StateProvinces.OrderBy(s => s.StateProvince_Name), "StateProvince_Id", "StateProvince_Name", artist.Artist_StateProvinceId);
            ViewBag.Artist_TypeId = new SelectList(db.Types.OrderBy(t => t.Type_Name), "Type_Id", "Type_Name", artist.Artist_TypeId);
            return PartialView(artist);
        }

        //
        // POST: /Artist/ThisIsMe/5

        [HttpPost]
        public ActionResult ThisIsMe(Artist artist)
        {
            if (ModelState.IsValid)
            {
                db.Entry(artist).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.Type = db.Types.FirstOrDefault(t => t.Type_Id == artist.Artist_TypeId);
                ViewBag.Agent = db.Agents.FirstOrDefault(a => a.Agent_Id == artist.Artist_AgentId);
                ViewBag.CountryRegion = db.CountryRegions.FirstOrDefault(c => c.CountryRegion_Id == artist.Artist_CountryId);
                ViewBag.StateProvince = db.StateProvinces.FirstOrDefault(c => c.StateProvince_Id == artist.Artist_StateProvinceId);

                ViewBag.AmITheArtist = true;

                return PartialView("DetailsArtist", artist);
            }
            else
            {
                return Content("Please review your form");
            }
        }













        //
        // GET: /Artist/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Artist artist = db.Artists.Find(id);
            if (artist == null)
            {
                return HttpNotFound();
            }
            return View(artist);
        }

        //
        // POST: /Artist/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Artist artist = db.Artists.Find(id);
            db.Artists.Remove(artist);
            db.SaveChanges();
            return RedirectToAction("Index");
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
        public WrappedJsonResult EditArtistPhoto(HttpPostedFileWrapper imageFile, int Artist_Id)
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

            Artist XArtist = db.Artists.FirstOrDefault(e => e.Artist_Id == Artist_Id);
            XArtist.Artist_Photo = fileData;
            XArtist.Artist_PhotoMimeType = mimeType;
            XArtist.Artist_PhotoFileName = fileName;

            //db.Artists.Attach(XArtist);
            db.Entry(XArtist).State = EntityState.Modified;
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

        
        
        
        
        
        
        
        
        
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
}