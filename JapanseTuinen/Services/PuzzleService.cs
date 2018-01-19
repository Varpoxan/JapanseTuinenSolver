using JapanseTuinen.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
//using static JapanseTuinen.Models.PuzzleViewModel;

namespace JapanseTuinen.Services
{
    public class PuzzleRoad : Road
    {
        public int StartPuzzleIndex { get; set; }
        public int? EndPuzzleIndex { get; set; }
        public bool DefinitiveRoad { get { return EndPuzzleIndex.HasValue; } }
        public List<int> PuzzleIndexArray { get; set; }
        public bool RoadStartsOrEndsAtPuzzleEdge { get; set; }

        public List<SpecialCondition> SpecialConditions { get; set; }

        public PuzzleRoad(int startPuzzleIndex, int endPuzzleIndex, Road road)
            : base(road)
        {
            this.StartPuzzleIndex = startPuzzleIndex;
            this.EndPuzzleIndex = endPuzzleIndex;
            this.SpecialConditions = new List<SpecialCondition>();
            this.PuzzleIndexArray = new List<int>();
            this.PuzzleIndexArray = new List<int>();
        }

        public PuzzleRoad(int startPuzzleIndex, Road road)
            : base(road)
        {
            this.StartPuzzleIndex = startPuzzleIndex;
            this.SpecialConditions = new List<SpecialCondition>();
            this.PuzzleIndexArray = new List<int>() { startPuzzleIndex };
        }

        public override string ToString()
        {
            return String.Format("PI: {0} - {1} to {2}", EndPuzzleIndex.HasValue ? EndPuzzleIndex.Value : StartPuzzleIndex, StartPosition, EndPosition);
        }

        public void CombineRoad(IEnumerable<PuzzleRoad> puzzleRoads)
        {
            foreach (var pr in puzzleRoads)
            {
                CombineRoad(pr);
            }
        }

        public void CombineRoad(PuzzleRoad puzzleRoad)
        {
            if (!this.SpecialConditions.Any() && puzzleRoad.RoadAttribute != Condition.None)
            {
                this.SpecialConditions.Add(new SpecialCondition(puzzleRoad.RoadAttribute, 1));
            }

            foreach (var spCon in puzzleRoad.SpecialConditions)
            {
                var knownSpCon = this.SpecialConditions.FirstOrDefault(s => s.Condition == spCon.Condition);
                if (knownSpCon != null)
                {
                    knownSpCon.Amount += spCon.Amount;
                }
                else
                {
                    this.SpecialConditions.Add(spCon);
                }
            }
            var roadHasUpdated = false;

            if (Math.Abs(this.StartPosition - puzzleRoad.StartPosition) == 5 &&
                (this.StartPosition + puzzleRoad.StartPosition == 7 || this.StartPosition + puzzleRoad.StartPosition == 11))
            {
                this.EndPosition = puzzleRoad.EndPosition != this.StartPosition ? puzzleRoad.EndPosition : puzzleRoad.StartPosition;
                roadHasUpdated = true;
            }
            else if (Math.Abs(this.StartPosition - puzzleRoad.EndPosition) == 5 &&
                (this.StartPosition + puzzleRoad.EndPosition == 7 || this.StartPosition + puzzleRoad.EndPosition == 11))
            {
                this.EndPosition = puzzleRoad.StartPosition != this.StartPosition ? puzzleRoad.StartPosition : puzzleRoad.EndPosition;
                roadHasUpdated = true;
            }
            else if (Math.Abs(this.EndPosition - puzzleRoad.EndPosition) == 5 &&
                (this.EndPosition + puzzleRoad.EndPosition == 7 || this.EndPosition + puzzleRoad.EndPosition == 11))
            {
                this.EndPosition = puzzleRoad.StartPosition;
                roadHasUpdated = true;
            }
            else if (Math.Abs(this.EndPosition - puzzleRoad.StartPosition) == 5 &&
                (this.EndPosition + puzzleRoad.StartPosition == 7 || this.EndPosition + puzzleRoad.StartPosition == 11))
            {
                this.EndPosition = puzzleRoad.EndPosition != this.StartPosition ? puzzleRoad.EndPosition : puzzleRoad.StartPosition;
                roadHasUpdated = true;
            }
            else if (Math.Abs(this.StartPosition - puzzleRoad.StartPosition) == 3 &&
                (this.StartPosition + puzzleRoad.StartPosition == 7 || this.StartPosition + puzzleRoad.StartPosition == 11))
            {
                this.EndPosition = puzzleRoad.EndPosition;
                roadHasUpdated = true;
            }
            else if (Math.Abs(this.StartPosition - puzzleRoad.EndPosition) == 3 &&
                (this.StartPosition + puzzleRoad.EndPosition == 7 || this.StartPosition + puzzleRoad.EndPosition == 11))
            {
                this.EndPosition = puzzleRoad.StartPosition;
                roadHasUpdated = true;
            }
            else if (Math.Abs(this.EndPosition - puzzleRoad.EndPosition) == 3 &&
                (this.EndPosition + puzzleRoad.EndPosition == 7 || this.EndPosition + puzzleRoad.EndPosition == 11))
            {
                this.EndPosition = puzzleRoad.StartPosition;
                roadHasUpdated = true;
            }
            else if (Math.Abs(this.EndPosition - puzzleRoad.StartPosition) == 3 &&
                (this.EndPosition + puzzleRoad.StartPosition == 7 || this.EndPosition + puzzleRoad.StartPosition == 11))
            {
                this.EndPosition = puzzleRoad.EndPosition;
                roadHasUpdated = true;
            }

            if (!roadHasUpdated) //Road is NOT updated! We need to review our method!
            {

            }

            this.EndPuzzleIndex = puzzleRoad.StartPuzzleIndex;
            this.PuzzleIndexArray.Add(puzzleRoad.StartPuzzleIndex);
        }

