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
        //public int TileIndex { get; set; }
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
            return String.Format("PI: {0} - {1}-{2} to {3}-{4}", EndPuzzleIndex.HasValue ? EndPuzzleIndex.Value : StartPuzzleIndex, StartPosition, StartOrientation, EndPosition, EndOrientation);
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
            var roadHasUpdated = new List<bool>(2);

            if (this.EndOrientation == Orientation.Bottom && puzzleRoad.EndOrientation == Orientation.Top)
            {
                if (this.EndPosition == puzzleRoad.EndPosition + 2)
                {
                    this.EndPosition = puzzleRoad.StartPosition;
                    roadHasUpdated.Add(true);
                }
                else if (this.EndPosition == puzzleRoad.StartPosition + 2)
                {
                    this.EndPosition = puzzleRoad.EndPosition;
                    roadHasUpdated.Add(true);
                }
                else
                {

                }

                this.EndOrientation = puzzleRoad.StartOrientation;
                roadHasUpdated.Add(true);
            }
            //else if (this.EndOrientation == Orientation.Bottom && puzzleRoad.StartOrientation == Orientation.Top)
            //{

            //    this.EndOrientation = puzzleRoad.EndOrientation;
            //    roadHasUpdated.Add(true);
            //}
            else if (this.EndOrientation == Orientation.Top && puzzleRoad.StartOrientation == Orientation.Bottom)
            {
                if (this.EndPosition == puzzleRoad.StartPosition - 2)
                {
                    this.EndPosition = puzzleRoad.EndPosition;
                    roadHasUpdated.Add(true);
                }
                else if (this.EndPosition == puzzleRoad.EndPosition - 2)
                {
                    this.EndPosition = puzzleRoad.StartPosition;
                    roadHasUpdated.Add(true);
                }
                else
                {

                }
                this.EndOrientation = puzzleRoad.EndOrientation;
                roadHasUpdated.Add(true);
            }
            else if (this.EndOrientation == Orientation.Top && puzzleRoad.EndOrientation == Orientation.Bottom)
            {
                if (this.EndPosition == puzzleRoad.EndPosition - 2)
                {
                    this.EndPosition = puzzleRoad.StartPosition;
                    roadHasUpdated.Add(true);
                }
                else
                {

                }
                this.EndOrientation = puzzleRoad.StartOrientation;
                roadHasUpdated.Add(true);
            }
            else if (this.EndsAt(Orientation.Right) && puzzleRoad.StartsAt(Orientation.Left))
            {
                if (this.EndPosition == puzzleRoad.StartPosition + 1)
                {
                    this.EndPosition = puzzleRoad.EndPosition;
                    roadHasUpdated.Add(true);
                }
                else if (this.EndPosition == puzzleRoad.EndPosition + 1)
                {
                    this.EndPosition = puzzleRoad.StartPosition;
                    roadHasUpdated.Add(true);
                }
                else
                {

                }
                this.EndOrientation = puzzleRoad.EndOrientation;
                roadHasUpdated.Add(true);
            }
            else if (this.EndsAt(Orientation.Left) && puzzleRoad.EndsAt(Orientation.Right))
            {
                if (this.EndPosition == puzzleRoad.EndPosition - 1)
                {
                    this.EndPosition = puzzleRoad.StartPosition;
                    roadHasUpdated.Add(true);
                }
                else if (this.EndPosition == puzzleRoad.StartPosition - 1)
                {
                    this.EndPosition = puzzleRoad.EndPosition;
                    roadHasUpdated.Add(true);
                }
                else
                {

                }
                this.EndOrientation = puzzleRoad.StartOrientation;
                roadHasUpdated.Add(true);
            }
            else if (this.StartsAt(Orientation.Left) && puzzleRoad.EndsAt(Orientation.Right))
            {
                if (this.StartPosition == puzzleRoad.EndPosition -1)
                {
                    this.EndPosition = puzzleRoad.StartPosition;
                    roadHasUpdated.Add(true);
                }
                else if (this.StartPosition == puzzleRoad.StartPosition - 1)
                {
                    this.EndPosition = puzzleRoad.EndPosition;
                    roadHasUpdated.Add(true);
                }
                else
                {

                }
                this.EndOrientation = puzzleRoad.StartOrientation;
                roadHasUpdated.Add(true);
            }
            else if (this.StartsAt(Orientation.Right) && puzzleRoad.EndsAt(Orientation.Left))
            {
                if (this.EndPosition == puzzleRoad.EndPosition + 1)
                {
                    this.EndPosition = puzzleRoad.StartPosition;
                    roadHasUpdated.Add(true);
                }
                else if (this.StartPosition == puzzleRoad.EndPosition + 1)
                {
                    this.EndPosition = puzzleRoad.StartPosition;
                    roadHasUpdated.Add(true);
                }
                else
                {

                }
                this.EndOrientation = puzzleRoad.StartOrientation;
                roadHasUpdated.Add(true);
            }
            else if (this.EndsAt(Orientation.Right) && puzzleRoad.EndsAt(Orientation.Left))
            {
                if (this.EndPosition == puzzleRoad.EndPosition + 1)
                {
                    this.EndPosition = puzzleRoad.StartPosition;
                    roadHasUpdated.Add(true);
                }
                else
                {

                }
                this.EndOrientation = puzzleRoad.StartOrientation;
                roadHasUpdated.Add(true);
            }
            else if (this.EndsAt(Orientation.Left) && puzzleRoad.StartsAt(Orientation.Right))
            {
                if (this.EndPosition == puzzleRoad.StartPosition - 1)
                {
                    this.EndPosition = puzzleRoad.EndPosition;
                    roadHasUpdated.Add(true);
                }
                else
                {

                }
                this.EndOrientation = puzzleRoad.EndOrientation;
                roadHasUpdated.Add(true);
            }
            else if (this.StartsAt(Orientation.Right) && puzzleRoad.StartsAt(Orientation.Left))
            {
                if (this.StartPosition == puzzleRoad.StartPosition + 1)
                {
                    this.EndPosition = puzzleRoad.EndPosition;
                    roadHasUpdated.Add(true);
                }
                else
                {

                }
                this.EndOrientation = puzzleRoad.EndOrientation;
                roadHasUpdated.Add(true);
            }
            else if (this.StartsAt(Orientation.Left) && puzzleRoad.StartsAt(Orientation.Right))
            {
                if (this.StartPosition == puzzleRoad.StartPosition - 1)
                {
                    this.EndPosition = puzzleRoad.EndPosition;
                    roadHasUpdated.Add(true);
                }
                else
                {

                }
                this.EndOrientation = puzzleRoad.EndOrientation;
                roadHasUpdated.Add(true);
            }

            if (roadHasUpdated.Count < 2) //Road is NOT updated! We need to review our method!
            {

            }

            this.EndPuzzleIndex = puzzleRoad.StartPuzzleIndex;
            this.PuzzleIndexArray.Add(puzzleRoad.StartPuzzleIndex);
        }

        public bool StartsOrEndsAt(int puzzleIndex, int position, Orientation orientation)
        {
            return (this.StartPuzzleIndex == puzzleIndex && this.StartPosition == position && this.StartOrientation == orientation) ||
                (this.EndPuzzleIndex == puzzleIndex && this.EndPosition == position && this.EndOrientation == orientation);
        }

        //public bool DoesRoadConnectOnOrientation(PuzzleRoad road1, PuzzleRoad road2, params Orientation[] orientations)
        //{
        //    var returnValue = false;
        //    if (road1.EndPosition == road2.StartPosition - 2 && road1.EndOrientation == Orientation.Bottom &&
        //        road == 0 && road2.StartOrientation == Orientation.Top)
        //    {
        //        return true;
        //    }
        //    if (road1.EndPosition == 2 && road1.EndOrientation == Orientation.Bottom &&
        //        road2.EndPosition == 0 && road2.EndOrientation == Orientation.Top)
        //    {
        //        return true;
        //    }
        //    if (road1.StartPosition == 2 &&)
        //}
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
                , 1, 1, 0
                , 2, 2, 0
                , 3, 3, 270);

            if (tileString == breakieBreakie)
            {

            }

            if (newPuzzleIndex <= 3 && road.StartsOrEndsAt(Orientation.Top, Orientation.Bottom))
            {
                if (road.StartsAt(Orientation.Bottom))
                {
                    var findRpad = OpenPuzzleRoads.Where(s => newPuzzleIndex == s.StartPuzzleIndex - 3 &&
                                                                ((road.StartPosition == s.StartPosition + 2 && s.StartsAt(Orientation.Top) ||
                                                                (road.StartPosition == s.EndPosition + 2 && s.EndsAt(Orientation.Top)))));

                    if (!findRpad.Any() || findRpad.Count() > 1)
                    {
                        //Dit giet even krek mis. Teveel of te weinig roads!
                        var test = UsedTileList;
                        //return false;
                    }
                    else
                    {
                        var relevantRoad = findRpad.First();
                        if (!CheckForRoundabout(road, relevantRoad))
                        {
                            road.CombineRoad(relevantRoad);
                            ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                        }
                        return true;
                    }
                }
                if (road.EndsAt(Orientation.Bottom))
                {
                    var findRpad = OpenPuzzleRoads.Where(s => newPuzzleIndex == s.StartPuzzleIndex - 3 &&
                                                                ((road.EndPosition == s.EndPosition + 2 && s.EndsAt(Orientation.Top)) ||
                                                                (road.EndPosition == s.StartPosition + 2 && s.StartsAt(Orientation.Top))));

                    if (!findRpad.Any() || findRpad.Count() > 1)
                    {
                        //Dit giet even krek mis. Teveel of te weinig roads!
                        var test = UsedTileList;
                        //return false;
                    }
                    else
                    {
                        var relevantRoad = findRpad.First();
                        if (!CheckForRoundabout(road, relevantRoad))
                        {
                            road.CombineRoad(relevantRoad);
                            ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                        }
                        return true;
                    }
                }
            }
            if (newPuzzleIndex >= 4 && road.StartsOrEndsAt(Orientation.Top, Orientation.Bottom))
            {
                if (road.StartsAt(Orientation.Top))
                {
                    var findRpad = OpenPuzzleRoads.Where(s => newPuzzleIndex == s.StartPuzzleIndex + 3 &&
                                                                ((road.StartPosition == s.StartPosition - 2 && s.StartsAt(Orientation.Bottom) ||
                                                                (road.StartPosition == s.EndPosition - 2 && s.EndsAt(Orientation.Bottom)))));

                    if (!findRpad.Any() || findRpad.Count() > 1)
                    {
                        //Dit giet even krek mis. Teveel of te weinig roads!
                        var test = UsedTileList;
                    }
                    else
                    {
                        var relevantRoad = findRpad.First();
                        if (!CheckForRoundabout(road, relevantRoad))
                        {
                            road.CombineRoad(relevantRoad);
                            ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                        }
                        return true;
                    }
                }
                if (road.EndsAt(Orientation.Top))
                {
                    var findRpad = OpenPuzzleRoads.Where(s => newPuzzleIndex == s.StartPuzzleIndex + 3 &&
                                                                ((road.EndPosition == s.EndPosition - 2 && s.EndsAt(Orientation.Bottom)) ||
                                                                (road.EndPosition == s.StartPosition - 2 && s.StartsAt(Orientation.Bottom))));

                    if (!findRpad.Any() || findRpad.Count() > 1)
                    {
                        //Dit giet even krek mis. Teveel of te weinig roads!
                        var test = UsedTileList;
                    }
                    else
                    {
                        var relevantRoad = findRpad.First();
                        if (!CheckForRoundabout(road, relevantRoad))
                        {
                            road.CombineRoad(relevantRoad);
                            ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                        }
                        return true;
                    }
                }
            }
            if (road.StartsOrEndsAt(Orientation.Left, Orientation.Right))
            {
                //if (newPuzzleIndex == 1 || newPuzzleIndex == 4)
                {
                    if (road.StartsAt(Orientation.Right))
                    {
                        var findRpad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) - 1 &&
                                                                    ((road.StartPosition == s.EndPosition + 1 && s.EndsAt(Orientation.Left)) ||
                                                                    (road.StartPosition == s.StartPosition + 1 && s.StartsAt(Orientation.Left))));

                        if (!findRpad.Any() || findRpad.Count() > 1)
                        {
                            //Dit giet even krek mis. Teveel of te weinig roads!
                            var test = UsedTileList;
                        }
                        else
                        {
                            var relevantRoad = findRpad.First();
                            if (!CheckForRoundabout(road, relevantRoad))
                            {
                                road.CombineRoad(relevantRoad);
                                ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                            }
                            return true;
                        }
                    }
                    if (road.EndsAt(Orientation.Right))
                    {
                        var findRpad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) - 1 &&
                                                                    ((road.EndPosition == s.EndPosition + 1 && s.EndsAt(Orientation.Left)) ||
                                                                    (road.EndPosition == s.StartPosition + 1 && s.StartsAt(Orientation.Left))));

                        if (!findRpad.Any() || findRpad.Count() > 1)
                        {
                            //Dit giet even krek mis. Teveel of te weinig roads!
                            var test = UsedTileList;
                        }
                        else
                        {
                            var relevantRoad = findRpad.First();
                            if (!CheckForRoundabout(road, relevantRoad))
                            {
                                road.CombineRoad(relevantRoad);
                                ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                            }
                            return true;
                        }
                    }
                    if (road.StartsAt(Orientation.Left))
                    {
                        var findRpad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) + 1 &&
                                                                    ((road.StartPosition == s.EndPosition - 1 && s.EndsAt(Orientation.Right)) ||
                                                                    (road.StartPosition == s.StartPosition - 1 && s.StartsAt(Orientation.Right))));

                        if (!findRpad.Any() || findRpad.Count() > 1)
                        {
                            //Dit giet even krek mis. Teveel of te weinig roads!
                            var test = UsedTileList;
                        }
                        else
                        {
                            var relevantRoad = findRpad.First();
                            if (!CheckForRoundabout(road, relevantRoad))
                            {
                                road.CombineRoad(relevantRoad);
                                ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                            }
                            return true;
                        }
                    }
                    if (road.EndsAt(Orientation.Left))
                    {
                        var findRpad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) + 1 &&
                                                                    ((road.EndPosition == s.EndPosition - 1 && s.EndsAt(Orientation.Right)) ||
                                                                    (road.EndPosition == s.StartPosition - 1 && s.StartsAt(Orientation.Right))));

                        if (!findRpad.Any() || findRpad.Count() > 1)
                        {
                            //Dit giet even krek mis. Teveel of te weinig roads!
                            var test = UsedTileList;
                        }
                        else
                        {
                            var relevantRoad = findRpad.First();
                            if (!CheckForRoundabout(road, relevantRoad))
                            {
                                road.CombineRoad(relevantRoad);
                                ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                            }
                            return true;
                        }
                    }
                }
            }
            //else if (newPuzzleIndex == 3 || newPuzzleIndex == 6)
            //{
            //    if (road.StartsAt(Orientation.Left))
            //    {
            //        var findRpad = OpenPuzzleRoads.Where(s => newPuzzleIndex == s.StartPuzzleIndex + 1 &&
            //                                                    ((road.EndPosition == s.EndPosition - 2 && s.EndsAt(Orientation.Right)) ||
            //                                                    (road.EndPosition == s.StartPosition - 2 && s.StartsAt(Orientation.Right))));

            //        if (!findRpad.Any() || findRpad.Count() > 1)
            //        {
            //            //Dit giet even krek mis. Teveel of te weinig roads!
            //            var test = UsedTileList;
            //        }
            //        else
            //        {
            //            var relevantRoad = findRpad.First();
            //            if (!CheckForRoundabout(road, relevantRoad))
            //            {
            //                road.CombineRoad(relevantRoad);
            //                ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
            //            }
            //            return true;
            //        }
            //    }
            //    if (road.EndsAt(Orientation.Left))
            //    {
            //        var findRpad = OpenPuzzleRoads.Where(s => newPuzzleIndex == s.StartPuzzleIndex + 1 &&
            //                                                    ((road.EndPosition == s.EndPosition + 2 && s.EndsAt(Orientation.Right)) ||
            //                                                    (road.EndPosition == s.StartPosition + 2 && s.StartsAt(Orientation.Right))));

            //        if (!findRpad.Any() || findRpad.Count() > 1)
            //        {
            //            //Dit giet even krek mis. Teveel of te weinig roads!
            //            var test = UsedTileList;
            //        }
            //        else
            //        {
            //            var relevantRoad = findRpad.First();
            //            if (!CheckForRoundabout(road, relevantRoad))
            //            {
            //                road.CombineRoad(relevantRoad);
            //                ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
            //            }
            //            return true;
            //        }
            //    }
            //}
            else if (newPuzzleIndex == 2 || newPuzzleIndex == 5)
            {

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
                        {
                            puzzleRoad.SwitchStartToEnd();
                        }
                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Bottom);
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(2))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Right))
                        {
                            puzzleRoad.SwitchStartToEnd();
                        }
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
                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Left);
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(3))
                    {
                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Right);
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(5))
                    {
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
                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Left);
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(6))
                    {
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
                        {
                            puzzleRoad.SwitchStartToEnd();
                        }
                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Top);
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(5))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Right))
                        {
                            puzzleRoad.SwitchStartToEnd();
                        }
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
                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Left);
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(6))
                    {
                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Right);
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(2))
                    {
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
                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Top);
                    }
                    if (SubmittedPuzzleTilesIndices.Contains(5))
                    {
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
            var amountOfTriesBeforeBreaking = AmountOfTotalSolutions * AmountOfTotalSolutions;
            var breakCount = 0;
            var tries = new List<String>();

            var start = DateTime.Now;
            while (!solvedPuzzleVM.Solved && AmountOfCheckedSolutions < AmountOfTotalSolutions && breakCount <= amountOfTriesBeforeBreaking)
            {
                foreach (var tile in totalRotationTileList)
                {
                    breakCount++;
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

                    foreach (var puzzleTile in puzzleVM.PuzzleTileList.Where(s => !UsedPuzzleTilesIndices.Contains(s.Index)))
                    {
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

                        if (CheckedTileDictionary[tileKey] >= AmountOfMaximumTriesPerTile)
                        {
                            UsedPuzzleTilesIndices.Remove(tile.PuzzleIndex);
                            UsedTileList.Remove(tile);
                            usedTileDictionary[tile.TileNumber] = false;
                            tile.PuzzleIndex = -1;
                            break;
                        }

                        tile.PuzzleIndex = puzzleTile.Index;
                        UsedPuzzleTilesIndices.Add(puzzleTile.Index);

                        if (UsedTileList.Count == submittedPuzzleTileCount)
                        {
                            AmountOfCheckedSolutions++;
                            //Debug.WriteLine(String.Format("Trying to solve with: {0}", String.Join(" AND ", UsedTileList.Select(s => s.ToString()))));
                            tries.Add(String.Format("Trying to solve with: {0}", String.Join(" AND ", UsedTileList.Select(s => s.ToString()))));
                            FillPuzzleRoads(UsedTileList);
                            if (DoesDefinitiveRoadListSolvePuzzle(simpleConditionsList))
                            {
                                var newList = new List<Tile>();
                                //solvedPuzzleVM.Solved = true;
                                foreach (var ut in UsedTileList)
                                {
                                    var newTile = new Tile(ut.TileNumber, ut.Degrees);
                                    newTile.PuzzleIndex = ut.PuzzleIndex;
                                    newList.Add(newTile);
                                }
                                solvedPuzzleVM.TileSet.Add(newList);
                                //break;
                            }

                            var allKeys = UsedTileList
                                .Select(s => new UsedTileDictionaryKey(s.PuzzleIndex, s.TileNumber, s.Degrees)).ToList();

                            foreach (var key in allKeys)
                            {
                                CheckedTileDictionary[key]++;
                            }

                            //There are still other tile combinations to be checked
                            foreach (var key in allKeys)
                            {
                                if (CheckedTileDictionary[key] >= AmountOfMaximumTriesPerTile)
                                {
                                    UsedPuzzleTilesIndices.Remove(key.PuzzleIndex);
                                    var relevantTile = UsedTileList.FirstOrDefault(s =>
                                        s.TileNumber == key.TileNumber && s.Degrees == key.Degrees);
                                    UsedTileList.Remove(relevantTile);
                                    usedTileDictionary[key.TileNumber] = false;
                                    relevantTile.PuzzleIndex = -1;
                                }
                            }

                            //Only remove the last tile if there is more than one, otherwise the list gets cleared 
                            //While we are still checking combinations
                            if (UsedTileList.Any())
                            {
                                var firstTile = UsedTileList.First();
                                if (CheckedTileDictionary[new UsedTileDictionaryKey(firstTile.PuzzleIndex, firstTile.TileNumber, firstTile.Degrees)] < AmountOfMaximumTriesPerTile &&
                                    UsedTileList.Count > 1)
                                {
                                    var lastTile = UsedTileList.Last();
                                    UsedPuzzleTilesIndices.Remove(lastTile.PuzzleIndex);
                                    UsedTileList.Remove(lastTile);
                                    usedTileDictionary[lastTile.TileNumber] = false;
                                    lastTile.PuzzleIndex = -1;
                                }
                            }
                        }
                    }
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
                                s.StartsOrEndsAt(conditionOne.PuzzleIndex, conditionOne.Position, conditionOne.Orientation) &&
                                s.StartsOrEndsAt(conditionTwo.PuzzleIndex, conditionTwo.Position, conditionTwo.Orientation));

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
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position, toSolve.Orientation) &&
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
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position, toSolve.Orientation) &&
                                s.SpecialConditions.Any(sc => sc.Condition == Condition.Pagoda));

                returnValues.Add(findRoad != null);
            }

            return returnValues.All(s => s);
        }
    }
}