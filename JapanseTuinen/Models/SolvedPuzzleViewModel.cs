using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JapanseTuinen.Models
{
    public class SolvedPuzzleViewModel : PuzzleViewModel
    {
        public List<string> ErrorList { get; set; }
        public Dictionary<int, Tile> TileSet { get; set; }
        public bool Solved { get; set; }
        public int AmountOfCheckedSolutions { get; set; }
        public int AmountOfTotalSolutions { get; set; }
        public TimeSpan SolveDuration { get; set; }

        public SolvedPuzzleViewModel()
        {
            this.ErrorList = new List<string>();
            this.TileSet = new Dictionary<int, Tile>();
        }
    }
}