        public bool StartsOrEndsAt(int puzzleIndex, int position)
        {
            return (this.StartPuzzleIndex == puzzleIndex && this.StartPosition == position) ||
                (this.EndPuzzleIndex == puzzleIndex && this.EndPosition == position);
        }
    }

    public class PuzzleService
    {
        private Initiator _initiator { get; set; }

        public Initiator Initiator
        {
            get
            {
                return _initiator ?? new Initiator();
            }
        }

        public HashSet<int> SubmittedPuzzleTilesIndices { get; set; }
        public int AmountOfCheckedSolutions { get; set; }
        public int AmountOfTotalSolutions = 1;
        public int AmountOfMaximumTriesPerTile { get; set; }
        public Dictionary<UsedTileDictionaryKey, int> CheckedTileDictionary = new Dictionary<UsedTileDictionaryKey, int>();
        private Dictionary<int, int> DynamicDepthCounter = new Dictionary<int, int>();
        private Dictionary<int, int> OriginalDepthCounter = new Dictionary<int, int>();
        private Dictionary<int, int> PuzzleIndexCounter = new Dictionary<int, int>();

        public PuzzleViewModel GetPuzzleVM()
        {
            var puzzleVM = new PuzzleViewModel();

            return puzzleVM;
        }

        public void FillCheckedTileDictionary()
        {
            foreach (var usedPIndex in SubmittedPuzzleTilesIndices)
            {
                foreach (var tile in Initiator.TileList)
                {
                    //var firstTileKey = new UsedTileDictionaryKey(usedPIndex, tile.TileNumber, 0);
                    //CheckedTileDictionary.Add(firstTileKey, 0);

                    foreach (var tileRot in tile.TotalTileRotationList)
                    {
                        var tileKey = new UsedTileDictionaryKey(usedPIndex, tile.TileNumber, tileRot.Degrees);
                        CheckedTileDictionary.Add(tileKey, 0);
                    }
                }
            }
        }

        private void FillTotalAmountOfSolutions()
        {
            for (int i = 0; i < SubmittedPuzzleTilesIndices.Count; i++)
            {
                AmountOfTotalSolutions *= (Initiator.TileList.Count - i) * 4;
            }

            var pzCount = 1;
            foreach (var pz in SubmittedPuzzleTilesIndices)
            {
                PuzzleIndexCounter.Add(pz, pzCount);
                pzCount++;
            }

            //Ok, so this should probably be calculated soon. But i need more time.
            if (SubmittedPuzzleTilesIndices.Count == 1)
            {
                DynamicDepthCounter.Add(1, 1);
                OriginalDepthCounter.Add(1, 1);
            }
            if (SubmittedPuzzleTilesIndices.Count == 2)
            {
                DynamicDepthCounter.Add(1, 24);
                DynamicDepthCounter.Add(2, 1);

                OriginalDepthCounter.Add(1, 24);
                OriginalDepthCounter.Add(2, 1);
            }
            if (SubmittedPuzzleTilesIndices.Count == 3)
            {
                DynamicDepthCounter.Add(1, 480);
                DynamicDepthCounter.Add(2, 20);
                DynamicDepthCounter.Add(3, 1);

                OriginalDepthCounter.Add(1, 480);
                OriginalDepthCounter.Add(2, 20);
                OriginalDepthCounter.Add(3, 1);
            }

            //var dicCounter = 1;
            //for (int i = SubmittedPuzzleTilesIndices.Count; i >= 0; i--)
            //{
            //    var depthCount = 1;
            //    for (int j = 0; i < SubmittedPuzzleTilesIndices.Count; j++)
            //    {
            //        depthCount *= i 
            //    }


            //    DepthCounter.Add(dicCounter, )
            //    dicCounter++;
            //}


            AmountOfMaximumTriesPerTile = AmountOfTotalSolutions / (Initiator.TileList.Count * 4);
        }

