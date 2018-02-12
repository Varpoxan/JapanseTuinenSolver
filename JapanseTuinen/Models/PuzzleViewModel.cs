using Newtonsoft.Json;
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

    public class Puzzle
    {
        public List<PuzzleViewModel> PuzzleList { get; set; }
    }

    public class PuzzleViewModel : HtmlPuzzleHelper
    {
        public string Name { get; set; }
        public TimeSpan BestSolveTime { get; set; }
        public List<PuzzleTile> TileIndexList { get; set; }

        [JsonIgnore]
        public bool UseBailout { get; set; }

        [JsonIgnore]
        public HashSet<string> KnownPuzzles { get; set; }

        public PuzzleViewModel()
        {
            this.TileIndexList = new List<PuzzleTile>();
        }
    }

    public class PuzzleTile
    {
        public int Index { get; set; }
        public List<TileInfo> TileInfoList { get; set; }

        public PuzzleTile()
        {
            this.TileInfoList = new List<TileInfo>();
        }

        [JsonIgnore]
        private List<SimpleTileIndex> _simpleTileIndex { get; set; }

        [JsonIgnore]
        public List<SimpleTileIndex> SimpleTileIndexList
        {
            get
            {
                if (_simpleTileIndex == null)
                {
                    var returnList = new List<SimpleTileIndex>();

                    foreach (var tileInfo in TileInfoList)
                    {
                        returnList.Add(new SimpleTileIndex(this.Index, tileInfo.Position, tileInfo.Condition, tileInfo.Amount));
                    }

                    _simpleTileIndex = returnList;
                }

                return _simpleTileIndex;
            }
        }
    }
}