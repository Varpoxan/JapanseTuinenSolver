using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JapanseTuinen.Models
{
    public class TileViewModel
    {
        public List<Tile> TileList { get; set; }

        public TileViewModel()
        {
            this.TileList = new List<Tile>();
        }
    }
}