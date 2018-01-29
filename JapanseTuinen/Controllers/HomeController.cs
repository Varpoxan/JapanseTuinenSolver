﻿using JapanseTuinen.Models;
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
            var jsonModels = init.GetJsonPuzzles();
            puzzleVM.KnownPuzzles = new HashSet<string>(jsonModels.PuzzleList.Select(s => s.Name));

            return View(puzzleVM);
        }

        public PartialViewResult LoadPuzzle(string puzzleName)
        {
            var init = new Services.PuzzleService();
            var jsonModels = init.GetJsonPuzzles();
            var relevantPuzzle = jsonModels.PuzzleList.FirstOrDefault(s => s.Name == puzzleName);

            return PartialView(relevantPuzzle);
        }

        public /*JsonResult*/ PartialViewResult SolvePuzzle(PuzzleViewModel puzzleVM)
        {
            if (puzzleVM.TileIndexList.Any())
            {
                var puzzleService = new Services.PuzzleService();
                var solvedPuzzle = puzzleService.SolvePuzzle(puzzleVM);
                //return new JsonResult() { Data = solvedPuzzle };
                return PartialView(solvedPuzzle);
            }

            //return new JsonResult(); 
            return PartialView();
        }
    }
}