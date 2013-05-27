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
    public class EventController : Controller
    {
        private PerfV100Entities db = new PerfV100Entities();

        //
        // GET: /Event/

        public ActionResult Index()
        {

            if (Request.IsAuthenticated)
            {
//                ViewBag.UserId = Membership.GetUser().ProviderUserKey;
            }

            // sort out the paging
            ViewBag.page = 0;

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

            // data for the Piece dropdown
            IEnumerable<SelectListItem> pieces = db.Pieces
                //.Where(p => db.Events.Select(e => e.Event_GenreId).Contains(g.Genre_Id))
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
            .Where(v => db.Events.Select(e => e.Event_VenueId).Contains(v.Venue_Id))
            .OrderBy(v => v.Venue_Name)
            .Select(v => new SelectListItem
            {
                Value = SqlFunctions.StringConvert((double)v.Venue_Id),
                Text = v.Venue_Name
            });
            ViewBag.Venues = venues;

            // data for the City dropdown
            IEnumerable<SelectListItem> cities = db.Venues
            .Where(v => db.Events.Select(e => e.Event_VenueId).Contains(v.Venue_Id))
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
            .Where(c => db.Venues.Where(v => db.Events.Select(e => e.Event_VenueId).Contains(v.Venue_Id)).Select(v => v.Venue_CountryId).Contains((Int32?)(c.CountryRegion_Id)))
            .OrderBy(c => c.CountryRegion_Name)
            .Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert((double)c.CountryRegion_Id),
                Text = c.CountryRegion_Name
            });
            ViewBag.Countries = countries;

            // data for the MyStatus dropdown
            IEnumerable<SelectListItem> mystatus = null;   
            if (Request.IsAuthenticated)
            {
                mystatus = db.EventUserStatus
                .OrderBy(c => c.EventUserStatus_Id)
                .Select(c => new SelectListItem
                {
                    Value = SqlFunctions.StringConvert((double)c.EventUserStatus_Id),
                    Text = c.EventUserStatus_Name
                });
                ViewBag.MyStatus = mystatus;
            }

            // set up the filters
            DateTime datetimefilter_from_Event_Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            ViewData["filter_from_Event_Date"] = DateTime.Now.ToLongDateString();

            // data for the events
            var events = db.Events
                .OrderBy(e => e.Event_Date)
                .Where(e => e.Event_Date >= datetimefilter_from_Event_Date)
                .Take(60);

            ViewBag.recordCount = db.Events
                .Count();

            
            return View(events.ToList());
        }


        public ActionResult MoreEvents(
            string filter_search,
            string filter_Event_GenreId,
            string filter_Performance_Piece,
            string filter_Event_Venue,
            string filter_Venue_City,
            string filter_Venue_CountryId,
            string filter_From_Event_Date,
            string filter_To_Event_Date,
            string filter_MyStatus,
            int page
            )
        {

            // Retrieve the User Id
            int intUsertId = 0;
            if (Request.IsAuthenticated)
            {
                intUsertId = (int)Membership.GetUser().ProviderUserKey;
            }

            
            // sort out the paging
            ViewBag.page = page;

            // set up the filters
            bool result;

            string strfilter_search = HttpUtility.UrlDecode((string)filter_search);

            int intfilter_Event_GenreId;
            result = int.TryParse(filter_Event_GenreId, out intfilter_Event_GenreId);

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

            int intfilter_MyStatus;
            result = int.TryParse(filter_MyStatus, out intfilter_MyStatus);


            // data for the events
            var events = db.Events
                .OrderBy(e => e.Event_Date)
                .Where(e => strfilter_search == null || strfilter_search == "" || e.Event_Name.ToUpper().Contains(strfilter_search.ToUpper()) || e.Event_Description.ToUpper().Contains(strfilter_search.ToUpper()))
                .Where(e => e.Event_Date >= datetimefilter_From_Event_Date)
                .Where(e => e.Event_Date <= datetimefilter_To_Event_Date)
                .Where(e => intfilter_Event_GenreId == 0 || e.Event_GenreId == intfilter_Event_GenreId)

                .Where(e => filter_Performance_Piece == null || filter_Performance_Piece == "" || db.Performances.Where(p => p.Piece.Piece_Name.ToUpper().Contains(filter_Performance_Piece.ToUpper())).Select(p => p.Performance_EventId).Contains(e.Event_Id))

                .Where(e => filter_Event_Venue == null || filter_Event_Venue == "" || e.Venue.Venue_Name.ToUpper().Contains(filter_Event_Venue.ToUpper()))

                .Where(e => filter_Venue_City == null || strfilter_Venue_City == "" || e.Venue.Venue_City == strfilter_Venue_City)
                .Where(e => intfilter_Venue_CountryId == 0 || e.Venue.Venue_CountryId == intfilter_Venue_CountryId)

                .Where(e => intfilter_MyStatus == 0 || e.EventUsers.Where(eu => eu.EventUser_StatusId == intfilter_MyStatus).Where(eu => eu.EventUser_UserId == intUsertId).Count() > 0)

                .Skip(page * 60)
                .Take(60);



            ViewBag.filter_search = strfilter_search;
            ViewBag.filter_Event_GenreId = filter_Event_GenreId;
            ViewBag.filter_Performance_Piece = filter_Performance_Piece;
            ViewBag.filter_Event_Venue = filter_Event_Venue;
            ViewBag.filter_Venue_City = strfilter_Venue_City;
            ViewBag.filter_Venue_CountryId = filter_Venue_CountryId;
            ViewBag.filter_from_Event_Date = filter_From_Event_Date;
            ViewBag.filter_to_Event_Date = filter_To_Event_Date;
            ViewBag.filter_MyStatus = filter_MyStatus;

            ViewBag.recordCount = db.Events
                .Where(c => strfilter_search == null || strfilter_search == "" || c.Event_Name.ToUpper().Contains(strfilter_search.ToUpper()) || c.Event_Description.ToUpper().Contains(strfilter_search.ToUpper()))
                .Where(c => c.Event_Date >= datetimefilter_From_Event_Date)
                .Where(c => c.Event_Date <= datetimefilter_To_Event_Date)
                .Where(c => intfilter_Event_GenreId == 0 || c.Event_GenreId == intfilter_Event_GenreId)
                .Where(c => filter_Performance_Piece == null || filter_Performance_Piece == "" || c.Performances.Select(p => p.Piece.Piece_Name).Contains(filter_Performance_Piece))
                .Where(e => filter_Event_Venue == null || filter_Event_Venue == "" || e.Venue.Venue_Name.Contains(filter_Event_Venue))
                .Where(c => filter_Venue_City == null || strfilter_Venue_City == "" || c.Venue.Venue_City == strfilter_Venue_City)
                .Where(c => intfilter_Venue_CountryId == 0 || c.Venue.Venue_CountryId == intfilter_Venue_CountryId)

                .Where(e => intfilter_MyStatus == 0 || e.EventUsers.Where(eu => eu.EventUser_StatusId == intfilter_MyStatus).Where(eu => eu.EventUser_UserId == intUsertId).Count() > 0)

                .Count();

            return PartialView("_MoreEvents", events);


        }


        public FileContentResult GetEventImage(int Id)
        {
            Event Xevent = db.Events.FirstOrDefault(e => e.Event_Id == Id);
            if (Xevent != null && Xevent.Event_Photo != null)
            {

                if (Xevent.Event_PhotoMimeType != null)
                {
                    return File(Xevent.Event_Photo, Xevent.Event_PhotoMimeType);
                }
                else
                {
                    return File(Xevent.Event_Photo, "JPEG");
                }
                
            }
            else
            {
                Venue venue = db.Venues.FirstOrDefault(v => v.Venue_Id == Xevent.Event_VenueId);
                if (venue != null && venue.Venue_Photo != null)
                {
                    if (venue.Venue_PhotoMimeType != null)
                    {
                        return File(venue.Venue_Photo, venue.Venue_PhotoMimeType);
                    }
                    else
                    {
                        return File(venue.Venue_Photo, "JPEG");
                    }
                }
                else
                {
                    return null;
                }
            }
        }








        //
        // GET: /Event/Details/5

        public ActionResult Details(int id = 0)
        {
            Event eventX = db.Events.Find(id);
            if (eventX == null)
            {
                return HttpNotFound();
            }

            // Can the current user edit this event?
            if (Request.IsAuthenticated)
            {
                ViewBag.UserCanEditEvent = true;
                int intUserId = (int)Membership.GetUser().ProviderUserKey;


                ViewBag.Event_Date = eventX.Event_Date;
                ViewBag.EventUser_EventId = id;
                ViewBag.EventUser_UserId = intUserId;
                ViewBag.EventUser_UserRating = "";

                EventUser eventUser = db.EventUsers.FirstOrDefault(eu => eu.EventUser_EventId == id && eu.EventUser_UserId == intUserId);
                if (eventUser != null)
                {
                    ViewBag.EventUser_Id = eventUser.EventUser_Id;
                    ViewBag.EventUser_StatusId = eventUser.EventUser_StatusId;
                    if (eventUser.EventUser_StatusId == null)
                    {
                        ViewBag.EventUser_StatusId = 0;
                    }
                    ViewBag.EventUser_UserRating = eventUser.EventUser_UserRating;
                    ViewBag.EventUser_Note = eventUser.EventUser_Note;
                }

                // data for the MyStatus dropdown
                if (eventX.Event_Date < DateTime.Today)
                {
                    var query = db.EventUserStatus.OrderBy(c => c.EventUserStatus_Id)
                        .Select(c => new
                        {
                            c.EventUserStatus_Id,
                            c.EventUserStatus_Past
                        });

                    ViewBag.MyStatus = new SelectList(query.AsEnumerable(), "EventUserStatus_Id", "EventUserStatus_Past", ViewBag.EventUser_StatusId);
                }
                else
                {
                    var query = db.EventUserStatus.OrderBy(c => c.EventUserStatus_Id)
                        .Select(c => new
                        {
                            c.EventUserStatus_Id,
                            c.EventUserStatus_Future
                        });

                    ViewBag.MyStatus = new SelectList(query.AsEnumerable(), "EventUserStatus_Id", "EventUserStatus_Future", ViewBag.EventUser_StatusId);
                }


            }
            else
            {
                ViewBag.UserCanEditEvent = false;
            }



            // data for the venue
            ViewBag.Venue = db.Venues.FirstOrDefault(v => v.Venue_Id == eventX.Event_VenueId);

            // data for the genre
            ViewBag.Genre = db.Genres.FirstOrDefault(g => g.Genre_Id == eventX.Event_GenreId);

            // data for the performances
            ViewBag.performances = db.Performances
                .Where(p => p.Performance_EventId == id)
                .OrderBy(p => p.Performance_Order)
                .Take(60);

            ViewBag.recordCount = db.Performances
                .Where(p => p.Performance_EventId == id)
            .Count();


            return View(eventX);
        }


        public ActionResult Update_EventUser_StatusId(
            int EventUser_UserId,
            int EventUser_EventId,
            int EventUser_StatusId
            )
        {
            EventUser eventUser = db.EventUsers.FirstOrDefault(eu => eu.EventUser_EventId == EventUser_EventId && eu.EventUser_UserId == EventUser_UserId);
            if (eventUser == null)
            {
                // insert new
                EventUser newEventUser = new EventUser();
                newEventUser.EventUser_UserId = EventUser_UserId;
                newEventUser.EventUser_EventId = EventUser_EventId;
                newEventUser.EventUser_StatusId = EventUser_StatusId;
                db.EventUsers.Add(newEventUser);
                db.SaveChanges();

            }
            else
            {
                // update
                eventUser.EventUser_StatusId = EventUser_StatusId;
                db.Entry(eventUser).State = EntityState.Modified;
                db.SaveChanges();
            }

            return null;
        }

        public ActionResult Update_EventUser_UserRating(
            int EventUser_UserId,
            int EventUser_EventId,
            int EventUser_UserRating
            )
        {
            EventUser eventUser = db.EventUsers.FirstOrDefault(eu => eu.EventUser_EventId == EventUser_EventId && eu.EventUser_UserId == EventUser_UserId);
            if (eventUser == null)
            {
                // insert new
                EventUser newEventUser = new EventUser();
                newEventUser.EventUser_UserId = EventUser_UserId;
                newEventUser.EventUser_EventId = EventUser_EventId;
                newEventUser.EventUser_UserRating = EventUser_UserRating;
                db.EventUsers.Add(newEventUser);
                db.SaveChanges();

            }
            else
            {
                // update
                eventUser.EventUser_UserRating = EventUser_UserRating;
                db.Entry(eventUser).State = EntityState.Modified;
                db.SaveChanges();
            }

            return null;
        }



        public ActionResult Update_EventUser_Note(
            int EventUser_UserId,
            int EventUser_EventId,
            string EventUser_Note
            )
        {
            EventUser eventUser = db.EventUsers.FirstOrDefault(eu => eu.EventUser_EventId == EventUser_EventId && eu.EventUser_UserId == EventUser_UserId);
            if (eventUser == null)
            {
                // insert new
                EventUser newEventUser = new EventUser();
                newEventUser.EventUser_UserId = EventUser_UserId;
                newEventUser.EventUser_EventId = EventUser_EventId;
                newEventUser.EventUser_Note = EventUser_Note;
                db.EventUsers.Add(newEventUser);
                db.SaveChanges();

            }
            else
            {
                // update
                eventUser.EventUser_Note = EventUser_Note;
                db.Entry(eventUser).State = EntityState.Modified;
                db.SaveChanges();
            }

            return null;
        }





        public ActionResult MoreEventPerformances(
            string filter_Event_Id,
            int page
            )
        {

            ViewBag.Event_Id = filter_Event_Id;

            // sort out the paging
            ViewBag.page = page;

            // set up the filters
            bool result;

            int intfilter_Event_Id;
            result = int.TryParse(filter_Event_Id, out intfilter_Event_Id);

            // data for the performances
            var performances = db.Performances
                .Where(p => p.Performance_EventId == intfilter_Event_Id)
                .OrderBy(p => p.Performance_Order)
                .Skip(page * 1)
                .Take(60);

            ViewBag.recordCount = db.Performances
                .Where(p => p.Performance_EventId == intfilter_Event_Id)
            .Count();

            return PartialView("_MoreEventPerformances", performances);
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
        public WrappedJsonResult EditEventPhoto(HttpPostedFileWrapper imageFile, int Event_Id)
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

            Event Xevent = db.Events.FirstOrDefault(e => e.Event_Id == Event_Id);
            Xevent.Event_Photo = fileData;
            Xevent.Event_PhotoMimeType = mimeType;
            Xevent.Event_PhotoFileName = fileName;

            //db.Events.Attach(Xevent);
            db.Entry(Xevent).State = EntityState.Modified;
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
        // GET: /Event/Create

        [Authorize]
        public ActionResult CreateEvent()
        {
            ViewBag.Event_BandId = new SelectList(db.Bands.OrderBy(b => b.Band_Name), "Band_Id", "Band_Name");
            ViewBag.Event_GenreId = new SelectList(db.Genres.OrderBy(g => g.Genre_Name), "Genre_Id", "Genre_Name");
            ViewBag.Event_VenueId = new SelectList(db.Venues.OrderBy(v => v.Venue_Name), "Venue_Id", "Venue_Name");
            return View();
        }

        //
        // POST: /Event/Create

        [HttpPost]
        [Authorize]
        public ActionResult CreateEvent(Event eventX, string Event_VenueName)
        {
            if (ModelState.IsValid)
            {

                Venue venue = db.Venues.FirstOrDefault(v => v.Venue_Name.Equals(Event_VenueName));
                if (venue != null)
                {
                    //attach the venue
                    eventX.Event_VenueId = venue.Venue_Id;
                }
                else
                {
                    //create a new venue
                    Venue newVenue = new Venue();

                    // remove any unnecessary spaces
                    Event_VenueName.Replace("  ", " ").Trim();
                    newVenue.Venue_Name = Event_VenueName;

                    db.Venues.Add(newVenue);
                    eventX.Event_VenueId = newVenue.Venue_Id;

                }


                db.Events.Add(eventX);
                db.SaveChanges();

                // data for the performances
                ViewBag.performances = db.Performances
                    .Where(p => p.Performance_EventId == eventX.Event_Id)
                    .OrderBy(p => p.Performance_Order)
                    .Take(60);

                ViewBag.recordCount = db.Performances
                    .Where(p => p.Performance_EventId == eventX.Event_Id)
                .Count();

                // data for the venue
                ViewBag.Venue = db.Venues.FirstOrDefault(v => v.Venue_Id == eventX.Event_VenueId);

                // data for the genre
                ViewBag.Genre = db.Genres.FirstOrDefault(g => g.Genre_Id == eventX.Event_GenreId);

                // data for the MyStatus dropdown
                if (eventX.Event_Date < DateTime.Today)
                {
                    var query = db.EventUserStatus.OrderBy(c => c.EventUserStatus_Id)
                        .Select(c => new
                        {
                            c.EventUserStatus_Id,
                            c.EventUserStatus_Past
                        });

                    ViewBag.MyStatus = new SelectList(query.AsEnumerable(), "EventUserStatus_Id", "EventUserStatus_Past", ViewBag.EventUser_StatusId);
                }
                else
                {
                    var query = db.EventUserStatus.OrderBy(c => c.EventUserStatus_Id)
                        .Select(c => new
                        {
                            c.EventUserStatus_Id,
                            c.EventUserStatus_Future
                        });

                    ViewBag.MyStatus = new SelectList(query.AsEnumerable(), "EventUserStatus_Id", "EventUserStatus_Future", ViewBag.EventUser_StatusId);
                }

                // Can the current user edit this event?
                ViewBag.UserCanEditEvent = true;


                return View("Details", eventX);
            }
            else
            {
                return Content("Please review your form");
            }
        }


        //
        // GET: /Event/EditEvent/5
        [Authorize]
        public ActionResult EditEvent(int id)
        {
            Event Xevent = db.Events.Single(e => e.Event_Id == id);
            ViewBag.Event_BandId = new SelectList(db.Bands.OrderBy(b => b.Band_Name), "Band_Id", "Band_Name", Xevent.Event_BandId);
            ViewBag.Event_GenreId = new SelectList(db.Genres.OrderBy(g => g.Genre_Name), "Genre_Id", "Genre_Name", Xevent.Event_GenreId);
            ViewBag.Event_VenueId = new SelectList(db.Venues.OrderBy(v => v.Venue_Name), "Venue_Id", "Venue_Name", Xevent.Event_VenueId);
            return PartialView(Xevent);
        }

        //
        // POST: /Event/EditEvent/5

        [HttpPost]
        [Authorize]
        public ActionResult EditEvent(Event Xevent, string VenueName)
        {
            if (ModelState.IsValid)
            {

                Venue venue = db.Venues.FirstOrDefault(a => a.Venue_Name == VenueName);
                if (venue != null)
                {
                    //attach the artist
                    Xevent.Event_VenueId = venue.Venue_Id;
                }
                else
                {
                    //create a new piece

                    // remove any unnecessary spaces
                    VenueName.Replace("  ", " ").Trim();

                    Venue newVenue = new Venue();
                    newVenue.Venue_Name = VenueName;
                    db.Venues.Add(newVenue);

                    Xevent.Event_VenueId = newVenue.Venue_Id;
                }
                
                db.Entry(Xevent).State = EntityState.Modified;

                ViewBag.Venue = db.Venues.FirstOrDefault(v => v.Venue_Id == Xevent.Event_VenueId);

                // data for the genre
                ViewBag.Genre = db.Genres.FirstOrDefault(g => g.Genre_Id == Xevent.Event_GenreId);

                return PartialView("DetailsEvent", Xevent);
            }
            else
            {
                return Content("Please review your form");
            }
        }








        //
        // GET: /Event/CopyEvent/5
        [Authorize]
        public ActionResult CopyEvent(int id = 0)
        {
            Event xEvent = db.Events.Single(e => e.Event_Id == id);

            Event newEvent = new Event();

            newEvent.Event_BandId = xEvent.Event_BandId;
            newEvent.Event_Date = xEvent.Event_Date;
            newEvent.Event_Description = xEvent.Event_Description;
            newEvent.Event_EndTime = xEvent.Event_EndTime;
            newEvent.Event_GenreId = xEvent.Event_GenreId;
            newEvent.Event_Name = xEvent.Event_Name;
            newEvent.Event_Photo = xEvent.Event_Photo;
            newEvent.Event_PhotoFileName = xEvent.Event_PhotoFileName;
            newEvent.Event_PhotoMimeType = xEvent.Event_PhotoMimeType;
            newEvent.Event_StartTime = xEvent.Event_StartTime;
            newEvent.Event_VenueId = xEvent.Event_VenueId;

            ViewBag.oldEventId = id;
            ViewBag.VenueName = xEvent.Venue.Venue_Name;

            ViewBag.Event_BandId = new SelectList(db.Bands.OrderBy(b => b.Band_Name), "Band_Id", "Band_Name", newEvent.Event_BandId);
            ViewBag.Event_GenreId = new SelectList(db.Genres.OrderBy(g => g.Genre_Name), "Genre_Id", "Genre_Name", newEvent.Event_GenreId);
            
            return PartialView(newEvent);
        }

        //
        // POST: /Event/CopyEvent/5

        [HttpPost]
        [Authorize]
        public ActionResult CopyEvent(Event xEvent, int oldEventId, string VenueName)
        {
            if (ModelState.IsValid)
            {

                // Sort out the venue
                Venue venue = db.Venues.FirstOrDefault(a => a.Venue_Name == VenueName);
                if (venue != null)
                {
                    //attach the artist
                    xEvent.Event_VenueId = venue.Venue_Id;
                }
                else
                {
                    //create a new piece

                    // remove any unnecessary spaces
                    VenueName.Replace("  ", " ").Trim();

                    Venue newVenue = new Venue();
                    newVenue.Venue_Name = VenueName;
                    db.Venues.Add(newVenue);

                    xEvent.Event_VenueId = newVenue.Venue_Id;
                }

                // copy over all the child objects
                Event oldEvent = db.Events.Single(e => e.Event_Id == oldEventId);


                foreach (PerfV400.Models.Performance performance in oldEvent.Performances)
                {

                    Performance newPerformance = new Performance();
                    newPerformance.Performance_EventId = xEvent.Event_Id;
                    newPerformance.Performance_Order = performance.Performance_Order;
                    newPerformance.Performance_Photo = performance.Performance_Photo;
                    newPerformance.Performance_PhotoFileName = performance.Performance_PhotoFileName;
                    newPerformance.Performance_PhotoMimeType = performance.Performance_PhotoMimeType;
                    newPerformance.Performance_PieceId = performance.Performance_PieceId;
                    newPerformance.Performance_ProductionId = performance.Performance_ProductionId;

                    db.Performances.Add(newPerformance);
                    db.Entry(newPerformance).State = EntityState.Added;


                    foreach (PerfV400.Models.PerformanceArtist performanceArtist in performance.PerformanceArtists)
                    {

                        PerformanceArtist newPerformanceArtist = new PerformanceArtist();
                        newPerformanceArtist.PerformanceArtist_ArtistId = performanceArtist.PerformanceArtist_ArtistId;
                        newPerformanceArtist.PerformanceArtist_Comments = performanceArtist.PerformanceArtist_Comments;
                        newPerformanceArtist.PerformanceArtist_PerformanceId = newPerformance.Performance_Id;
                        newPerformanceArtist.PerformanceArtist_Photo = performanceArtist.PerformanceArtist_Photo;
                        newPerformanceArtist.PerformanceArtist_RoleId = performanceArtist.PerformanceArtist_RoleId;

                        db.PerformanceArtists.Add(newPerformanceArtist);
                        db.Entry(newPerformanceArtist).State = EntityState.Added;

                    }
                
                }

                db.Events.Add(xEvent);
                db.SaveChanges();

                

// Retrieve the data for the view

                // data for the performances
                ViewBag.performances = db.Performances
                    .Where(p => p.Performance_EventId == xEvent.Event_Id)
                    .OrderBy(p => p.Performance_Order)
                    .Take(60)
                    .Include(p => p.Piece.Genre)
                    .Include(p => p.PerformanceArtists.Select(pa => pa.Role))
                    .Include(p => p.PerformanceArtists.Select(pa => pa.Artist))
                    ;

                ViewBag.recordCount = db.Performances
                    .Where(p => p.Performance_EventId == xEvent.Event_Id)
                .Count();

                // data for the venue
                ViewBag.Venue = db.Venues.FirstOrDefault(v => v.Venue_Id == xEvent.Event_VenueId);

                // data for the genre
                ViewBag.Genre = db.Genres.FirstOrDefault(g => g.Genre_Id == xEvent.Event_GenreId);

                // Can the current user edit this event?
                ViewBag.UserCanEditEvent = true;

                return PartialView("Details", xEvent);

            }
            else
            {
                return Content("Please review your form");
            }
        }









        //
        // GET: /Event/NewPerformance/5
        [Authorize]
        public ActionResult NewPerformance(int EventId)
        {
            Event xevent = db.Events.Single(e => e.Event_Id == EventId);
            int GenreId = db.Events.Single(e => e.Event_Id == EventId).Event_GenreId;

            ViewBag.EventId = EventId;

            ViewBag.Performance_PieceId = new SelectList(db.Pieces
                .Where(p => p.Piece_GenreId == GenreId)
                .OrderBy(p => p.Piece_Name), "Piece_Id", "Piece_Name");


            ViewBag.Performance_ProductionId = new SelectList(db.Productions.OrderBy(p => p.Production_Name), "Production_Id", "Production_Name");

            Performance performance = new Performance();
            performance.Performance_EventId = EventId;

            return PartialView("NewPerformance", performance);
        }

        //
        // POST: /Event/NewPerformance/5

        [HttpPost]
        [Authorize]
        public ActionResult NewPerformance(Performance performance, string PieceName)
        {
            if (ModelState.IsValid)
            {

                Piece piece = db.Pieces.FirstOrDefault(a => a.Piece_Name == PieceName);
                if (piece != null)
                {
                    //attach the artist
                    performance.Performance_PieceId = piece.Piece_Id;
                }
                else
                {
                    //create a new piece

                    // remove any unnecessary spaces
                    PieceName.Replace("  ", " ").Trim();

                    Piece newPiece = new Piece();
                    newPiece.Piece_Name = PieceName;
                    db.Pieces.Add(newPiece);

                    performance.Performance_PieceId = newPiece.Piece_Id;
                }

                db.Performances.Attach(performance);
                db.Entry(performance).State = EntityState.Added;
                db.SaveChanges();

                ViewBag.Piece = db.Pieces.Find(performance.Performance_PieceId);

                // Can the current user edit this event?
                ViewBag.UserCanEditEvent = true;

                return PartialView("DetailsPerformance", performance);
            }
            else
            {
                return Content("Please review your form");
            }
        }















        //
        // GET: /Event/EditPerformance/5
        [Authorize]
        public ActionResult EditPerformance(int id)
        {
            Performance performance = db.Performances.Single(p => p.Performance_Id == id);
            ViewBag.Performance_PieceId = new SelectList(db.Pieces.OrderBy(p => p.Piece_Name), "Piece_Id", "Piece_Name", performance.Performance_PieceId);
            ViewBag.Performance_ProductionId = new SelectList(db.Productions.OrderBy(p => p.Production_Name), "Production_Id", "Production_Name", performance.Performance_ProductionId);
            return PartialView(performance);
        }

        //
        // POST: /Event/EditPerformance/5

        [HttpPost]
        [Authorize]
        public ActionResult EditPerformance(Performance performance, string PieceName)
        {
            if (ModelState.IsValid)
            {
                Piece piece = db.Pieces.FirstOrDefault(a => a.Piece_Name == PieceName);
                if (piece != null)
                {
                    //attach the artist
                    performance.Performance_PieceId = piece.Piece_Id;
                }
                else
                {
                    //create a new piece

                    // remove any unnecessary spaces
                    PieceName.Replace("  ", " ").Trim();

                    Piece newPiece = new Piece();
                    newPiece.Piece_Name = PieceName;
                    db.Pieces.Add(newPiece);

                    performance.Performance_PieceId = newPiece.Piece_Id;
                }

                db.Entry(performance).State = EntityState.Modified;
                db.SaveChanges();

                return PartialView("DetailsPerformance", performance);
            }
            else
            {
                return Content("Please review your form");
            }
        }









        //
        // GET: /Event/DeletePerformance/5
        [Authorize]
        public ActionResult DeletePerformance(int id)
        {
            Performance performance = db.Performances.Single(p => p.Performance_Id == id);
            return PartialView("DeletePerformance", performance);
        }



        //
        // POST: /Event/DeletePerformance/5

        [HttpPost, ActionName("DeletePerformance")]
        [Authorize]
        public ActionResult DeletePerformanceConfirmed(int id)
        {
            Performance performance = db.Performances.Single(p => p.Performance_Id == id);
            if (performance != null)
            {
                db.Performances.Remove(performance);
                db.SaveChanges();
            }
            return PartialView("Deleted");
        }










        // PERFORMANCE ARTIST

        //
        // GET: /Event/NewPerformanceArtist/5
        [Authorize]
        public ActionResult NewPerformanceArtist(int PerformanceId)
        {
            Performance performance = db.Performances.Single(p => p.Performance_Id == PerformanceId);

            int GenreId = db.Performances.Single(p => p.Performance_Id == PerformanceId).Event.Event_GenreId;
            int PieceId = db.Performances.Single(p => p.Performance_Id == PerformanceId).Performance_PieceId;

            ViewBag.PerformanceId = PerformanceId;

            ViewBag.PerformanceArtist_RoleId = new SelectList(
                db.Roles
                    .Where(r => r.Role_GenreId == GenreId || r.Role_PieceId == PieceId)
                    .OrderBy(r => r.Role_Rank),

                "Role_Id",
                "Role_Name");

            PerformanceArtist performanceArtist = new PerformanceArtist();
            performanceArtist.PerformanceArtist_PerformanceId = PerformanceId;

            return PartialView("NewPerformanceArtist", performanceArtist);
        }

        //
        // POST: /Event/NewPerformanceArtist/5

        [HttpPost]
        [Authorize]
        public ActionResult NewPerformanceArtist(PerformanceArtist performanceArtist, string PerformanceArtist_ArtistFullName)
        {
            if (ModelState.IsValid)
            {
                Artist artist = db.Artists.FirstOrDefault(a => string.Concat(a.Artist_FirstName, " ", a.Artist_LastName).Equals(PerformanceArtist_ArtistFullName));
                if (artist != null)
                {
                    //attach the artist
                    performanceArtist.PerformanceArtist_ArtistId = artist.Artist_Id;
                }
                else
                {
                    //create a new artist

                    // remove any unnecessary spaces
                    PerformanceArtist_ArtistFullName = PerformanceArtist_ArtistFullName.Replace("  ", " ").Trim();

                    string firstName = "";
                    string lastName;
                    
                    // find the position of the last space
                    int i = PerformanceArtist_ArtistFullName.LastIndexOf(" ");

                    if (i > 0)
                    { 
                        //there are two words or more
                        firstName = PerformanceArtist_ArtistFullName.Substring(0, i).Trim();
                        lastName = PerformanceArtist_ArtistFullName.Substring(i, PerformanceArtist_ArtistFullName.Length-i).Trim();

                    }
                    else
                    { //there's only one word
                        lastName = PerformanceArtist_ArtistFullName;
                    }

                    Artist newArtist = new Artist();
                    newArtist.Artist_FirstName = firstName;
                    newArtist.Artist_LastName = lastName;
                    db.Artists.Add(newArtist);

                    performanceArtist.PerformanceArtist_ArtistId = newArtist.Artist_Id;
                
                }
                db.PerformanceArtists.Add(performanceArtist);
                db.SaveChanges();

                ViewBag.artist = db.Artists.Find(performanceArtist.PerformanceArtist_ArtistId);
                ViewBag.role = db.Roles.Find(performanceArtist.PerformanceArtist_RoleId);

                // Can the current user edit this event?
                ViewBag.UserCanEditEvent = true;

                return PartialView("DetailsPerformanceArtist", performanceArtist);
            }
            else
            {
                return Content("Please review your form");
            }
        }




        //
        // GET: /Event/EditPerformanceArtist/5
        [Authorize]
        public ActionResult EditPerformanceArtist(int id)
        {
            ViewBag.PerformanceArtistId = id;

            PerformanceArtist performanceArtist = db.PerformanceArtists.Single(p => p.PerformanceArtist_Id == id);

            int PieceId = performanceArtist.Performance.Piece.Piece_Id;
            int GenreId = performanceArtist.Performance.Piece.Genre.Genre_Id;

            ViewBag.PerformanceArtist_RoleId = new SelectList(
                db.Roles
                    .Where(r => r.Role_GenreId == GenreId || r.Role_PieceId == PieceId)
                    .OrderBy(r => r.Role_Rank),

                "Role_Id",
                "Role_Name",
                performanceArtist.PerformanceArtist_RoleId);

            return PartialView(performanceArtist);
        }

        //
        // POST: /Event/EditPerformanceArtist/5

        [HttpPost]
        [Authorize]
        public ActionResult EditPerformanceArtist(PerformanceArtist performanceArtist, string PerformanceArtist_ArtistFullName)
        {
            if (ModelState.IsValid)
            {
                Artist artist = db.Artists.FirstOrDefault(a => string.Concat(a.Artist_FirstName, " ", a.Artist_LastName).Equals(PerformanceArtist_ArtistFullName));
                if (artist != null)
                {
                    //attach the artist
                    performanceArtist.PerformanceArtist_ArtistId = artist.Artist_Id;
                }
                else
                {
                    //create a new artist

                    // remove any unnecessary spaces
                    PerformanceArtist_ArtistFullName = PerformanceArtist_ArtistFullName.Replace("  ", " ").Trim();

                    string firstName = "";
                    string lastName;

                    // find the position of the last space
                    int i = PerformanceArtist_ArtistFullName.LastIndexOf(" ");

                    if (i > 0)
                    {
                        //there are two words or more
                        firstName = PerformanceArtist_ArtistFullName.Substring(0, i).Trim();
                        lastName = PerformanceArtist_ArtistFullName.Substring(i, PerformanceArtist_ArtistFullName.Length - i).Trim();

                    }
                    else
                    { //there's only one word
                        lastName = PerformanceArtist_ArtistFullName;
                    }

                    Artist newArtist = new Artist();
                    newArtist.Artist_FirstName = firstName;
                    newArtist.Artist_LastName = lastName;
                    db.Artists.Add(newArtist);

                    performanceArtist.PerformanceArtist_ArtistId = newArtist.Artist_Id;

                }

                db.Entry(performanceArtist).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.artist = db.Artists.Find(performanceArtist.PerformanceArtist_ArtistId);
                ViewBag.role = db.Roles.Find(performanceArtist.PerformanceArtist_RoleId);

                // Can the current user edit this event?
                ViewBag.UserCanEditEvent = true;

                return PartialView("DetailsPerformanceArtist", performanceArtist);
            }
            else
            {
                return Content("Please review your form");
            }
        }










        //
        // GET: /Event/DeletePerformanceArtist/5
        [Authorize]
        public ActionResult DeletePerformanceArtist(int id)
        {
            PerformanceArtist performanceArtist = db.PerformanceArtists.Single(p => p.PerformanceArtist_Id == id);
            return PartialView("DeletePerformanceArtist", performanceArtist);
        }


        //
        // POST: /Event/DeletePerformanceArtist/5

        [HttpPost, ActionName("DeletePerformanceArtist")]
        [Authorize]
        public ActionResult DeletePerformanceArtistConfirmed(int id)
        {
            PerformanceArtist performanceArtist = db.PerformanceArtists.Single(p => p.PerformanceArtist_Id == id);
            if (performanceArtist != null)
            {
                db.PerformanceArtists.Remove(performanceArtist);
                db.SaveChanges();
            }
            return PartialView("Deleted");
        }



















        //
        // GET: /Event/Edit/5
        [Authorize]

        public ActionResult Edit(int id = 0)
        {
            Event eventX = db.Events.Find(id);
            if (eventX == null)
            {
                return HttpNotFound();
            }
            ViewBag.Event_BandId = new SelectList(db.Bands.OrderBy(b => b.Band_Name), "Band_Id", "Band_Name", eventX.Event_BandId);
            ViewBag.Event_GenreId = new SelectList(db.Genres.OrderBy(g => g.Genre_Name), "Genre_Id", "Genre_Name", eventX.Event_GenreId);
            ViewBag.Event_VenueId = new SelectList(db.Venues.OrderBy(v => v.Venue_Name), "Venue_Id", "Venue_Name", eventX.Event_VenueId);
            return View(eventX);
        }

        //
        // POST: /Event/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult Edit(Event eventX)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventX).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Event_BandId = new SelectList(db.Bands.OrderBy(b => b.Band_Name), "Band_Id", "Band_Name", eventX.Event_BandId);
            ViewBag.Event_GenreId = new SelectList(db.Genres.OrderBy(g => g.Genre_Name), "Genre_Id", "Genre_Name", eventX.Event_GenreId);
            ViewBag.Event_VenueId = new SelectList(db.Venues.OrderBy(v => v.Venue_Name), "Venue_Id", "Venue_Name", eventX.Event_VenueId);
            return View(eventX);
        }

        //
        // GET: /Event/DeleteEvent/5

        [Authorize]
        public ActionResult DeleteEvent(int id = 0)
        {
            Event eventX = db.Events.Find(id);
            if (eventX == null)
            {
                return HttpNotFound();
            }

            return PartialView("DeleteEvent", eventX);
 
        }

        //
        // POST: /Event/Delete/5

        [HttpPost, ActionName("DeleteEvent")]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Event eventX = db.Events.Single(p => p.Event_Id == id);
            if (eventX != null)
            {
                db.Events.Remove(eventX);
                db.SaveChanges();
            } 
            return PartialView("Deleted");
        }




















 

        public ActionResult Autocomplete_Artist_FullName(string term)
        {


            var artists1 = db.Artists
                .Where(a => string.Concat(a.Artist_FirstName, " ", a.Artist_LastName).Contains(term.Trim()))
                .OrderBy(a => a.Artist_FirstName)
                .ThenBy(a => a.Artist_LastName)
                .ToList()
                .Select(a => new { value = string.Format("{0} {1}", a.Artist_FirstName, a.Artist_LastName) })
                .Distinct();

            //work out the current user id
            int intUserId = (int)Membership.GetUser().ProviderUserKey;
            webpages_OAuthMembership OAuth = db.webpages_OAuthMembership.FirstOrDefault(oa => oa.UserId == intUserId);

            int intFacebookId = int.Parse(OAuth.ProviderUserId);

            var artists2 = db.UserFriends
                .Where(a => a.UserFriend_Name.Contains(term.Trim()))
                .Where(a => a.UserFriend_UserFacebookId == intFacebookId)
                .OrderBy(a => a.UserFriend_Name)
                .ToList()
                .Select(a => new { value = string.Format("{0} (facebook)", a.UserFriend_Name) })
                .Distinct();
            
            return Json(artists1.Concat(artists2.OrderBy(v => v.value)), JsonRequestBehavior.AllowGet);
            
        }

        private List<Artist> GetArtist_FullName(string searchString)
        {

            List<Artist> artists = db.Artists
                .Where(a => string.Concat(a.Artist_FirstName, " ", a.Artist_LastName).Contains(searchString))
                .OrderBy(a => a.Artist_FirstName)
                .ThenBy(a => a.Artist_LastName)
                .ToList();

            List<UserFriend> userFriends = db.UserFriends
                .Where(a => a.UserFriend_Name.Contains(searchString))
                .OrderBy(a => a.UserFriend_Name)
                .ToList();


            return artists;
        }


        public ActionResult Autocomplete_Event_VenueName(string term)
        {
            var venues = GetVenueName(term.Trim()).Select(v => new { value = v.Venue_Name }).Distinct();
            return Json(venues, JsonRequestBehavior.AllowGet);
        }
        private List<Venue> GetVenueName(string searchString)
        {
            return db.Venues
            .Where(v => v.Venue_Name.Contains(searchString))
            .OrderBy(v => v.Venue_Name)
            .ToList();
        }



        public ActionResult Autocomplete_PieceName(string term)
        {
            var pieces = GetPieceName(term.Trim()).Select(a => new { value = a.Piece_Name }).Distinct();
            return Json(pieces, JsonRequestBehavior.AllowGet);
        }
        private List<Piece> GetPieceName(string searchString)
        {
            return db.Pieces
            .Where(p => p.Piece_Name.Contains(searchString))
            .OrderBy(p => p.Piece_Name)
            .ToList();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public int intUserId { get; set; }
    }
}