﻿using System;
using System.Web;
using System.Web.Mvc;
using PerfV400.Models;
using System.Linq;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;




namespace PerfV400.Controllers
{
    public class HomeController : Controller
    {

        private PerfV100Entities db = new PerfV100Entities();

        public ActionResult Index()
        {
            // data for the Genre dropdown
            ViewBag.Event_GenreId = new SelectList(db.Genres.OrderBy(g => g.Genre_Name), "Genre_Id", "Genre_Name");

            // set up the filters
            DateTime datetimefilter_from_Event_Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            ViewData["filter_from_Event_Date"] = DateTime.Now.ToLongDateString();

            // data for the next events
            ViewBag.nextevents = db.Events
                .OrderBy(e => e.Event_Date)
                .Where(e => e.Event_Date >= datetimefilter_from_Event_Date)
                .Take(6);


            return View();
        }


    public ActionResult MoreStuff(string filter_search)
        {

            // set up the filters
            string strfilter_search = HttpUtility.UrlDecode((string)filter_search).ToUpper().Replace("  ", " ").Trim();

            DateTime datetimefilter_from_Event_Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);


            // data for the events
            var events = db.Events
                .OrderBy(e => e.Event_Date)
                .Where(e => strfilter_search == null || strfilter_search == "" || e.Event_Name.ToUpper().Contains(strfilter_search) || e.Event_Description.ToUpper().Contains(strfilter_search))
                .Where(e => e.Event_Date >= datetimefilter_from_Event_Date)
                .Take(6);
            if (events.Count() == 0)
            {
                events = db.Events
                    .OrderByDescending(e => e.Event_Date)
                    .Where(e => strfilter_search == null || strfilter_search == "" || e.Event_Name.ToUpper().Contains(strfilter_search) || e.Event_Description.ToUpper().Contains(strfilter_search))
                    .Take(6);
            }
            ViewBag.events = events;

            // data for the artists
            ViewBag.artists = db.Artists
                .OrderByDescending(a => a.PerformanceArtists.Count())
                .ThenBy(a => a.Artist_LastName).ThenBy(a => a.Artist_Middle_Names).ThenBy(a => a.Artist_FirstName)
                .Where(a => strfilter_search == null || strfilter_search == "" || a.Artist_LastName.ToUpper().Contains(strfilter_search) || a.Artist_Middle_Names.ToUpper().Contains(strfilter_search) || a.Artist_FirstName.ToUpper().Contains(strfilter_search))
                .Take(6);

            // data for the venues
            ViewBag.venues = db.Venues
                .OrderByDescending(v => v.Events.Count())
                .ThenBy(v => v.Venue_Name)
                .Where(v => strfilter_search == null || strfilter_search == "" || v.Venue_Name.ToUpper().Contains(strfilter_search))
                .Take(6);

            // data for the pieces
            ViewBag.pieces = db.Pieces
                .OrderByDescending(p => p.Performances.Count())
                .ThenBy(p => p.Piece_Name)
                .Where(p => strfilter_search == null || strfilter_search == "" || p.Piece_Name.ToUpper().Contains(strfilter_search))
                .Take(6);
           
            ViewBag.filter_search = strfilter_search;

            return PartialView("_MoreStuff");


        }


    public ActionResult Search(string filter_search)
    {

        // set up the filters
        string strfilter_search = HttpUtility.UrlDecode((string)filter_search).ToUpper().Replace("  ", " ").Trim();

        DateTime datetimefilter_from_Event_Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);


        // data for the events
        var events = db.Events
            .OrderBy(e => e.Event_Date)
            .Where(e => strfilter_search == null || strfilter_search == "" || e.Event_Name.ToUpper().Contains(strfilter_search) || e.Event_Description.ToUpper().Contains(strfilter_search))
            .Where(e => e.Event_Date >= datetimefilter_from_Event_Date)
            .Take(6);
        if (events.Count() == 0)
        {
            events = db.Events
                .OrderByDescending(e => e.Event_Date)
                .Where(e => strfilter_search == null || strfilter_search == "" || e.Event_Name.ToUpper().Contains(strfilter_search) || e.Event_Description.ToUpper().Contains(strfilter_search))
                .Take(6);
        }
        ViewBag.events = events;

        // data for the artists
        ViewBag.artists = db.Artists
            .OrderByDescending(a => a.PerformanceArtists.Count())
            .ThenBy(a => a.Artist_LastName).ThenBy(a => a.Artist_Middle_Names).ThenBy(a => a.Artist_FirstName)
            .Where(a => strfilter_search == null || strfilter_search == "" || a.Artist_LastName.ToUpper().Contains(strfilter_search) || a.Artist_Middle_Names.ToUpper().Contains(strfilter_search) || a.Artist_FirstName.ToUpper().Contains(strfilter_search))
            .Take(6);

        // data for the venues
        ViewBag.venues = db.Venues
            .OrderByDescending(v => v.Events.Count())
            .ThenBy(v => v.Venue_Name)
            .Where(v => strfilter_search == null || strfilter_search == "" || v.Venue_Name.ToUpper().Contains(strfilter_search))
            .Take(6);

        // data for the pieces
        ViewBag.pieces = db.Pieces
            .OrderByDescending(p => p.Performances.Count())
            .ThenBy(p => p.Piece_Name)
            .Where(p => strfilter_search == null || strfilter_search == "" || p.Piece_Name.ToUpper().Contains(strfilter_search))
            .Take(6);

        ViewBag.filter_search = strfilter_search;

        return View("Search");


    }
        
        
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
