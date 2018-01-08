using JapanseTuinen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JapanseTuinen.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Tiles()
        {
            ViewBag.Message = "Tegels";
            var init = new Services.Initiator();
            var tileVM = init.GetTileVM();

            return View(tileVM);
        }

        public ActionResult Puzzles()
        {
            ViewBag.Message = "Puzzels";
            var init = new Services.PuzzleService();

            var puzzleVM = init.GetPuzzleVM();

            return View(puzzleVM);
        }

        public PartialViewResult SolvePuzzle(PuzzleViewModel puzzleVM)
        {
            if (puzzleVM.PuzzleTileList.Any())
            {
                var puzzleService = new Services.PuzzleService();
                var solvedPuzzle = puzzleService.SolvePuzzle(puzzleVM);
                return PartialView(solvedPuzzle);
            }

            return PartialView();
        }
    }
}