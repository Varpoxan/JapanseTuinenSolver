using JapanseTuinen.Models;
using System;
using System.Collections.Generic;
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
            return String.Format("PI: {0} - {1}-{2} to {3}-{4}", StartPuzzleIndex, StartPosition, StartOrientation, EndPosition, EndOrientation);
        }

        public void CombineRoad(PuzzleRoad puzzleRoad)
        {
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
            if (this.EndPosition == puzzleRoad.EndPosition + 2)
            {
                this.EndPosition = puzzleRoad.StartPosition;
                roadHasUpdated = true;
            }
            else if (this.EndPosition == puzzleRoad.StartPosition + 2)
            {
                this.EndPosition = puzzleRoad.EndPosition;
                roadHasUpdated = true;
            }
            else if (this.StartPosition == puzzleRoad.EndPosition - 2)
            {
                this.EndPosition = puzzleRoad.EndPosition;
                roadHasUpdated = true;
            }
            else if (this.StartPosition == puzzleRoad.StartPosition + 2)
            {
                this.EndPosition = puzzleRoad.EndPosition;
                roadHasUpdated = true;
            }
            else if (this.StartPosition == puzzleRoad.StartPosition - 2)
            {
                this.EndPosition = puzzleRoad.EndPosition;
                roadHasUpdated = true;
            }
            

            if (this.EndOrientation == Orientation.Bottom && puzzleRoad.EndOrientation == Orientation.Top)
            {
                this.EndOrientation = puzzleRoad.StartOrientation;
                roadHasUpdated = true;
            }
            else if (this.EndOrientation == Orientation.Bottom && puzzleRoad.StartOrientation == Orientation.Top)
            {
                this.EndOrientation = puzzleRoad.EndOrientation;
                roadHasUpdated = true;
            }

            if (!roadHasUpdated) //Road is NOT updated! We need to review our method!
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

        public HashSet<int> UsedPuzzleTilesIndices { get; set; }
        public int AmountOfCheckedSolutions { get; set; }
        public int AmountOfTotalSolutions = 1;
        public Dictionary<UsedTileDictionaryKey, int> CheckedTileDictionary = new Dictionary<UsedTileDictionaryKey, int>();

        public PuzzleViewModel GetPuzzleVM()
        {
            var puzzleVM = new PuzzleViewModel();

            return puzzleVM;
        }

        public void FillCheckedTileDictionary()
        {
            foreach (var usedPIndex in UsedPuzzleTilesIndices)
            {
                foreach (var tile in Initiator.TileList)
                {
                    var firstTileKey = new UsedTileDictionaryKey(usedPIndex, tile.TileNumber, 0);
                    CheckedTileDictionary.Add(firstTileKey, 0);

                    foreach (var tileRot in tile.TileRotationList)
                    {
                        var tileKey = new UsedTileDictionaryKey(usedPIndex, tile.TileNumber, tileRot.Degrees);
                        CheckedTileDictionary.Add(tileKey, 0);
                    }
                }
            }
        }

        private void FillTotalAmountOfSolutions()
        {
            for (int i = 0; i < UsedPuzzleTilesIndices.Count; i++)
            {
                AmountOfTotalSolutions *= (Initiator.TileList.Count - i) * 4;
            }
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


            while (OpenPuzzleRoads.Any())
            {
                //foreach (var openRoad in OpenPuzzleRoads)
                //{
                var road = FindRoadEndingOnCurrentStart(OpenPuzzleRoads[0]);
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
                DefinitivePuzzleRoads.Add(newPuzzleRoad);
            }
            else
            {
                OpenPuzzleRoads.Add(newPuzzleRoad);
            }

            return newPuzzleRoad;
        }

        public PuzzleRoad FindRoadEndingOnCurrentStart(PuzzleRoad road)
        {
            if (road.StartsOrEndsAt(Orientation.Bottom))
            {
                var newPuzzleIndex = road.EndPuzzleIndex.HasValue ? road.EndPuzzleIndex.Value : road.StartPuzzleIndex;
                var findRpad = OpenPuzzleRoads.Where(s => (newPuzzleIndex == s.StartPuzzleIndex - 3 || newPuzzleIndex == s.StartPuzzleIndex + 3) &&
                                                            ((road.EndPosition == s.EndPosition + 2 && s.EndsAt(Orientation.Top)) ||
                                                            (road.EndPosition == s.StartPosition + 2 && s.StartsAt(Orientation.Top)) ||
                                                            (road.StartPosition == s.StartPosition + 2 && s.StartsAt(Orientation.Top)) ||
                                                            (road.StartPosition == s.EndPosition + 2 && s.EndsAt(Orientation.Top))));

                if (!findRpad.Any() || findRpad.Count() > 1)
                {
                    //Dit giet even krek mis. Teveel of te weinig roads!
                    var test = UsedTileList;
                }
                else
                {
                    var relevantRoad = findRpad.First();
                    road.CombineRoad(relevantRoad);

                    ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    return road;
                }
            }
            if (road.StartsOrEndsAt(Orientation.Top))
            {
                var newPuzzleIndex = road.EndPuzzleIndex.HasValue ? road.EndPuzzleIndex.Value : road.StartPuzzleIndex;
                var findRpad = OpenPuzzleRoads.Where(s => (newPuzzleIndex == s.StartPuzzleIndex - 3 || newPuzzleIndex == s.StartPuzzleIndex + 3) &&
                                                            ((road.EndPosition == s.EndPosition - 2 && s.EndsAt(Orientation.Bottom)) ||
                                                            (road.EndPosition == s.StartPosition - 2 && s.StartsAt(Orientation.Bottom)) ||
                                                            (road.StartPosition == s.StartPosition - 2 && s.StartsAt(Orientation.Bottom)) ||
                                                            (road.StartPosition == s.EndPosition - 2 && s.EndsAt(Orientation.Bottom))));

                if (!findRpad.Any() || findRpad.Count() > 1)
                {
                    //Dit giet even krek mis. Teveel of te weinig roads!
                    var test = UsedTileList;
                }
                else
                {
                    var relevantRoad = findRpad.First();
                    road.CombineRoad(relevantRoad);
                    ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    return road;
                }
            }

            //Het gaat hier HELEMAAL niet goed. Er is geen weg gevonden!?
            return null;
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

        public int AmountOfOpenRoadEnds(PuzzleRoad puzzleRoad)
        {
            int amount = 0;
            //var puzzleIndices = new List<int>() { puzzleRoad.StartPuzzleIndex };
            if (puzzleRoad.EndPuzzleIndex.HasValue)
            {
                //puzzleIndices.Add(puzzleRoad.EndPuzzleIndex.Value);
            }
            var usingPuzzleIndex = puzzleRoad.StartPuzzleIndex;
            //foreach (var usingPuzzleIndex in puzzleIndices)
            {
                if (puzzleRoad.DefinitiveEndingRoad(usingPuzzleIndex))
                {
                    return 0;
                }

                switch (usingPuzzleIndex)
                {
                    case 1:
                        if (!UsedPuzzleTilesIndices.Contains(2) && !UsedPuzzleTilesIndices.Contains(4))
                        {
                            return 0;
                        }
                        if (UsedPuzzleTilesIndices.Contains(4))
                        {
                            if (puzzleRoad.StartsAt(Orientation.Bottom))
                            {
                                puzzleRoad.SwitchStartToEnd();
                                //amount++;
                            }
                            amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Bottom);
                        }
                        if (UsedPuzzleTilesIndices.Contains(2))
                        {
                            if (puzzleRoad.StartsAt(Orientation.Right))
                            {
                                puzzleRoad.SwitchStartToEnd();
                                //amount++;
                            }
                            //if (puzzleRoad.EndsAt(Orientation.Right))
                            //{
                                //amount++;
                            //}
                            amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Right);
                        }
                        //if (!UsedPuzzleTilesIndices.Contains(2) && )
                        //{
                        //    if (!puzzleRoad.StartsOrEndsAt(Orientation.Right))
                        //    {
                        //        return 2;
                        //    }
                        //}

                        //if (!UsedPuzzleTilesIndices.Contains(4))
                        //{
                        //    if (!puzzleRoad.StartsOrEndsAt(Orientation.Bottom))
                        //    {
                        //        return 2;
                        //    }
                        //}
                        break;
                    case 2:
                        if (!UsedPuzzleTilesIndices.Contains(1) && !UsedPuzzleTilesIndices.Contains(3) && !UsedPuzzleTilesIndices.Contains(5))
                        {
                            return 2;
                        }
                        if (!UsedPuzzleTilesIndices.Contains(1))
                        {
                            if ((puzzleRoad.EndPosition == 0 && puzzleRoad.EndOrientation == Orientation.Left) ||
                                (puzzleRoad.EndPosition == 2 && puzzleRoad.EndOrientation == Orientation.Left))
                            {
                                amount++;
                            }
                        }
                        if (!UsedPuzzleTilesIndices.Contains(3))
                        {
                            if ((puzzleRoad.EndPosition == 1 && puzzleRoad.EndOrientation == Orientation.Right) ||
                                (puzzleRoad.EndPosition == 3 && puzzleRoad.EndOrientation == Orientation.Right))
                            {
                                amount++;
                            }
                        }
                        if (!UsedPuzzleTilesIndices.Contains(5))
                        {
                            if ((puzzleRoad.EndPosition == 2 && puzzleRoad.EndOrientation == Orientation.Bottom) ||
                                (puzzleRoad.EndPosition == 3 && puzzleRoad.EndOrientation == Orientation.Bottom))
                            {
                                amount++;
                            }
                        }
                        break;
                    case 3:
                        if (!UsedPuzzleTilesIndices.Contains(2) && !UsedPuzzleTilesIndices.Contains(6))
                        {
                            return 2;
                        }
                        if (!UsedPuzzleTilesIndices.Contains(2))
                        {
                            if ((puzzleRoad.EndPosition == 0 && puzzleRoad.EndOrientation == Orientation.Left) ||
                                (puzzleRoad.EndPosition == 2 && puzzleRoad.EndOrientation == Orientation.Left))
                            {
                                amount++;
                            }
                        }
                        if (!UsedPuzzleTilesIndices.Contains(6))
                        {
                            if ((puzzleRoad.EndPosition == 2 && puzzleRoad.EndOrientation == Orientation.Bottom) ||
                                (puzzleRoad.EndPosition == 3 && puzzleRoad.EndOrientation == Orientation.Bottom))
                            {
                                amount++;
                            }
                        }
                        break;
                    case 4:
                        if (!UsedPuzzleTilesIndices.Contains(1) && !UsedPuzzleTilesIndices.Contains(5))
                        {
                            return 2;
                        }
                        if (UsedPuzzleTilesIndices.Contains(1))
                        {
                            if (puzzleRoad.StartsAt(Orientation.Top))
                            {
                                puzzleRoad.SwitchStartToEnd();
                                //amount++;
                            }
                            //if (puzzleRoad.EndsAt(Orientation.Top))
                            //{
                                //amount++;
                            //}
                            amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Top);
                        }
                        if (UsedPuzzleTilesIndices.Contains(5))
                        {
                            if (puzzleRoad.StartsAt(Orientation.Right))
                            {
                                puzzleRoad.SwitchStartToEnd();
                                //amount++;
                            }
                            //if (puzzleRoad.EndsAt(Orientation.Right))
                            //{
                            //amount++;
                            //}
                            amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Right);
                        }
                        //if (!UsedPuzzleTilesIndices.Contains(1))
                        //{
                        //    if (puzzleRoad.StartsOrEndsAt(Orientation.Top))
                        //    {
                        //        amount++;
                        //    }
                        //}
                        //if (!UsedPuzzleTilesIndices.Contains(5))
                        //{
                        //    if (puzzleRoad.StartsOrEndsAt(Orientation.Right))
                        //    {
                        //        amount++;
                        //    }
                        //}
                        break;
                    case 5:
                        if (!UsedPuzzleTilesIndices.Contains(4) && !UsedPuzzleTilesIndices.Contains(6) && !UsedPuzzleTilesIndices.Contains(2))
                        {
                            return 2;
                        }
                        if (!UsedPuzzleTilesIndices.Contains(4))
                        {
                            if ((puzzleRoad.EndPosition == 0 && puzzleRoad.EndOrientation == Orientation.Left) ||
                                (puzzleRoad.EndPosition == 2 && puzzleRoad.EndOrientation == Orientation.Left))
                            {
                                return 2;
                            }
                        }
                        if (!UsedPuzzleTilesIndices.Contains(6))
                        {
                            if ((puzzleRoad.EndPosition == 1 && puzzleRoad.EndOrientation == Orientation.Right) ||
                                (puzzleRoad.EndPosition == 3 && puzzleRoad.EndOrientation == Orientation.Right))
                            {
                                return 2;
                            }
                        }
                        if (!UsedPuzzleTilesIndices.Contains(2))
                        {
                            if ((puzzleRoad.EndPosition == 0 && puzzleRoad.EndOrientation == Orientation.Top) ||
                                (puzzleRoad.EndPosition == 1 && puzzleRoad.EndOrientation == Orientation.Top))
                            {
                                return 2;
                            }
                        }
                        break;
                    case 6:
                        if (!UsedPuzzleTilesIndices.Contains(3) && !UsedPuzzleTilesIndices.Contains(5))
                        {
                            return 2;
                        }
                        if (!UsedPuzzleTilesIndices.Contains(3))
                        {
                            if ((puzzleRoad.EndPosition == 0 && puzzleRoad.EndOrientation == Orientation.Top) ||
                                (puzzleRoad.EndPosition == 1 && puzzleRoad.EndOrientation == Orientation.Top))
                            {
                                return 2;
                            }
                        }
                        if (!UsedPuzzleTilesIndices.Contains(5))
                        {
                            if ((puzzleRoad.EndPosition == 0 && puzzleRoad.EndOrientation == Orientation.Left) ||
                                (puzzleRoad.EndPosition == 2 && puzzleRoad.EndOrientation == Orientation.Left))
                            {
                                return 2;
                            }
                        }
                        break;
                }
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
            UsedPuzzleTilesIndices = new HashSet<int>(puzzleVM.PuzzleTileList.Select(s => s.Index));
            var simpleConditionsList = puzzleVM.PuzzleTileList.SelectMany(s => s.SimpleTileIndexList).ToList();
            FillCheckedTileDictionary();
            FillTotalAmountOfSolutions();
            //usedTileList = new List<Tile>();

            var start = DateTime.Now;
            while (!solvedPuzzleVM.Solved && AmountOfCheckedSolutions <= AmountOfTotalSolutions)
            {
                foreach (var tile in Initiator.TileList)
                {
                    if (UsedTileList.Count == puzzleVM.PuzzleTileList.Count)
                    {
                        AmountOfCheckedSolutions++;
                        FillPuzzleRoads(UsedTileList);
                        if (DoesDefinitiveRoadListSolvePuzzle(simpleConditionsList))
                        {
                            solvedPuzzleVM.Solved = true;
                            break;
                        }
                        else
                        {
                            UsedTileList.Clear();
                        }
                        //break;
                    }

                    var tileIsUsed = false;

                    var tempTileRotationList = new List<Tile> { tile };
                    tempTileRotationList.AddRange(tile.TileRotationList);

                    foreach (var rotatedTile in tempTileRotationList)
                    {
                        if (UsedTileList.Count == puzzleVM.PuzzleTileList.Count)
                        {
                            FillPuzzleRoads(UsedTileList);
                            if (DoesDefinitiveRoadListSolvePuzzle(simpleConditionsList))
                            {
                                solvedPuzzleVM.Solved = true;
                                break;
                            }
                        }

                        foreach (var puzzleTile in puzzleVM.PuzzleTileList)
                        {
                            if (UsedTileList.Any(s => s.PuzzleIndex == puzzleTile.Index))
                            {
                                continue;
                            }
                            if (UsedTileList.Count == puzzleVM.PuzzleTileList.Count)
                            {
                                FillPuzzleRoads(UsedTileList);
                                if (DoesDefinitiveRoadListSolvePuzzle(simpleConditionsList))
                                {
                                    solvedPuzzleVM.Solved = true;
                                    break;
                                }
                            }

                            rotatedTile.PuzzleIndex = puzzleTile.Index;
                            UsedTileList.Add(rotatedTile);
                            var tileKey = new UsedTileDictionaryKey(puzzleTile.Index, rotatedTile.TileNumber, rotatedTile.Degrees);
                            CheckedTileDictionary[tileKey]++;
                            tileIsUsed = true;
                            break;
                        }
                        if (tileIsUsed)
                        {
                            break;
                        }
                    }

                    if (tileIsUsed)
                    {
                        continue;
                    }
                }
            }

            var end = DateTime.Now;
            solvedPuzzleVM.SolveDuration = (end - start);
            solvedPuzzleVM.AmountOfCheckedSolutions = AmountOfCheckedSolutions;
            solvedPuzzleVM.AmountOfTotalSolutions = AmountOfTotalSolutions;
            solvedPuzzleVM.TileSet = UsedTileList.ToDictionary(s => s.PuzzleIndex);

            return solvedPuzzleVM;
        }

        public bool DoesDefinitiveRoadListSolvePuzzle(List<SimpleTileIndex> simpleConditionsList)
        {
            var iconConditionsToSolve = simpleConditionsList.Where(s =>
                    s.SpecialCondition.Condition.IsIconCondition())
                    .GroupBy(s => s.SpecialCondition.Condition);

            //if (!iconConditionsToSolve.Any()) return true;

            foreach (var toSolve in iconConditionsToSolve)
            {
                var conditionOne = toSolve.First();
                var conditionTwo = toSolve.Last();

                var findRoad = DefinitivePuzzleRoads.FirstOrDefault(s =>
                                s.StartsOrEndsAt(conditionOne.PuzzleIndex, conditionOne.Position, conditionOne.Orientation) &&
                                s.StartsOrEndsAt(conditionTwo.PuzzleIndex, conditionTwo.Position, conditionTwo.Orientation));

                if (findRoad != null)
                {
                    return true;
                }

                foreach (var road in DefinitivePuzzleRoads)
                {
                    //if (conditionOne.po)
                }
            }

            return false;
        }
    }
}