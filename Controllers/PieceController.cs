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
    public class PieceController : Controller
    {
        public const int PageSize = 20;

        private PerfV100Entities db = new PerfV100Entities();

        //
        // GET: /Piece/

        public ActionResult Index()
        {
            // sort out the return url
            ViewBag.ReturnUrl = Url.Action("Index", "Piece");

            // sort out the paging
            ViewBag.page = 0;
            ViewBag.PageSize = PageSize;

            // data for the Genre dropdown
            IEnumerable<SelectListItem> genres = db.Genres
            .Where(g => db.Events.Select(e => e.Event_GenreId).Contains(g.Genre_Id))
            .OrderBy(g => g.Genre_Name)
            .Select(g => new SelectListItem
            {
                Value = SqlFunctions.StringConvert((double)g.Genre_Id),
                Text = g.Genre_Name
            })
            ;
            ViewBag.Genres = genres;


            // data for the Pieces            
            var Pieces = db.Pieces
                .OrderByDescending(p => p.Performances.Count())
                .ThenBy(p => p.Piece_Name)
                .Take(PageSize+1);

            return View(Pieces.ToList());
        }


        public ActionResult MorePieces(
            string filter_search,
            string filter_Piece_GenreId,
            string filter_Artist_FullName,
            int page
            )
        {

            // sort out the paging
            ViewBag.page = page;
            ViewBag.PageSize = PageSize;


            // set up the filters
            bool result;

            string strfilter_search = HttpUtility.UrlDecode((string)filter_search);

            int intfilter_Piece_GenreId;
            result = int.TryParse(filter_Piece_GenreId, out intfilter_Piece_GenreId);



            // data for the Pieces
            var Pieces = db.Pieces
                .OrderByDescending(p => p.Performances.Count())
                .ThenBy(p => p.Piece_Name)
                .Where(c => filter_search == null || filter_search == "" || c.Piece_Name.ToUpper().Contains(filter_search.ToUpper()))
                .Where(c => intfilter_Piece_GenreId == 0 || c.Piece_GenreId == intfilter_Piece_GenreId)

                .Where(p => filter_Artist_FullName == null 
                        || filter_Artist_FullName == "" 
                        || p.PieceArtists.Select(pa => string.Concat(pa.Artist.Artist_FirstName.ToUpper(), " ", pa.Artist.Artist_LastName.ToUpper())).Contains(filter_Artist_FullName.ToUpper())
                        )


                .Skip(page * PageSize)
                .Take(PageSize+1);


            ViewBag.filter_search = strfilter_search;
            ViewBag.filter_Piece_GenreId = filter_Piece_GenreId;
            ViewBag.filter_Artist_FullName = filter_Artist_FullName;

            return PartialView("_MorePieces", Pieces);

        }


        public FileContentResult GetPieceImage(int id)
        {
            Piece piece = db.Pieces.FirstOrDefault(e => e.Piece_Id == id);
            if (piece != null && piece.Piece_Photo != null)
            {

                if (piece.Piece_PhotoMimeType != null)
                {
                    return File(piece.Piece_Photo, piece.Piece_PhotoMimeType);
                }
                else
                {
                    return File(piece.Piece_Photo, "JPEG");
                }

            }
            else
            {

                Performance performance = db.Performances
                    .Where(p => p.Performance_PieceId == id)
                    .FirstOrDefault(p => p.Performance_Photo != null)
                    ;
                if (performance != null && performance.Performance_Photo != null)
                {

                    if (performance.Performance_PhotoMimeType != null)
                    {
                        return File(performance.Performance_Photo, performance.Performance_PhotoMimeType);
                    }
                    else
                    {
                        return File(performance.Performance_Photo, "JPEG");
                    }

                }
                else
                {




                    Event xEvent = db.Events
                        .Where(e => db.Performances.Where(p => p.Performance_PieceId == id).Select(p => p.Event.Event_Id).Contains(e.Event_Id))


                        .FirstOrDefault(e => e.Event_Photo != null)

                        ;
                    if (xEvent != null && xEvent.Event_Photo != null)
                    {

                        if (xEvent.Event_PhotoMimeType != null)
                        {
                            return File(xEvent.Event_Photo, xEvent.Event_PhotoMimeType);
                        }
                        else
                        {
                            return File(xEvent.Event_Photo, "JPEG");
                        }

                    }
                    else
                    {
                        return null;
                    }
                
                
                
                
                
                
                }
            
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
        public WrappedJsonResult EditPiecePhoto(HttpPostedFileWrapper imageFile, int Piece_Id)
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

            Piece piece = db.Pieces.FirstOrDefault(e => e.Piece_Id == Piece_Id);
            piece.Piece_Photo = fileData;
            piece.Piece_PhotoMimeType = mimeType;
            piece.Piece_PhotoFileName = fileName;

            //db.Pieces.Attach(piece);
            db.Entry(piece).State = EntityState.Modified;
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











        public FileContentResult GetPerformanceImage(int id)
        {
            if (id == 0)
            {
                return null;
            }
            else
            {
                Performance performance = db.Performances.Find(id);
                if (performance != null)
                {
                    if (performance.Performance_Photo != null)
                    {
                        return File(performance.Performance_Photo, performance.Performance_PhotoMimeType);
                    }
                    else if (performance.Event.Event_Photo != null)
                    {
                        if (performance.Event.Event_PhotoMimeType != null)
                        {
                            return File(performance.Event.Event_Photo, performance.Event.Event_PhotoMimeType);
                        }
                        else
                        {
                            return File(performance.Event.Event_Photo, "JPEG");
                        }
                    }
                    else if (performance.Event.Venue.Venue_Photo != null)
                    {
                        return File(performance.Event.Venue.Venue_Photo, performance.Event.Venue.Venue_PhotoMimeType);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }


        //
        // GET: /Piece/Details/5

        public ActionResult Details(int id = 0)
        {
            Piece Piece = db.Pieces.Single(v => v.Piece_Id == id);

            // sort out the return url
            ViewBag.ReturnUrl = Url.Action(string.Format("Details/{0}", id), "Piece");

            // sort out the paging
            ViewBag.page = 0;
            ViewBag.PageSize = PageSize;

            // data for the Venue dropdown
            IEnumerable<SelectListItem> Venues = db.Venues
            .Where(v => db.Performances.Where(p => p.Performance_PieceId == id).Select(p => p.Event.Event_VenueId).Contains(v.Venue_Id))
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
            .Where(v => db.Performances.Where(p => p.Performance_PieceId == id).Select(p => p.Event.Event_VenueId).Contains(v.Venue_Id))
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
            .Where(c => db.Performances.Where(p => p.Performance_PieceId == id).Select(p => p.Event.Venue.Venue_CountryId).Contains(c.CountryRegion_Id))
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


            // data for the pieceArtists
            ViewBag.pieceArtists = db.PieceArtists
            .Where(p => p.PieceArtist_PieceId == id)
            .OrderBy(p => p.PieceArtist_Id)
            .Take(PageSize+1);

            // data for the roles
            ViewBag.roles = db.Roles
            .Where(p => p.Role_PieceId == id)
            .OrderBy(p => p.Role_Rank)
            .Take(PageSize+1);

            // data for the performances
            ViewBag.performances = db.Performances
            .Where(p => p.Performance_PieceId == id)
            .Where(p => p.Event.Event_Date >= datetimefilter_from_Event_Date)
            .OrderBy(p => p.Event.Event_Date)
            .Take(PageSize+1);

            
            return View(Piece);
        }


        public ActionResult MorePieceArtists(
            string filter_Piece_Id,
            int page
            )
        {

            ViewBag.Piece_Id = filter_Piece_Id;

            // sort out the paging
            ViewBag.page = page;
            ViewBag.PageSize = PageSize;

            // set up the filters
            bool result;

            int intfilter_Piece_Id;
            result = int.TryParse(filter_Piece_Id, out intfilter_Piece_Id);

            // data for the pieceArtists
            var pieceArtists = db.PieceArtists
                .Where(p => p.PieceArtist_PieceId == intfilter_Piece_Id)
                .OrderBy(p => p.PieceArtist_Id)
                .Skip(page * 1)
                .Take(PageSize+1);

            return PartialView("_MorePieceArtists", pieceArtists);
        }



        public ActionResult MoreRoles(
            string filter_Piece_Id,
            int page
            )
        {

            ViewBag.Piece_Id = filter_Piece_Id;

            // sort out the paging
            ViewBag.page = page;
            ViewBag.PageSize = PageSize;

            // set up the filters
            bool result;

            int intfilter_Piece_Id;
            result = int.TryParse(filter_Piece_Id, out intfilter_Piece_Id);

            // data for the Roles
            var Roles = db.Roles
                .Where(p => p.Role_PieceId == intfilter_Piece_Id)
                .OrderBy(p => p.Role_Rank)
                .Skip(page * 1)
                .Take(PageSize+1);

            
            return PartialView("_MoreRoles", Roles);
        }

        public ActionResult MorePiecePerformances(
            string filter_Piece_Id,
            string filter_Event_VenueId,
            string filter_Event_Venue,
            string filter_Venue_City,
            string filter_Venue_CountryId,
            string filter_From_Event_Date,
            string filter_To_Event_Date,
            int page
            )
        {

            ViewBag.Piece_Id = filter_Piece_Id;

            // sort out the paging
            ViewBag.page = page;
            ViewBag.PageSize = PageSize;

            // set up the filters
            bool result;

            int intfilter_Piece_Id;
            result = int.TryParse(filter_Piece_Id, out intfilter_Piece_Id);

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


            // data for the performances
            var performances = db.Performances
                .Where(p => p.Performance_PieceId == intfilter_Piece_Id)
                .Where(p => p.Event.Event_Date >= datetimefilter_From_Event_Date)
                .Where(p => p.Event.Event_Date <= datetimefilter_To_Event_Date)
                .Where(p => intfilter_Event_VenueId == 0 || p.Event.Event_VenueId == intfilter_Event_VenueId)
                .Where(p => filter_Venue_City == null || strfilter_Venue_City == "" || p.Event.Venue.Venue_City == strfilter_Venue_City)
                .Where(p => intfilter_Venue_CountryId == 0 || p.Event.Venue.Venue_CountryId == intfilter_Venue_CountryId)
                .OrderBy(p => p.Event.Event_Date)
                .Skip(page * PageSize)
                .Take(PageSize+1);

            ViewBag.filter_Event_VenueId = filter_Event_VenueId;
            ViewBag.filter_Event_Venue = filter_Event_Venue;
            ViewBag.filter_Venue_City = strfilter_Venue_City;
            ViewBag.filter_Venue_CountryId = filter_Venue_CountryId;
            ViewBag.filter_from_Event_Date = filter_From_Event_Date;
            ViewBag.filter_to_Event_Date = filter_To_Event_Date;


            return PartialView("_MorePiecePerformances", performances);
        }

















        //
        // GET: /Piece/CreatePiece

        public ActionResult CreatePiece()
        {
            ViewBag.Piece_GenreId = new SelectList(db.Genres.OrderBy(g => g.Genre_Name), "Genre_Id", "Genre_Name");
            return View();
        }

        //
        // POST: /Piece/CreatePiece

        [HttpPost]
        public ActionResult CreatePiece(Piece piece)
        {
            if (ModelState.IsValid)
            {
                db.Pieces.Add(piece);
                db.SaveChanges();






                int id = piece.Piece_Id;

                // sort out the paging
                ViewBag.page = 0;
                ViewBag.PageSize = PageSize;

                // data for the Venue dropdown
                IEnumerable<SelectListItem> Venues = db.Venues
                .Where(v => db.Performances.Where(p => p.Performance_PieceId == id).Select(p => p.Event.Event_VenueId).Contains(v.Venue_Id))
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
                .Where(v => db.Performances.Where(p => p.Performance_PieceId == id).Select(p => p.Event.Event_VenueId).Contains(v.Venue_Id))
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
                .Where(c => db.Performances.Where(p => p.Performance_PieceId == id).Select(p => p.Event.Venue.Venue_CountryId).Contains(c.CountryRegion_Id))
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


                // data for the pieceArtists
                ViewBag.pieceArtists = db.PieceArtists
                .Where(p => p.PieceArtist_PieceId == id)
                .Take(PageSize+1);

                // data for the roles
                ViewBag.roles = db.Roles
                .Where(p => p.Role_PieceId == id)
                .Take(PageSize+1);

                // data for the performances
                ViewBag.performances = db.Performances
                .Where(p => p.Performance_PieceId == id)
                .Take(PageSize+1);

                
                return View("Details", piece);
            }
            else
            {
                return Content("Please review your form");
            }

       }



        //
        // GET: /Piece/EditPiece/5
        [Authorize]
        public ActionResult EditPiece(int id)
        {
            Piece piece = db.Pieces.Single(e => e.Piece_Id == id);
            ViewBag.Piece_GenreId = new SelectList(db.Genres.OrderBy(g => g.Genre_Name), "Genre_Id", "Genre_Name", piece.Piece_GenreId);
            return PartialView(piece);
        }

        //
        // POST: /Piece/EditPiece/5

        [HttpPost]
        [Authorize]
        public ActionResult EditPiece(Piece piece)
        {
            if (ModelState.IsValid)
            {
                db.Entry(piece).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.Genre = db.Venues.FirstOrDefault(v => v.Venue_Id == piece.Piece_GenreId);

                return PartialView("DetailsPiece", piece);
            }
            else
            {
                return Content("Please review your form");
            }
        }



        //
        // GET: /Piece/EditPieceArtist/5
        [Authorize]
        public ActionResult EditPieceArtist(int id)
        {
            PieceArtist pieceArtist = db.PieceArtists.Single(e => e.PieceArtist_Id == id);
            ViewBag.PieceArtist_PieceArtistTypeId = new SelectList(db.PieceArtistTypes.OrderBy(p => p.PieceArtistType_Name), "PieceArtistType_Id", "PieceArtistType_Name", pieceArtist.PieceArtist_PieceArtistTypeId);
            return PartialView(pieceArtist);
        }

        //
        // POST: /Piece/EditPieceArtist/5

        [HttpPost]
        [Authorize]
        public ActionResult EditPieceArtist(PieceArtist pieceArtist)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pieceArtist).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.Artist = db.Artists.FirstOrDefault(a => a.Artist_Id == pieceArtist.PieceArtist_ArtistId);
                ViewBag.PieceArtistType = db.PieceArtistTypes.FirstOrDefault(v => v.PieceArtistType_Id == pieceArtist.PieceArtist_PieceArtistTypeId);

                return PartialView("DetailsPieceArtist", pieceArtist);
            }
            else
            {
                return Content("Please review your form");
            }
        }


        //
        // GET: /Piece/EditRole/5
        [Authorize]
        public ActionResult EditRole(int id)
        {
            Role Role = db.Roles.Single(e => e.Role_Id == id);
            ViewBag.Role_TypeId = new SelectList(db.Types.OrderBy(t => t.Type_Name), "Type_Id", "Type_Name", Role.Role_TypeId);
            return PartialView(Role);
        }

        //
        // POST: /Piece/EditRole/5

        [HttpPost]
        [Authorize]
        public ActionResult EditRole(Role Role)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Role).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.Type = db.Types.FirstOrDefault(v => v.Type_Id == Role.Role_TypeId);

                return PartialView("DetailsRole", Role);
            }
            else
            {
                return Content("Please review your form");
            }
        }


        // Piece Artist

        //
        // GET: /Event/NewPieceArtist/5
        [Authorize]
        public ActionResult NewPieceArtist(int PieceId)
        {
            Piece Piece = db.Pieces.Single(p => p.Piece_Id == PieceId);

            ViewBag.PieceId = PieceId;

            ViewBag.PieceArtist_PieceArtistTypeId = new SelectList(
                db.PieceArtistTypes
                    .OrderBy(r => r.PieceArtistType_Id),
                "PieceArtistType_Id",
                "PieceArtistType_Name");

            ViewBag.PieceArtist_ArtistId = new SelectList(db.Artists
                .OrderBy(a => a.Artist_LastName).ThenBy(a => a.Artist_FirstName), "Artist_Id", "Artist_LastName");

            PieceArtist PieceArtist = new PieceArtist();
            PieceArtist.PieceArtist_PieceId = PieceId;

            return PartialView("NewPieceArtist", PieceArtist);
        }

        //
        // POST: /Event/NewPieceArtist/5

        [HttpPost]
        [Authorize]
        public ActionResult NewPieceArtist(PieceArtist PieceArtist, string PieceArtist_ArtistFullName)
        {
            if (ModelState.IsValid)
            {
                Artist artist = db.Artists.FirstOrDefault(a => string.Concat(a.Artist_FirstName, " ", a.Artist_LastName).Equals(PieceArtist_ArtistFullName));
                if (artist != null)
                {
                    //attach the artist
                    PieceArtist.PieceArtist_ArtistId = artist.Artist_Id;
                }
                else
                {
                    //create a new artist

                    // remove any unnecessary spaces
                    PieceArtist_ArtistFullName.Replace("  ", " ").Trim();

                    string firstName = "";
                    string lastName;
                    
                    // find the position of the last space
                    int i = PieceArtist_ArtistFullName.LastIndexOf(" ");

                    if (i > 0)
                    {
                        //there are two words or more
                        firstName = PieceArtist_ArtistFullName.Substring(0, i);
                        lastName = PieceArtist_ArtistFullName.Substring(i, PieceArtist_ArtistFullName.Length - i);
                    }
                    else
                    { //there's only one word
                        lastName = PieceArtist_ArtistFullName;
                    }

                    Artist newArtist = new Artist();
                    newArtist.Artist_FirstName = firstName;
                    newArtist.Artist_LastName = lastName;
                    db.Artists.Add(newArtist);

                    PieceArtist.PieceArtist_ArtistId = newArtist.Artist_Id;

                }
                db.PieceArtists.Add(PieceArtist);
                db.SaveChanges();

                ViewBag.artist = db.Artists.Find(PieceArtist.PieceArtist_ArtistId);
                ViewBag.pieceArtistType = db.PieceArtistTypes.Find(PieceArtist.PieceArtist_PieceArtistTypeId);

                return PartialView("DetailsPieceArtist", PieceArtist);
            }
            else
            {
                return Content("Please review your form");
            }
        }

        //
        // GET: /Event/DeletePieceArtist/5
        [Authorize]
        public ActionResult DeletePieceArtist(int id)
        {
            PieceArtist PieceArtist = db.PieceArtists.Single(p => p.PieceArtist_Id == id);
            return PartialView("DeletePieceArtist", PieceArtist);
        }


        //
        // POST: /Event/DeletePieceArtist/5

        [HttpPost, ActionName("DeletePieceArtist")]
        [Authorize]
        public ActionResult DeletePieceArtistConfirmed(int id)
        {
            PieceArtist PieceArtist = db.PieceArtists.Single(p => p.PieceArtist_Id == id);
            if (PieceArtist != null)
            {
                db.PieceArtists.Remove(PieceArtist);
                db.SaveChanges();
            }
            return PartialView("Deleted");
        }







        // Role

        //
        // GET: /Event/NewRole/5
        [Authorize]
        public ActionResult NewRole(int PieceId)
        {
            Piece Piece = db.Pieces.Single(p => p.Piece_Id == PieceId);

            ViewBag.PieceId = PieceId;

            ViewBag.Role_TypeId = new SelectList(
                db.Types
                    .OrderBy(r => r.Type_Name),
                "Type_Id",
                "Type_Name");





            Role role = new Role();
            role.Role_PieceId = PieceId;

            role.Role_Rank = db.Roles.Where(r => r.Role_PieceId == PieceId).Select(r => r.Role_Rank).Max() + 1;

            return PartialView("NewRole", role);
        }

        //
        // POST: /Event/NewRole/5

        [HttpPost]
        [Authorize]
        public ActionResult NewRole(Role Role)
        {
            if (ModelState.IsValid)
            {
 
                db.Roles.Add(Role);
                db.SaveChanges();

                ViewBag.Type = db.Types.Find(Role.Role_TypeId);

                return PartialView("DetailsRole", Role);
            }
            else
            {
                return Content("Please review your form");
            }
        }

        //
        // GET: /Event/DeleteRole/5
        [Authorize]
        public ActionResult DeleteRole(int id)
        {
            Role Role = db.Roles.Single(p => p.Role_Id == id);
            return PartialView("DeleteRole", Role);
        }


        //
        // POST: /Event/DeleteRole/5

        [HttpPost, ActionName("DeleteRole")]
        [Authorize]
        public ActionResult DeleteRoleConfirmed(int id)
        {
            Role Role = db.Roles.Single(p => p.Role_Id == id);
            if (Role != null)
            {
                db.Roles.Remove(Role);
                db.SaveChanges();
            }
            return PartialView("Deleted");
        }







        public ActionResult Autocomplete_Artist_FullName(string term)
        {
            var artists = GetArtist_FullName(term.Trim()).Select(a => new { value = string.Format("{0} {1}", a.Artist_FirstName, a.Artist_LastName) }).Distinct();
            return Json(artists, JsonRequestBehavior.AllowGet);
        }
        private List<Artist> GetArtist_FullName(string searchString)
        {
            return db.Artists
            .Where(a => string.Concat(a.Artist_FirstName, " ", a.Artist_LastName).Contains(searchString))
            .OrderBy(a => a.Artist_FirstName)
            .ThenBy(a => a.Artist_LastName)
            .ToList();
        }











        //
        // GET: /Piece/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Piece piece = db.Pieces.Find(id);
            if (piece == null)
            {
                return HttpNotFound();
            }
            return View(piece);
        }


        //
        // POST: /Piece/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Piece piece = db.Pieces.Find(id);
            db.Pieces.Remove(piece);
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