        public List<string> ValidatePuzzle(PuzzleViewModel puzzleVM)
        {
            var returnValue = new List<string>();
            var totalPuzzleConditions = puzzleVM.PuzzleTileList.SelectMany(s => s.SimpleTileIndexList);

            var IconConditions = new HashSet<Condition>() { Condition.Butterfly, Condition.Flower, Condition.Gate, Condition.Tree };

            foreach (var icon in IconConditions)
            {
                if (totalPuzzleConditions.Count(s => s.SpecialCondition.Condition == icon) >= 3)
                {
                    returnValue.Add(String.Format("Het volgende symbool komt een te vaak voor: {0}", icon));
                }
                else if (totalPuzzleConditions.Count(s => s.SpecialCondition.Condition == icon) % 2 != 0)
                {
                    returnValue.Add(String.Format("Het volgende symbool komt een oneven aantal voor: {0}", icon));
                }
            }

            if (totalPuzzleConditions.Count(t =>
                t.SpecialCondition.Condition == Condition.YinYang) > 1)
            {
                returnValue.Add(String.Format("Één van de volgende symbolen komt meer dan één keer voor: YinYang"));
            }

            return returnValue;
        }

        List<PuzzleRoad> DefinitivePuzzleRoads = new List<PuzzleRoad>();
        List<PuzzleRoad> OpenPuzzleRoads = new List<PuzzleRoad>();
        List<Tile> UsedTileList = new List<Tile>();

        public List<PuzzleRoad> FillPuzzleRoads(List<Tile> puzzleTileList)
        {
            DefinitivePuzzleRoads = new List<PuzzleRoad>();
            OpenPuzzleRoads = new List<PuzzleRoad>();

            foreach (var puzzleTile in puzzleTileList)
            {
                foreach (var road in puzzleTile.RoadList)
                {
                    var newPuzzleRoad = CreatePuzzleRoad(road, puzzleTile.PuzzleIndex);
                }
            }

            int endWhileCount = 0;
            OpenPuzzleRoads = OpenPuzzleRoads.OrderBy(s => s.RoadStartsOrEndsAtPuzzleEdge == false).ToList();
            while (OpenPuzzleRoads.Any() && endWhileCount <= 30)
            {
                //foreach (var openRoad in OpenPuzzleRoads)
                //{
                if (!FindRoadEndingOnCurrentStart(OpenPuzzleRoads[0]))
                {
                    if (OpenPuzzleRoads.Count > 1)
                        FindRoadEndingOnCurrentStart(OpenPuzzleRoads[1]);
                };
                endWhileCount++;
                if (endWhileCount >= 29)
                {
                    //Dit is niet goed...
                }
            }

            return DefinitivePuzzleRoads;
        }

        public PuzzleRoad CreatePuzzleRoad(Road road, int startPuzzleIndex)
        {
            //int? endPuzzleIndex;
            var newPuzzleRoad = new PuzzleRoad(startPuzzleIndex, road);

            var amountOfOpenRoadEnds = AmountOfOpenRoadEnds(newPuzzleRoad);

            if (amountOfOpenRoadEnds == 0)
            {
                newPuzzleRoad.EndPuzzleIndex = newPuzzleRoad.StartPuzzleIndex;
                newPuzzleRoad.SpecialConditions.Add(new SpecialCondition(newPuzzleRoad.RoadAttribute));
                DefinitivePuzzleRoads.Add(newPuzzleRoad);
            }
            else
            {
                newPuzzleRoad.RoadStartsOrEndsAtPuzzleEdge = amountOfOpenRoadEnds <= 1;
                OpenPuzzleRoads.Add(newPuzzleRoad);
            }

            return newPuzzleRoad;
        }

