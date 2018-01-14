using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JapanseTuinen.Models
{
    public class SolvedPuzzleViewModel : PuzzleViewModel
    {
        public List<string> ErrorList { get; set; }
        public List<List<Tile>> TileSet { get; set; }
        public bool Solved { get; set; }
        public int AmountOfCheckedSolutions { get; set; }
        public int AmountOfTotalSolutions { get; set; }
        public int AmountOfFoundSolutions { get; set; }
        public TimeSpan SolveDuration { get; set; }
        public List<String> TriedSolutions { get; set; }

        public SolvedPuzzleViewModel()
        {
            this.ErrorList = new List<string>();
            this.TileSet = new List<List<Tile>>();
        }
    }
}