using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JapanseTuinen.Models
{
    public class Tile
    {
        public int PuzzleIndex { get; set; }
        public int PuzzleDepthCounter { get; set; }
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
        public int Index { get; set; }
        public List<TileInfo> TileInfoList { get; set; }

        public TileIndex()
        {
            //this.TileInfoList = new List<SpecialCondition>();
        }

        
    }

    public class TileInfo
    {
        public int Position { get; set; }
        //public SpecialCondition TileInfoList { get; set; }
        public Condition Condition { get; set; }
        public int? Amount { get; set; }

        public bool IsSvgCondition
        {
            get
            {
                return Condition.IsSvgIcon();
            }
        }

        public bool IsTileOrBridge
        {
            get
            {
                return Condition == Condition.Tile || Condition == Condition.Bridge;
            }
        }

        public string GetClass()
        {
            if (Condition.IsIconCondition())
            {
                return String.Format("condition-choice svg-icon {0}-icon", GetCondition());
            }
            else
            {
                return String.Format("condition-choice {0}-road-end", GetCondition());
            }
        }

        public string GetSrc()
        {
            return String.Format("/Content/Icons/{0}.svg", GetCondition());
        }

        public string GetCondition()
        {
            return Condition.ToString().ToLower();
        }
    }
}