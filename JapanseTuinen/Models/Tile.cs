using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JapanseTuinen.Models
{
    public class Tile
    {
        public int PuzzleIndex { get; set; }
        public int TileNumber { get; set; }
        public List<Tile> TotalTileRotationList { get; set; }
        public int Degrees { get; set; }
        public List<Road> RoadList { get; set; }

        public Tile(int tileNumber, int degrees)
        {
            this.TileNumber = tileNumber;
            this.PuzzleIndex = -1;
            this.Degrees = degrees;
            this.TotalTileRotationList = new List<Tile>(4);
            this.RoadList = new List<Road>();
        }

        public Tile GetRotation(int degrees)
        {
            return this.TotalTileRotationList.FirstOrDefault(s => s.Degrees == degrees);
        }

        public override string ToString()
        {
            return String.Format("{0} - {1} - {2}°", this.PuzzleIndex, this.TileNumber, this.Degrees);
        }

        public string ToSimpleString()
        {
            var degree = "A";
            if (this.Degrees == 90)
                degree = "B";
            if (this.Degrees == 180)
                degree = "C";
            if (this.Degrees == 270)
                degree = "D";
            return String.Format("{0}{1}{2}", this.PuzzleIndex, this.TileNumber, degree);
        }
    }

    public class SimpleTileIndex
    {
        public int PuzzleIndex { get; set; }
        public int Position { get; set; }
        public SpecialCondition SpecialCondition { get; set; }

        public SimpleTileIndex(int pIndex, int pos, SpecialCondition spCon)
        {
            this.PuzzleIndex = pIndex;
            this.Position = pos;
            this.SpecialCondition = spCon;
        }
    }

    public class TileIndex
    {
        public int Position { get; set; }
        public List<TileInfo> TileInfoList { get; set; }

        public TileIndex()
        {
            this.TileInfoList = new List<TileInfo>(2);
        }

        public class TileInfo
        {
            public SpecialCondition SpecialCondition { get; set; }
        }
    }
}