        public bool FindRoadEndingOnCurrentStart(PuzzleRoad road)
        {
            var newPuzzleIndex = road.EndPuzzleIndex.HasValue ? road.EndPuzzleIndex.Value : road.StartPuzzleIndex;

            var tileString = String.Join(" + ", UsedTileList.Select(s => s.ToString()));
            var breakieBreakie = String.Format("{0} - {1} - {2}° + {3} - {4} - {5}° + {6} - {7} - {8}°"
                , 1, 3, 0
                , 3, 5, 270
                , 2, 7, 270);

            var breakieBreakie2 = String.Format("{0} - {1} - {2}° + {3} - {4} - {5}°"
                , 1, 4, 270
                , 4, 5, 90);

            if (tileString == breakieBreakie)
            {
                if (road.StartPuzzleIndex == 1 && road.StartPosition == 8)
                {

                }
            }

            if (road.StartsOrEndsAt(5))
            {
                var findRoad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) - 3 &&
                                                        s.StartsOrEndsAt(2));

                if (findRoad.Count() == 1)
                {
                    var test = UsedTileList;
                    var relevantRoad = findRoad.First();
                    if (!CheckForRoundabout(road, relevantRoad))
                    {
                        road.CombineRoad(relevantRoad);
                        ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    }
                    return true;
                }

            }
            if (road.StartsOrEndsAt(6))
            {
                var findRoad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) - 3 &&
                                                        s.StartsOrEndsAt(1));

                if (findRoad.Count() == 1)
                {
                    var test = UsedTileList;
                    var relevantRoad = findRoad.First();
                    if (!CheckForRoundabout(road, relevantRoad))
                    {
                        road.CombineRoad(relevantRoad);
                        ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    }
                    return true;
                }
            }
            if (road.StartsOrEndsAt(1))
            {
                var findRoad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) + 3 &&
                                                        s.StartsOrEndsAt(6));

                if (findRoad.Count() == 1)
                {
                    var test = UsedTileList;
                    var relevantRoad = findRoad.First();
                    if (!CheckForRoundabout(road, relevantRoad))
                    {
                        road.CombineRoad(relevantRoad);
                        ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    }
                    return true;
                }
            }
            if (road.StartsOrEndsAt(2))
            {
                var findRoad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) + 3 &&
                                                        s.StartsOrEndsAt(5));

                if (findRoad.Count() == 1)
                {
                    var test = UsedTileList;
                    var relevantRoad = findRoad.First();
                    if (!CheckForRoundabout(road, relevantRoad))
                    {
                        road.CombineRoad(relevantRoad);
                        ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    }
                    return true;
                }
            }
            if (road.StartsOrEndsAt(3))
            {
                var findRoad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) - 1 &&
                                                        s.StartsOrEndsAt(8));

                if (findRoad.Count() == 1)
                {
                    var test = UsedTileList;
                    var relevantRoad = findRoad.First();
                    if (!CheckForRoundabout(road, relevantRoad))
                    {
                        road.CombineRoad(relevantRoad);
                        ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    }
                    return true;
                }
            }
            if (road.StartsOrEndsAt(4))
            {
                var findRoad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) - 1 &&
                                                        s.StartsOrEndsAt(7));

                if (findRoad.Count() == 1)
                {
                    var test = UsedTileList;
                    var relevantRoad = findRoad.First();
                    if (!CheckForRoundabout(road, relevantRoad))
                    {
                        road.CombineRoad(relevantRoad);
                        ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    }
                    return true;
                }
            }
            if (road.StartsOrEndsAt(7))
            {
                var findRoad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) + 1 &&
                                                        s.StartsOrEndsAt(4));

                if (findRoad.Count() == 1)
                {
                    var test = UsedTileList;
                    var relevantRoad = findRoad.First();
                    if (!CheckForRoundabout(road, relevantRoad))
                    {
                        road.CombineRoad(relevantRoad);
                        ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    }
                    return true;
                }
            }
            if (road.StartsOrEndsAt(8))
            {
                var findRoad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) + 1 &&
                                                        s.StartsOrEndsAt(3));

                if (findRoad.Count() == 1)
                {
                    var test = UsedTileList;
                    var relevantRoad = findRoad.First();
                    if (!CheckForRoundabout(road, relevantRoad))
                    {
                        road.CombineRoad(relevantRoad);
                        ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    }
                    return true;
                }
            }

            //Het gaat hier HELEMAAL niet goed. Er is geen weg gevonden!?
            return false;
        }

        public void ChangePuzzleRoadListsBasedOnEndings(PuzzleRoad road, PuzzleRoad relevantRoad)
        {
            var amountOfOpenRoadEnds = AmountOfOpenRoadEnds(relevantRoad);
            if (amountOfOpenRoadEnds == 2)
            {
                //The relevantroad is still open, we need to further connect roads to the road object.
                OpenPuzzleRoads.Remove(relevantRoad);
                //DefinitivePuzzleRoads.Add(road);
            }
            else if (amountOfOpenRoadEnds == 1)
            {
                //We connected 2 roads which makes 1 definitive road.
                OpenPuzzleRoads.Remove(road);
                DefinitivePuzzleRoads.Add(road);
                OpenPuzzleRoads.Remove(relevantRoad);
            }
            else if (amountOfOpenRoadEnds == 0)
            {
                //This shouldn't happen in practice.
                OpenPuzzleRoads.Remove(relevantRoad);
            }
        }

        public bool CheckForRoundabout(PuzzleRoad road, PuzzleRoad relevantRoad)
        {
            if (road.StartsAndEndsAtSameOrientation && relevantRoad.StartsAndEndsAtSameOrientation)
            {
                OpenPuzzleRoads.Remove(road);
                OpenPuzzleRoads.Remove(relevantRoad);
                return true;
            }
            return false;
        }

        public int AmountOfOpenRoadEnds(PuzzleRoad puzzleRoad)
        {
            int amount = 0;
            var usingPuzzleIndex = puzzleRoad.StartPuzzleIndex;
            if (puzzleRoad.DefinitiveEndingRoad(usingPuzzleIndex))
            {
                return 0;
            }

            switch (usingPuzzleIndex)
            {
                case 1:
                    if (!SubmittedPuzzleTilesIndices.Contains(2) && !SubmittedPuzzleTilesIndices.Contains(4))
                    {
                        return 0;
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(4))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Bottom))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Bottom);
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(2))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Right))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Right);
                    }
                    break;
                case 2:
                    if (!SubmittedPuzzleTilesIndices.Contains(1) && !SubmittedPuzzleTilesIndices.Contains(3) && !SubmittedPuzzleTilesIndices.Contains(5))
                    {
                        return 0;
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(1))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Left))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Left);
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(3))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Right))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Right);
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(5))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Bottom))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Bottom);
                    }
                    break;
                case 3:
                    if (!SubmittedPuzzleTilesIndices.Contains(2) && !SubmittedPuzzleTilesIndices.Contains(6))
                    {
                        return 0;
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(2))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Left))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Left);
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(6))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Bottom))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Bottom);
                    }
                    break;
                case 4:
                    if (!SubmittedPuzzleTilesIndices.Contains(1) && !SubmittedPuzzleTilesIndices.Contains(5))
                    {
                        return 0;
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(1))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Top))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Top);
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(5))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Right))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Right);
                    }
                    break;
                case 5:
                    if (!SubmittedPuzzleTilesIndices.Contains(4) && !SubmittedPuzzleTilesIndices.Contains(6) && !SubmittedPuzzleTilesIndices.Contains(2))
                    {
                        return 0;
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(4))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Left))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Left);
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(6))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Right))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Right);
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(2))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Top))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Top);
                    }
                    break;
                case 6:
                    if (!SubmittedPuzzleTilesIndices.Contains(3) && !SubmittedPuzzleTilesIndices.Contains(5))
                    {
                        return 0;
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(3))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Top))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Top);
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(5))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Left))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Left);
                    }
                    break;
            }

            return amount;
        }

        public SolvedPuzzleViewModel SolvePuzzle(PuzzleViewModel puzzleVM)
        {
            var solvedPuzzleVM = new SolvedPuzzleViewModel();

            solvedPuzzleVM.ErrorList = ValidatePuzzle(puzzleVM);
            if (solvedPuzzleVM.ErrorList.Any())
            {
                return solvedPuzzleVM;
            }
            SubmittedPuzzleTilesIndices = new HashSet<int>(puzzleVM.PuzzleTileList.Select(s => s.Index));
            var submittedPuzzleTileCount = SubmittedPuzzleTilesIndices.Count;

            var simpleConditionsList = puzzleVM.PuzzleTileList.SelectMany(s => s.SimpleTileIndexList).ToList();
            FillCheckedTileDictionary();
            FillTotalAmountOfSolutions();
            UsedTileList.Clear();
            var totalRotationTileList = Initiator.TileList.SelectMany(s => s.TotalTileRotationList);
            var usedTileDictionary = Initiator.TileList.ToDictionary(s => s.TileNumber, s => false);
            var UsedPuzzleTilesIndices = new HashSet<int>();
            var breakCount = 0;
            var tries = new List<String>();

            var start = DateTime.Now;
            var tileDepthDictionary = Initiator.TileList.ToDictionary(s => s.TileNumber, s => 0);

            //foreach (var puzzleTile in puzzleVM.PuzzleTileList)
            //{
            //    foreach (var run1 in totalRotationTileList)
            //    {
            //        if (usedTileDictionary[run1.TileNumber])
            //        {
            //            continue;
            //        }

            //        if (!usedTileDictionary[run1.TileNumber])
            //        {
            //            UsedTileList.Add(run1);
            //            usedTileDictionary[run1.TileNumber] = true;
            //        }

            //        foreach (var run2 in totalRotationTileList)
            //        {
            //            foreach (var run3 in totalRotationTileList)
            //            {
            //                foreach (var run4 in totalRotationTileList)
            //                {
            //                    foreach (var run5 in totalRotationTileList)
            //                    {
            //                        foreach (var run6 in totalRotationTileList)
            //                        {

            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}


            UsedTileDictionaryKey nextTileKey = null;
            
            //After the first run, the DepthCounter dictionary should probably be updated to be * 2?
            while (!solvedPuzzleVM.Solved && AmountOfCheckedSolutions < AmountOfTotalSolutions && breakCount <= AmountOfTotalSolutions)
            {
                breakCount++;
                foreach (var tile in totalRotationTileList)
                {
                    if (solvedPuzzleVM.Solved)
                    {
                        break;
                    }
                    if (usedTileDictionary[tile.TileNumber])
                    {
                        continue;
                    }

                    if (!usedTileDictionary[tile.TileNumber])
                    {
                        UsedTileList.Add(tile);
                        usedTileDictionary[tile.TileNumber] = true;
                    }

                    //var puzzleFieldCount = 0;
                    foreach (var puzzleTile in puzzleVM.PuzzleTileList.Where(s => !UsedPuzzleTilesIndices.Contains(s.Index)))
                    {
                        //puzzleFieldCount++;
                        if (nextTileKey != null)
                        {
                            if (nextTileKey.TileNumber != puzzleTile.Index || nextTileKey.TileNumber != tile.TileNumber || nextTileKey.Degrees != tile.Degrees)
                            {
                                break;
                            }
                        }

                        //There already is a tile on this puzzlefield
                        if (UsedPuzzleTilesIndices.Contains(puzzleTile.Index))
                        {
                            continue;
                        }

                        if (tile.PuzzleIndex >= 0)
                        {
                            break;
                        }
                        var tileKey = new UsedTileDictionaryKey(puzzleTile.Index, tile.TileNumber, tile.Degrees);
                        tile.PuzzleDepthCounter = PuzzleIndexCounter[puzzleTile.Index];

                        if (CheckedTileDictionary[tileKey] >= DynamicDepthCounter[tile.PuzzleDepthCounter])// AmountOfMaximumTriesPerTile)
                        {
                            UsedPuzzleTilesIndices.Remove(tile.PuzzleIndex);
                            UsedTileList.Remove(tile);
                            usedTileDictionary[tile.TileNumber] = false;
                            tile.PuzzleIndex = -1;
                            tile.PuzzleDepthCounter = 0;
                            break;
                        }

                        tile.PuzzleIndex = puzzleTile.Index;
                        UsedPuzzleTilesIndices.Add(puzzleTile.Index);

                        if (UsedTileList.Count == submittedPuzzleTileCount)
                        {
                            AmountOfCheckedSolutions++;

                            Debug.WriteLine(String.Format("Trying to solve with: {0}", String.Join(" AND ", UsedTileList.Select(s => s.ToString()))));
                            tries.Add(String.Format("Trying to solve with: {0}", String.Join(" AND ", UsedTileList.Select(s => s.ToString()))));
                            FillPuzzleRoads(UsedTileList);
                            if (DoesDefinitiveRoadListSolvePuzzle(simpleConditionsList))
                            {
                                var newList = new List<Tile>();
                                //solvedPuzzleVM.Solved = true;
                                //foreach (var ut in UsedTileList)
                                //{
                                //    var newTile = new Tile(ut.TileNumber, ut.Degrees);
                                //    newTile.PuzzleIndex = ut.PuzzleIndex;
                                //    newList.Add(newTile);
                                //}
                                //solvedPuzzleVM.TileSet.Add(newList);
                                //break;
                            }

                            var allKeys = UsedTileList
                                .Select(s => new UsedTileDictionaryKey(s.PuzzleIndex, s.TileNumber, s.Degrees)).ToList();

                            foreach (var key in allKeys)
                            {
                                CheckedTileDictionary[key]++;
                                tileDepthDictionary[key.TileNumber] = key.PuzzleIndex;
                                //depthCounter[key.PuzzleIndex] = CheckedTileDictionary[key];
                            }

                            if (AmountOfCheckedSolutions % 20 == 0)
                            {

                            }
                            //There are still other tile combinations to be checked
                            foreach (var key in allKeys.OrderByDescending(s => s.PuzzleIndex))
                            {
                                var relevantTile = UsedTileList.FirstOrDefault(s =>
                                        s.TileNumber == key.TileNumber && s.Degrees == key.Degrees);

                                if (CheckedTileDictionary[key] >= DynamicDepthCounter[relevantTile.PuzzleDepthCounter])// AmountOfMaximumTriesPerTile)
                                {
                                    UsedPuzzleTilesIndices.Remove(key.PuzzleIndex);
                                    UsedTileList.Remove(relevantTile);
                                    usedTileDictionary[key.TileNumber] = false;
                                    tileDepthDictionary[key.TileNumber] = 0;
                                    //DynamicDepthCounter[relevantTile.PuzzleDepthCounter] += OriginalDepthCounter[relevantTile.PuzzleDepthCounter];
                                    //Debug.WriteLine(String.Format("Amount of tiles with depth {0} is count {1}", 
                                    //relevantTile.PuzzleDepthCounter, CheckedTileDictionary.Count(s => s.Value >= DynamicDepthCounter[relevantTile.PuzzleDepthCounter])));
                                    //We should only += the DynamicDepthCounter in the above layers.
                                    var aboveLayers = DynamicDepthCounter.Where(s => s.Key > relevantTile.PuzzleDepthCounter).ToList();
                                    foreach (var ddc in aboveLayers)
                                    {
                                        DynamicDepthCounter[ddc.Key] += OriginalDepthCounter[ddc.Key];
                                    }
                                    relevantTile.PuzzleDepthCounter = 0;
                                    relevantTile.PuzzleIndex = -1;
                                }
                            }

                            if (UsedTileList.Count == submittedPuzzleTileCount)
                            {
                                //This means none of the tiles were removed, so we need an extra incentive to do just that.
                                var lastTile = UsedTileList.Last();
                                UsedPuzzleTilesIndices.Remove(lastTile.PuzzleIndex);
                                UsedTileList.Remove(lastTile);
                                usedTileDictionary[lastTile.TileNumber] = false;
                                tileDepthDictionary[lastTile.TileNumber] = 0;
                                lastTile.PuzzleIndex = -1;
                                lastTile.PuzzleDepthCounter = 0;
                            }

                            //if ( == totalRotationTileList.Count() / ()
                            //{
                            //var relevantKey = CheckedTileDictionary.FirstOrDefault(s => s.Value >= DynamicDepthCounter[s.Key.TileNumber]);
                            //var relevantTile = totalRotationTileList.FirstOrDefault(s => s.TileNumber == relevantKey.Key.TileNumber && s.Degrees == relevantKey.Key.Degrees);
                            //DynamicDepthCounter[relevantTile.PuzzleDepthCounter] += OriginalDepthCounter[relevantTile.PuzzleDepthCounter];
                            //}

                            //Only remove the last tile if there is more than one, otherwise the list gets cleared 
                            //While we are still checking combinations
                            //if (UsedTileList.Any())
                            //{
                            //    var firstTile = UsedTileList.First();
                            //    var usedKey = new UsedTileDictionaryKey(firstTile.PuzzleIndex, firstTile.TileNumber, firstTile.Degrees);
                            //    if (CheckedTileDictionary[usedKey] < DepthCounter[firstTile.PuzzleDepthCounter]// AmountOfMaximumTriesPerTile 
                            //        && UsedTileList.Count > 1)
                            //    {
                            //        var lastTile = UsedTileList.Last();
                            //        UsedPuzzleTilesIndices.Remove(lastTile.PuzzleIndex);
                            //        UsedTileList.Remove(lastTile);
                            //        usedTileDictionary[lastTile.TileNumber] = false;
                            //        tileDepthDictionary[lastTile.TileNumber] = 0;
                            //        lastTile.PuzzleIndex = -1;
                            //        lastTile.PuzzleDepthCounter = 0;
                            //    }
                            //}
                        }
                    }

                    //foreach (var ddc in DynamicDepthCounter)
                    //{
                        //var items = CheckedTileDictionary.Where(s => s.Key.PuzzleIndex == ddc.Key);

                        //if (items.Count(c => c.Value >= DynamicDepthCounter[ddc.Key]) >= 20)
                        //{
                            //Does this mean all tiles have more thean the Dynamic Depth Counter?
                        //}
                    //}

                    //puzzleFieldCount = 0;
                }
            }

            var end = DateTime.Now;
            solvedPuzzleVM.SolveDuration = (end - start);
            solvedPuzzleVM.AmountOfCheckedSolutions = AmountOfCheckedSolutions;
            solvedPuzzleVM.AmountOfTotalSolutions = AmountOfTotalSolutions;
            solvedPuzzleVM.AmountOfFoundSolutions = solvedPuzzleVM.TileSet.Count;
            //solvedPuzzleVM.TileSet = UsedTileList;
            solvedPuzzleVM.TriedSolutions = tries;

            return solvedPuzzleVM;
        }

        public bool DoesDefinitiveRoadListSolvePuzzle(List<SimpleTileIndex> simpleConditionsList)
        {
            var icons = DoesDefinitiveRoadListSolveIcons(simpleConditionsList);
            var pagoda = DoesDefinitiveRoadListSolvePagoda(simpleConditionsList);
            var yinYang = DoesDefinitiveRoadListSolveYinYang(simpleConditionsList);

            return icons && pagoda && yinYang;
        }

        public bool DoesDefinitiveRoadListSolveIcons(List<SimpleTileIndex> simpleConditionsList)
        {
            var returnValues = new HashSet<bool>();
            var iconConditionsToSolve = simpleConditionsList.Where(s =>
                    s.SpecialCondition.Condition.IsIconCondition())
                    .GroupBy(s => s.SpecialCondition.Condition);

            if (!iconConditionsToSolve.Any()) return true;

            foreach (var toSolve in iconConditionsToSolve)
            {
                var conditionOne = toSolve.First();
                var conditionTwo = toSolve.Last();

                var findRoad = DefinitivePuzzleRoads.FirstOrDefault(s =>
                                s.StartsOrEndsAt(conditionOne.PuzzleIndex, conditionOne.Position) &&
                                s.StartsOrEndsAt(conditionTwo.PuzzleIndex, conditionTwo.Position));

                returnValues.Add(findRoad != null);
            }

            return returnValues.All(s => s);
        }

        public bool DoesDefinitiveRoadListSolvePagoda(List<SimpleTileIndex> simpleConditionsList)
        {
            var returnValues = new HashSet<bool>();

            var conditionsToSolve = simpleConditionsList.Where(s =>
                    s.SpecialCondition.Condition == Condition.Pagoda);

            if (!conditionsToSolve.Any()) return true;

            foreach (var toSolve in conditionsToSolve)
            {
                var findRoad = DefinitivePuzzleRoads.FirstOrDefault(s =>
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position) &&
                                s.SpecialConditions.Any(sc => sc.Condition == Condition.Pagoda));

                returnValues.Add(findRoad != null);
            }

            return returnValues.All(s => s);
        }

        public bool DoesDefinitiveRoadListSolveYinYang(List<SimpleTileIndex> simpleConditionsList)
        {
            var returnValues = new HashSet<bool>();

            var conditionsToSolve = simpleConditionsList.Where(s =>
                    s.SpecialCondition.Condition == Condition.YinYang);

            if (!conditionsToSolve.Any()) return true;

            foreach (var toSolve in conditionsToSolve)
            {
                var findRoad = DefinitivePuzzleRoads.FirstOrDefault(s =>
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position) &&
                                s.SpecialConditions.Any(sc => sc.Condition == Condition.Pagoda));

                returnValues.Add(findRoad != null);
            }

            return returnValues.All(s => s);
        }
    }
}