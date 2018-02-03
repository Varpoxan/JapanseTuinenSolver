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

        public string BasePath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public string JsonPuzzlesPath
        {
            get
            {
                return String.Format("{0}/Content/Puzzles/puzzles.json", BasePath);
            }
        }

        public Puzzle GetJsonPuzzles()
        {
            string str = String.Empty;
            using (StreamReader sr = new StreamReader(JsonPuzzlesPath))
            {
                str = sr.ReadToEnd();
            }
            var modelsDeserialized = JsonConvert.DeserializeObject<Puzzle>(str);
            return modelsDeserialized;
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
                if (totalPuzzleConditions.Count(s => s.Condition == icon) >= 3)
                {
                    returnValue.Add(String.Format("Het volgende symbool komt een te vaak voor: {0}", icon));
                }
                else if (totalPuzzleConditions.Count(s => s.Condition == icon) % 2 != 0)
                {
                    returnValue.Add(String.Format("Het volgende symbool komt een oneven aantal voor: {0}", icon));
                }
            }

            if (totalPuzzleConditions.Count(t =>
                t.Condition == Condition.YinYang) > 2)
            {
                returnValue.Add(String.Format("Één van de volgende symbolen komt meer dan twee keer voor: YinYang"));
            }

            if (totalPuzzleConditions.Count(t =>
                t.Condition == Condition.Pagoda) > 2)
            {
                returnValue.Add(String.Format("Één van de volgende symbolen komt meer dan twee keer voor: Pagode"));
            }

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

                        if (UsedTileList.Count > 1)
                        {
                            FillPuzzleRoads(UsedTileList);
                            if (EarlyBailOut(UsedTileList, Initiator.SimpleConditionsList))
                            {
                                //Upvote all tiles in current and deeper layers with corresponding amounts?
                            }
                        }

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
                if (puzzleVM.Name != "Puzzle ")
                {
                    var jsonPuzzles = GetJsonPuzzles();
                    var relevantPuzzle = jsonPuzzles.PuzzleList.FirstOrDefault(s => s.Name == puzzleVM.Name);
                    if (relevantPuzzle != null)
                    {
                        if (relevantPuzzle.BestSolveTime.TotalMilliseconds == 0 || solvedPuzzleVM.SolveDuration < relevantPuzzle.BestSolveTime)
                        {
                            solvedPuzzleVM.ErrorList.Add("New solve record!");
                            relevantPuzzle.BestSolveTime = solvedPuzzleVM.SolveDuration;
                            //open file stream
                            using (StreamWriter file = File.CreateText(JsonPuzzlesPath))
                            {
                                string json = JsonConvert.SerializeObject(jsonPuzzles);
                                JsonSerializer serializer = new JsonSerializer();
                                //serialize object directly into file stream
                                serializer.Serialize(file, jsonPuzzles);
                                solvedPuzzleVM.SavedPuzzleName = puzzleVM.Name;
                            }
                        }
                        else
                        {
                            solvedPuzzleVM.ErrorList.Add("Puzzle is already saved as: " + puzzleVM.Name + " !");
                        }
                    }
                }
            }

            return solvedPuzzleVM;
        }

        public bool EarlyBailOut()
        {
            FillPuzzleRoads(UsedTileList);
            
            if (DefinitivePuzzleRoads.Count > 0)
            {

            }

            return false;
        }

        public bool DoesDefinitiveRoadListSolvePuzzle(List<SimpleTileIndex> simpleConditionsList)
        {
            var icons = DoesDefinitiveRoadListSolveIcons();
            if (!icons) return false;

            var pagoda = DoesDefinitiveRoadListSolvePagoda();
            if (!pagoda) return false;

            var yinYang = DoesDefinitiveRoadListSolveYinYang();
            if (!yinYang) return false;

            var bridge = DoesDefinitiveRoadListSolveBridge();
            if (!bridge) return false;

            var tile = DoesDefinitiveRoadListSolveTile();
            if (!tile) return false;

            return icons && pagoda && yinYang && bridge && tile;
        }

        public bool DoesDefinitiveRoadListSolveIcons()
        {
            var returnValues = new HashSet<bool>();

            if (Initiator.IconConditionsToSolve.Count == 0) return true;

            foreach (var toSolve in Initiator.IconConditionsToSolve)
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

        public bool DoesDefinitiveRoadListSolvePagoda()
        {
            var returnValues = new HashSet<bool>();
            
            if (Initiator.PagodaConditionsToSolve.Count == 0) return true;

            foreach (var toSolve in Initiator.PagodaConditionsToSolve)
            {
                var findRoad = DefinitivePuzzleRoads.Any(s =>
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position) &&
                                s.SpecialConditions.Any(sc => sc.Value == toSolve.Amount &&
                                    sc.Key == toSolve.Condition));

                returnValues.Add(findRoad);
            }

            return returnValues.All(s => s);
        }

        public bool DoesDefinitiveRoadListSolveYinYang()
        {
            var returnValues = new HashSet<bool>();

            if (Initiator.YinYangConditionsToSolve.Count == 0) return true;

            foreach (var toSolve in Initiator.PagodaConditionsToSolve)
            {
                var findRoad = DefinitivePuzzleRoads.Any(s =>
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position) &&
                                s.SpecialConditions.Any(sc => sc.Value == toSolve.Amount &&
                                    sc.Key == toSolve.Condition));

                returnValues.Add(findRoad);
            }

            return returnValues.All(s => s);
        }

        public bool DoesDefinitiveRoadListSolveTile()
        {
            var returnValues = new HashSet<bool>();

            if (Initiator.TileConditionsToSolve.Count == 0) return true;

            foreach (var toSolve in Initiator.TileConditionsToSolve)
            {
                var findRoad = DefinitivePuzzleRoads.Any(s =>
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position) &&
                                s.PuzzleIndexArray.Count == toSolve.Amount);

                returnValues.Add(findRoad);
            }

            return returnValues.All(s => s);
        }

        public bool DoesDefinitiveRoadListSolveBridge()
        {
            var returnValues = new HashSet<bool>();
            
            if (Initiator.BridgeConditionsToSolve.Count == 0) return true;

            foreach (var toSolve in Initiator.BridgeConditionsToSolve)
            {
                var findRoad = DefinitivePuzzleRoads.Any(s =>
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position) &&
                                s.SpecialConditions.Any(sc => sc.Value == toSolve.Amount && 
                                    sc.Key == toSolve.Condition));

                returnValues.Add(findRoad);
            }

            return returnValues.All(s => s);
        }

        public bool IsDefinitiveRoadListPossibleForTile()
        {
            var returnValues = new HashSet<bool>();

            if (Initiator.TileConditionsToSolve.Count == 0) return true;

            foreach (var toSolve in Initiator.TileConditionsToSolve)
            {
                var findRoad = DefinitivePuzzleRoads.Any(s =>
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position) &&
                                s.PuzzleIndexArray.Count > toSolve.Amount);

                returnValues.Add(findRoad);
            }

            return returnValues.All(s => s);
        }
    }
}