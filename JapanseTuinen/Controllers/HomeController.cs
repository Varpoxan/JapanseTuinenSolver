using JapanseTuinen.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
            var path = AppDomain.CurrentDomain.BaseDirectory;
            string str = (new StreamReader(String.Format("{0}/Content/Puzzles/puzzles.json", path))).ReadToEnd();
            var modelsDeserialized = JsonConvert.DeserializeObject<PuzzleViewModel>(str);
            puzzleVM.KnownPuzzles = new HashSet<string>(modelsDeserialized.TileIndexList.Select(s => s.Name));

            return View(puzzleVM);
        }

        public PartialViewResult LoadPuzzle(string puzzleName)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            string str = (new StreamReader(String.Format("{0}/Content/Puzzles/puzzles.json", path))).ReadToEnd();
            var modelsDeserialized = JsonConvert.DeserializeObject<PuzzleViewModel>(str);
            var modelsDeserialized2 = JsonConvert.DeserializeObject<PuzzleTile>(str);
            var relevantPuzzle = modelsDeserialized.TileIndexList.FirstOrDefault(s => s.Name == puzzleName);

            return PartialView(relevantPuzzle);
        }

        public PartialViewResult SolvePuzzle(PuzzleViewModel puzzleVM)
        {
            if (puzzleVM.TileIndexList.Any())
            {
                var puzzleService = new Services.PuzzleService();
                var solvedPuzzle = puzzleService.SolvePuzzle(puzzleVM);
                return PartialView(solvedPuzzle);
            }

            return PartialView();
        }
    }
}