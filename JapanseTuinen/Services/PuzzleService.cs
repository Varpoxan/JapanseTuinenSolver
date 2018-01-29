using JapanseTuinen.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JapanseTuinen.Services
{
    public partial class PuzzleService
    {
        private Initiator _initiator { get; set; }

        public Initiator Initiator
        {
            get
            {
                if (_initiator == null)
                {
                    _initiator = new Initiator();
                }
                return _initiator;
            }
        }


        public PuzzleViewModel GetPuzzleVM()
        {
            var puzzleVM = new PuzzleViewModel();

            return puzzleVM;
        }

        private int AmountOfCheckedSolutions { get { return Initiator.AmountOfCheckedSolutions; } set { Initiator.AmountOfCheckedSolutions = value; } }
        private int AmountOfTotalSolutions { get { return Initiator.AmountOfTotalSolutions; } }
        private int AmountOfMaximumTriesPerTile { get { return Initiator.AmountOfMaximumTriesPerTile; } }
        private Dictionary<int, int> PuzzleIndexCounter { get { return Initiator.PuzzleIndexCounter; } }
        private Dictionary<int, int> DepthToIndex { get { return Initiator.DepthToIndex; } }
        private Dictionary<int, int> OriginalDepthCounter { get { return Initiator.OriginalDepthCounter; } }
        private Dictionary<UsedTileDictionaryKey, int> CheckedTileDictionary { get { return Initiator.CheckedTileDictionary; } }
        private Dictionary<UsedTileDictionaryKey, int> DynamicCheckedTileDictionary { get { return Initiator.DynamicCheckedTileDictionary; } }
        private int SubmittedPuzzleTileCount { get { return Initiator.SubmittedPuzzleTileCount; } }
        private List<Tile> TileList { get { return Initiator.TileList; } }

        public List<string> ValidatePuzzle(PuzzleViewModel puzzleVM)
        {
            var returnValue = new List<string>();
            var totalPuzzleConditions = puzzleVM.TileIndexList.SelectMany(s => s.SimpleTileIndexList);

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

            //if (totalPuzzleConditions.Count(t =>
            //    t.SpecialCondition.Condition == Condition.YinYang) > 1)
            //{
            //    returnValue.Add(String.Format("Één van de volgende symbolen komt meer dan één keer voor: YinYang"));
            //}

            return returnValue;
        }

        List<PuzzleRoad> DefinitivePuzzleRoads = new List<PuzzleRoad>();
        List<PuzzleRoad> OpenPuzzleRoads = new List<PuzzleRoad>();
        List<Tile> UsedTileList = new List<Tile>();

        public SolvedPuzzleViewModel SolvePuzzle(PuzzleViewModel puzzleVM)
        {
            var solvedPuzzleVM = new SolvedPuzzleViewModel();

            solvedPuzzleVM.ErrorList = ValidatePuzzle(puzzleVM);
            if (solvedPuzzleVM.ErrorList.Any())
            {
                return solvedPuzzleVM;
            }
            Initiator.Initialize(puzzleVM);
            UsedTileList.Clear();
            var usedTileDictionary = TileList.ToDictionary(s => s.TileNumber, s => false);
            var UsedPuzzleTilesIndices = new HashSet<int>();

            var breakCount = 0;
            var tries = new List<String>();

            var start = DateTime.Now;

            //After the first run, the DepthCounter dictionary should probably be updated to be * 2?
            while (!solvedPuzzleVM.Solved && AmountOfCheckedSolutions < AmountOfTotalSolutions && breakCount <= AmountOfTotalSolutions)
            {
                breakCount++;
                foreach (var tile in Initiator.TotalRotationTileList)
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
                    foreach (var puzzleTile in puzzleVM.TileIndexList.Where(s => !UsedPuzzleTilesIndices.Contains(s.Index)))
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
                        tile.PuzzleDepthCounter = PuzzleIndexCounter[puzzleTile.Index];
                        var tileKey = new UsedTileDictionaryKey(puzzleTile.Index, tile.TileNumber, tile.Degrees);

                        if (CheckedTileDictionary[tileKey] >= DynamicCheckedTileDictionary[tileKey])
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

                        if (UsedTileList.Count == SubmittedPuzzleTileCount)
                        {
                            AmountOfCheckedSolutions++;
                            //var solveString = String.Format("Trying to solve with: {0}", String.Join(" AND ", UsedTileList.OrderBy(s => s.PuzzleIndex).Select(s => s.ToString())));
                            //tries.Add(solveString);

                            FillPuzzleRoads(UsedTileList);
                            if (DoesDefinitiveRoadListSolvePuzzle(Initiator.SimpleConditionsList))
                            {
                                var newList = new List<Tile>();
                                //solvedPuzzleVM.Solved = true;
                                if (solvedPuzzleVM.TileSet.Count <= 2)
                                {
                                    foreach (var ut in UsedTileList)
                                    {
                                        var newTile = new Tile(ut.TileNumber, ut.Degrees);
                                        newTile.PuzzleIndex = ut.PuzzleIndex;
                                        newList.Add(newTile);
                                    }

                                    solvedPuzzleVM.TileSet.Add(newList);
                                }
                            }

                            var allKeys = UsedTileList
                                .Select(s => new UsedTileDictionaryKey(s.PuzzleIndex, s.TileNumber, s.Degrees)).ToList();

                            foreach (var key in allKeys)
                            {
                                Initiator.CheckedTileDictionary[key]++;
                            }

                            //There are still other tile combinations to be checked
                            foreach (var key in allKeys.OrderByDescending(s => s.PuzzleIndex))
                            {
                                var relevantTile = UsedTileList.FirstOrDefault(s =>
                                        s.TileNumber == key.TileNumber && s.Degrees == key.Degrees);

                                //See if one of the used keys (starting at the end) has been used too much
                                if (Initiator.CheckedTileDictionary[key] >= DynamicCheckedTileDictionary[key])
                                {
                                    UsedPuzzleTilesIndices.Remove(key.PuzzleIndex);
                                    UsedTileList.Remove(relevantTile);
                                    usedTileDictionary[key.TileNumber] = false;

                                    //We should only += the DynamicDepthCounter in the above layers.
                                    var aboveLayer = OriginalDepthCounter.FirstOrDefault(s => s.Key > relevantTile.PuzzleDepthCounter);
                                    if (aboveLayer.Key != 0)
                                    {
                                        var usedTileNumbersAboveThisLayer = allKeys.Where(s => s.PuzzleIndex < DepthToIndex[aboveLayer.Key]).Select(a => a.TileNumber).ToList();
                                        var relevantIndex = DepthToIndex[aboveLayer.Key];
                                        var everyKeyExceptUsedOnes = DynamicCheckedTileDictionary.Where(s =>
                                        s.Key.PuzzleIndex == relevantIndex &&
                                        !usedTileNumbersAboveThisLayer.Contains(s.Key.TileNumber)).ToList();

                                        foreach (var keyEx in everyKeyExceptUsedOnes)
                                        {
                                            //Update the count for the next round
                                            DynamicCheckedTileDictionary[keyEx.Key] += OriginalDepthCounter[aboveLayer.Key];
                                        }
                                    }

                                    relevantTile.PuzzleDepthCounter = 0;
                                    relevantTile.PuzzleIndex = -1;
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
            solvedPuzzleVM.TriedSolutions = tries;
            var nonMax = Initiator.CheckedTileDictionary.Where(s => s.Value < AmountOfMaximumTriesPerTile).ToList();
            var nonCheckedAmount = nonMax.Sum(s => AmountOfMaximumTriesPerTile - s.Value);

            if (solvedPuzzleVM.AmountOfFoundSolutions == 1)
            {
                //Valid puzzle, lets write to JSON
                if (puzzleVM.PuzzleNumber > 0)
                {
                    var path = AppDomain.CurrentDomain.BaseDirectory;
                    string str = (new StreamReader(String.Format("{0}/Content/Puzzles/puzzles.json", path))).ReadToEnd();

                    // deserializes string into object
                    var modelsDeserialized = JsonConvert.DeserializeObject<Models.JSON.PuzzleSet>(str);
                    var modelsDeserialized2 = JsonConvert.DeserializeObject<PuzzleViewModel>(str);
                    if (modelsDeserialized.puzzles.Any(s => s.name == "Puzzle " + puzzleVM.PuzzleNumber))
                    {
                        solvedPuzzleVM.ErrorList.Add("Puzzle is already saved as: Puzzle" + puzzleVM.PuzzleNumber + " !");
                    }
                    else
                    {

                    }
                }
            }

            return solvedPuzzleVM;
        }

        public bool DoesDefinitiveRoadListSolvePuzzle(List<SimpleTileIndex> simpleConditionsList)
        {
            var icons = DoesDefinitiveRoadListSolveIcons(simpleConditionsList);
            if (!icons) return false;

            var pagoda = DoesDefinitiveRoadListSolveGivenCondition(simpleConditionsList, Condition.Pagoda);
            if (!pagoda) return false;

            var yinYang = DoesDefinitiveRoadListSolveGivenCondition(simpleConditionsList, Condition.YinYang);
            if (!yinYang) return false;

            var bridge = DoesDefinitiveRoadListSolveBridge(simpleConditionsList, Condition.Bridge);
            if (!bridge) return false;

            var tile = DoesDefinitiveRoadListSolveTile(simpleConditionsList, Condition.Tile);
            if (!tile) return false;

            return icons && pagoda && yinYang && bridge && tile;
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

                var findRoad = DefinitivePuzzleRoads.Any(s =>
                                s.StartsOrEndsAt(conditionOne.PuzzleIndex, conditionOne.Position) &&
                                s.StartsOrEndsAt(conditionTwo.PuzzleIndex, conditionTwo.Position));

                returnValues.Add(findRoad);
            }

            return returnValues.All(s => s);
        }

        public bool DoesDefinitiveRoadListSolveTile(List<SimpleTileIndex> simpleConditionsList, Condition condition)
        {
            var returnValues = new HashSet<bool>();

            var conditionsToSolve = simpleConditionsList.Where(s =>
                    s.SpecialCondition.Condition == condition);

            if (!conditionsToSolve.Any()) return true;

            foreach (var toSolve in conditionsToSolve)
            {
                var findRoad = DefinitivePuzzleRoads.Any(s =>
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position) &&
                                s.PuzzleIndexArray.Count == toSolve.SpecialCondition.Amount);

                returnValues.Add(findRoad);
            }

            return returnValues.All(s => s);
        }

        public bool DoesDefinitiveRoadListSolveBridge(List<SimpleTileIndex> simpleConditionsList, Condition condition)
        {
            var returnValues = new HashSet<bool>();

            var conditionsToSolve = simpleConditionsList.Where(s =>
                    s.SpecialCondition.Condition == condition);

            if (!conditionsToSolve.Any()) return true;

            foreach (var toSolve in conditionsToSolve)
            {
                var findRoad = DefinitivePuzzleRoads.Any(s =>
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position) &&
                                s.SpecialConditions.Any(sc => sc.Value == toSolve.SpecialCondition.Amount && 
                                    sc.Key == toSolve.SpecialCondition.Condition));

                returnValues.Add(findRoad);
            }

            return returnValues.All(s => s);
        }

        public bool DoesDefinitiveRoadListSolveGivenCondition(List<SimpleTileIndex> simpleConditionsList, Condition condition)
        {
            var returnValues = new HashSet<bool>();

            var conditionsToSolve = simpleConditionsList.Where(s =>
                    s.SpecialCondition.Condition == condition);

            if (!conditionsToSolve.Any()) return true;
            
            foreach (var toSolve in conditionsToSolve)
            {
                var findRoad = DefinitivePuzzleRoads.Any(s =>
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position) &&
                                s.SpecialConditions.Any(sc => sc.Key == condition));

                returnValues.Add(findRoad);
            }

            return returnValues.All(s => s);
        }
    }
}