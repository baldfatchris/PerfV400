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
    public class ProdutionController : Controller
    {
        private PerfV100Entities db = new PerfV100Entities();

        //
        // GET: /Prodution/

        public ActionResult Index()
        {
            var productions = db.Productions.Include(p => p.Piece);
            return View(productions.ToList());
        }

        //
        // GET: /Prodution/Details/5

        public ActionResult Details(int id = 0)
        {
            Production production = db.Productions.Find(id);
            if (production == null)
            {
                return HttpNotFound();
            }
            return View(production);
        }

        //
        // GET: /Prodution/Create

        public ActionResult Create()
        {
            ViewBag.Production_PieceId = new SelectList(db.Pieces, "Piece_Id", "Piece_Name");
            return View();
        }

        //
        // POST: /Prodution/Create

        [HttpPost]
        public ActionResult Create(Production production)
        {
            if (ModelState.IsValid)
            {
                db.Productions.Add(production);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Production_PieceId = new SelectList(db.Pieces, "Piece_Id", "Piece_Name", production.Production_PieceId);
            return View(production);
        }

        //
        // GET: /Prodution/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Production production = db.Productions.Find(id);
            if (production == null)
            {
                return HttpNotFound();
            }
            ViewBag.Production_PieceId = new SelectList(db.Pieces, "Piece_Id", "Piece_Name", production.Production_PieceId);
            return View(production);
        }

        //
        // POST: /Prodution/Edit/5

        [HttpPost]
        public ActionResult Edit(Production production)
        {
            if (ModelState.IsValid)
            {
                db.Entry(production).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Production_PieceId = new SelectList(db.Pieces, "Piece_Id", "Piece_Name", production.Production_PieceId);
            return View(production);
        }

        //
        // GET: /Prodution/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Production production = db.Productions.Find(id);
            if (production == null)
            {
                return HttpNotFound();
            }
            return View(production);
        }

        //
        // POST: /Prodution/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Production production = db.Productions.Find(id);
            db.Productions.Remove(production);
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