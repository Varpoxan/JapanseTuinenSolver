using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JapanseTuinen.Models
{
    public class UsedTileDictionaryKey
    {
        public int PuzzleIndex { get; set; }
        public int TileNumber { get; set; }
        public int Degrees { get; set; }

        public UsedTileDictionaryKey(int pIndex, int tNumber, int degrees)
        {
            this.PuzzleIndex = pIndex;
            this.TileNumber = tNumber;
            this.Degrees = degrees;
        }

        public override bool Equals(object UTDK)
        {
            var newObj = (UsedTileDictionaryKey)UTDK;
            return this.PuzzleIndex == newObj.PuzzleIndex && this.TileNumber == newObj.TileNumber && this.Degrees == newObj.Degrees;
        }

        public override int GetHashCode()
        {
            return this.PuzzleIndex.GetHashCode() + this.TileNumber.GetHashCode() + this.Degrees.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("PI: {0} - TNumber: {1} - TRotation: {2}", this.PuzzleIndex, this.TileNumber, this.Degrees);
        }
    }

    public class PuzzleViewModel
    {
        public List<PuzzleTile> PuzzleTileList { get; set; }

        public PuzzleViewModel()
        {
            this.PuzzleTileList = new List<PuzzleTile>();
        }

        public class PuzzleTile
        {
            public int Index { get; set; }
            public int PuzzleTileCounter { get; set; }
            public List<TileIndex> TileIndexList { get; set; }

            public PuzzleTile()
            {
                this.TileIndexList = new List<TileIndex>();
            }

            public List<SimpleTileIndex> SimpleTileIndexList
            {
                get
                {
                    var returnList = new List<SimpleTileIndex>();

                    foreach (var tileIndex in TileIndexList)
                    {

                        foreach (var tileInfo in tileIndex.TileInfoList)
                        {
                            returnList.Add(new SimpleTileIndex(this.Index, tileIndex.Position, tileInfo.SpecialCondition));
                        }
                    }

                    return returnList;
                }
            }
        }

        #region HtmlHelpers
        public int GetRowNumberByIndex(int index)
        {
            if (index <= 3)
            {
                return 1;
            }
            else if (index >= 4 && index <= 6)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        public int GetColumnNumberByIndex(int index)
        {
            if (index == 1 || index == 4 || index == 7)
            {
                return 1;
            }
            else if (index == 2 || index == 5 || index == 8)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        public string GetAbsLeftByIndex(int index)
        {
            var row = GetRowNumberByIndex(index);
            var col = GetColumnNumberByIndex(index);
            if (col > 1)
            {
                return String.Format("{0}px", (col - 1) * 200);
            }
            else
            {
                return "0";
            }
        }

        public string GetAbsTopByIndex(int index)
        {
            var row = GetRowNumberByIndex(index);
            var col = GetColumnNumberByIndex(index);
            if (row > 1)
            {
                return String.Format("{0}px", (row - 1) * 200);
            }
            else
            {
                return "0";
            }
        }

        public string GetRowClass(int index)
        {
            if (index <= 3)
            {
                return "row-one";
            }
            else if (index >= 4 && index <= 6)
            {
                return "row-two";
            }
            else
            {
                return "row-three";
            }
        }

        public string GetColumnClass(int index)
        {
            if (index == 1 || index == 4 || index == 7)
            {
                return "column-one";
            }
            else if (index == 2 || index == 5 || index == 8)
            {
                return "column-two";
            }
            else
            {
                return "column-three";
            }
        }
        #endregion
    }
}