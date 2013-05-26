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
    public class PerformanceController : Controller
    {
        private PerfV100Entities db = new PerfV100Entities();

        //
        // GET: /Performance/

        public ActionResult Index()
        {
            var performances = db.Performances.Include(p => p.Event).Include(p => p.Piece).Include(p => p.Production);
            return View(performances.ToList());
        }

        //
        // GET: /Performance/Details/5

        public ActionResult Details(int id = 0)
        {
            Performance performance = db.Performances.Find(id);
            if (performance == null)
            {
                return HttpNotFound();
            }
            return View(performance);
        }

        //
        // GET: /Performance/Create

        public ActionResult Create()
        {
            ViewBag.Performance_PieceId = new SelectList(db.Pieces, "Piece_Id", "Piece_Name");
            ViewBag.Performance_ProductionId = new SelectList(db.Productions, "Production_Id", "Production_Name");
            return View();
        }

        //
        // POST: /Performance/Create

        [HttpPost]
        public ActionResult Create(Performance performance)
        {
            if (ModelState.IsValid)
            {
                db.Performances.Add(performance);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Performance_PieceId = new SelectList(db.Pieces, "Piece_Id", "Piece_Name", performance.Performance_PieceId);
            ViewBag.Performance_ProductionId = new SelectList(db.Productions, "Production_Id", "Production_Name", performance.Performance_ProductionId);
            return View(performance);
        }

        //
        // GET: /Performance/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Performance performance = db.Performances.Find(id);
            if (performance == null)
            {
                return HttpNotFound();
            }
            ViewBag.Performance_PieceId = new SelectList(db.Pieces, "Piece_Id", "Piece_Name", performance.Performance_PieceId);
            ViewBag.Performance_ProductionId = new SelectList(db.Productions, "Production_Id", "Production_Name", performance.Performance_ProductionId);
            return View(performance);
        }

        //
        // POST: /Performance/Edit/5

        [HttpPost]
        public ActionResult Edit(Performance performance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(performance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Performance_PieceId = new SelectList(db.Pieces, "Piece_Id", "Piece_Name", performance.Performance_PieceId);
            ViewBag.Performance_ProductionId = new SelectList(db.Productions, "Production_Id", "Production_Name", performance.Performance_ProductionId);
            return View(performance);
        }

        //
        // GET: /Performance/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Performance performance = db.Performances.Find(id);
            if (performance == null)
            {
                return HttpNotFound();
            }
            return View(performance);
        }

        //
        // POST: /Performance/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Performance performance = db.Performances.Find(id);
            db.Performances.Remove(performance);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public FileContentResult GetPerformanceImage(int id)
        {
            if (id == 0)
            {
                return null;
            }
            else
            {
                Performance performance = db.Performances.FirstOrDefault(p => p.Performance_Id == id);
                if (performance != null && performance.Performance_Photo != null)
                {
                    return File(performance.Performance_Photo, performance.Performance_PhotoMimeType);
                }
                else
                {
                    Piece piece = db.Pieces.FirstOrDefault(p => p.Piece_Id == performance.Performance_PieceId);
                    if (piece != null && piece.Piece_Photo != null)
                    {
                        return File(piece.Piece_Photo, piece.Piece_PhotoMimeType);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

    }
}