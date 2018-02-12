using JapanseTuinen.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        Dictionary<int, bool> UsedTileDictionary { get; set; }
        HashSet<int> UsedPuzzleTilesIndices = new HashSet<int>();

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
            UsedTileDictionary = TileList.ToDictionary(s => s.TileNumber, s => false);

            var breakCount = 0;
            var tries = new List<String>();

            var start = DateTime.Now;

            //After the first run, the DepthCounter dictionary should probably be updated to be * 2?
            //while (!solvedPuzzleVM.Solved && CheckedTileDictionary.Values.All(s => s == AmountOfMaximumTriesPerTile))
            while (!solvedPuzzleVM.Solved && AmountOfCheckedSolutions < AmountOfTotalSolutions && breakCount <= AmountOfTotalSolutions)
            {
                breakCount++;
                foreach (var tile in Initiator.TotalRotationTileList)
                {
                    if (solvedPuzzleVM.Solved)
                    {
                        break;
                    }
                    if (UsedTileDictionary[tile.TileNumber])
                    {
                        continue;
                    }

                    if (!UsedTileDictionary[tile.TileNumber])
                    {
                        UsedTileList.Add(tile);
                        UsedTileDictionary[tile.TileNumber] = true;
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

                        //var usedTileNumbers = new HashSet<int>(UsedTileList.Select(a => a.TileNumber));
                        //var tilesInSameLayer = CheckedTileDictionary.Keys.Where(s => s.PuzzleIndex == tileKey.PuzzleIndex && !usedTileNumbers.Contains(s.TileNumber));
                        //var lessAmount = tilesInSameLayer.Any(a => CheckedTileDictionary[a] < DynamicCheckedTileDictionary[tileKey]);

                        if (CheckedTileDictionary[tileKey] >= DynamicCheckedTileDictionary[tileKey])
                        {
                            RemoveAndResetPuzzleTile(tile);
                            //UsedPuzzleTilesIndices.Remove(tile.PuzzleIndex);
                            //UsedTileList.Remove(tile);
                            //UsedTileDictionary[tile.TileNumber] = false;
                            //tile.PuzzleIndex = -1;
                            //tile.PuzzleDepthCounter = 0;
                            break;
                        }

                        tile.PuzzleIndex = puzzleTile.Index;
                        UsedPuzzleTilesIndices.Add(puzzleTile.Index);

                        if (puzzleVM.UseBailout)
                        {
                            if (UsedTileList.Count >= 1 && UsedTileList.Count < SubmittedPuzzleTileCount)
                            {
                                if (DoEarlyBailOut())
                                {
                                    break;
                                }
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



                            //var allKeys = UsedTileList
                            //    .Select(s => new UsedTileDictionaryKey(s.PuzzleIndex, s.TileNumber, s.Degrees)).ToList();

                            //foreach (var key in allKeys)
                            //{
                            //    Initiator.CheckedTileDictionary[key]++;
                            //}

                            ////There are still other tile combinations to be checked
                            //foreach (var key in allKeys.OrderByDescending(s => s.PuzzleIndex))
                            //{
                            //    var relevantTile = UsedTileList.FirstOrDefault(s =>
                            //            s.TileNumber == key.TileNumber && s.Degrees == key.Degrees);

                            //    //See if one of the used keys (starting at the end) has been used too much
                            //    if (Initiator.CheckedTileDictionary[key] >= DynamicCheckedTileDictionary[key])
                            //    {
                            //        //UsedPuzzleTilesIndices.Remove(key.PuzzleIndex);
                            //        //UsedTileList.Remove(relevantTile);
                            //        //UsedTileDictionary[key.TileNumber] = false;

                            //        //We should only += the DynamicDepthCounter in the above layers.
                            //        var aboveLayer = OriginalDepthCounter.FirstOrDefault(s => s.Key > relevantTile.PuzzleDepthCounter);
                            //        if (aboveLayer.Key != 0)
                            //        {
                            //            var usedTileNumbersAboveThisLayer = allKeys.Where(s => s.PuzzleIndex < DepthToIndex[aboveLayer.Key]).Select(a => a.TileNumber).ToList();
                            //            var relevantIndex = DepthToIndex[aboveLayer.Key];
                            //            var everyKeyExceptUsedOnes = DynamicCheckedTileDictionary.Where(s =>
                            //            s.Key.PuzzleIndex == relevantIndex &&
                            //            !usedTileNumbersAboveThisLayer.Contains(s.Key.TileNumber)).ToList();

                            //            foreach (var keyEx in everyKeyExceptUsedOnes)
                            //            {
                            //                //Update the count for the next round
                            //                DynamicCheckedTileDictionary[keyEx.Key] += OriginalDepthCounter[aboveLayer.Key];
                            //            }
                            //        }

                            //        RemoveAndResetPuzzleTile(relevantTile);

                            //        //relevantTile.PuzzleDepthCounter = 0;
                            //        //relevantTile.PuzzleIndex = -1;
                            //    }
                            //}
                        }

                        CheckTileCountAndUpIfNeeded(false);
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

        private void CheckTileCountAndUpIfNeeded(bool didBailOut)
        {
            var usedTileNumbers = UsedTileList.Select(s => s.TileNumber);
            var allKeys = UsedTileList.Select(s => new UsedTileDictionaryKey(s.PuzzleIndex, s.TileNumber, s.Degrees)).ToList();
            //var otherTileKeys = Initiator.TotalRotationTileList.Where(s => !usedTileNumbers.Contains(s.TileNumber));
            var uppedAboveTiles = false;

            if (AmountOfCheckedSolutions % 480 == 0)
            {

            }

            if (!didBailOut)
            {

            }

            if (didBailOut)
            {
                //if (!uppedAboveTiles)
                {
                    var lastTile = UsedTileList.OrderBy(s => s.PuzzleIndex).LastOrDefault();
                    var keysAboveCurrent = allKeys.Where(s => PuzzleIndexCounter[s.PuzzleIndex] <= lastTile.PuzzleDepthCounter);
                    var relevantLayer = PuzzleIndexCounter[lastTile.PuzzleIndex];
                    var oriDepthAmount = OriginalDepthCounter[relevantLayer];
                    if (keysAboveCurrent.Any())
                    {
                        foreach (var keyAbove in keysAboveCurrent)
                        {
                            if (CheckedTileDictionary[keyAbove] + oriDepthAmount <= AmountOfMaximumTriesPerTile)
                            {
                                CheckedTileDictionary[keyAbove] += oriDepthAmount;
                            }
                        }
                        uppedAboveTiles = true;
                    }


                    //var usedTileKeysBelowThisLayer = CheckedTileDictionary.Keys.Where(s => s.PuzzleIndex > key.PuzzleIndex && !usedTileNumbers.Contains(s.TileNumber));
                    var usedTileKeysBelowThisLayer = DynamicCheckedTileDictionary.Keys.Where(s =>
                            s.PuzzleIndex > lastTile.PuzzleIndex &&
                            !usedTileNumbers.Contains(s.TileNumber)).ToList();
                    var firstLayerAfterBailOut = relevantLayer + 1;
                    foreach (var otKey in usedTileKeysBelowThisLayer)
                    {
                        var otKeyLayer = PuzzleIndexCounter[otKey.PuzzleIndex];
                        if (DynamicCheckedTileDictionary[otKey] + OriginalDepthCounter[firstLayerAfterBailOut] <= AmountOfMaximumTriesPerTile)
                        {
                            DynamicCheckedTileDictionary[otKey] += OriginalDepthCounter[firstLayerAfterBailOut];
                        }
                        else
                        {
                            Debug.WriteLine(String.Format("We bailout out, but did NOT up the below layer! {0}", otKey.ToString()));
                            //Lets fill it up towards max
                            DynamicCheckedTileDictionary[otKey] = AmountOfMaximumTriesPerTile;
                        }
                        CheckedTileDictionary[otKey] += OriginalDepthCounter[firstLayerAfterBailOut];
                    }

                    var keysAboveCurrentlayer = allKeys.Where(s => PuzzleIndexCounter[s.PuzzleIndex] != lastTile.PuzzleDepthCounter);
                    var tileNumbersAboveCurrentLayer = keysAboveCurrentlayer.Select(s => s.TileNumber);
                    var tilesInSameLayer = CheckedTileDictionary.Keys.Where(s => s.PuzzleIndex == lastTile.PuzzleIndex && !tileNumbersAboveCurrentLayer.Contains(s.TileNumber));
                    var key = new UsedTileDictionaryKey(lastTile.PuzzleIndex, lastTile.TileNumber, lastTile.Degrees);
                    var allSameAmount = tilesInSameLayer.All(a => CheckedTileDictionary[a] == CheckedTileDictionary[key]);

                    if (allSameAmount)
                    {
                        foreach (var tsl in tilesInSameLayer)
                        {
                            if (DynamicCheckedTileDictionary[tsl] + oriDepthAmount <= AmountOfMaximumTriesPerTile)
                            {
                                DynamicCheckedTileDictionary[tsl] += oriDepthAmount;
                            }
                        }
                    }

                    foreach (var kkey in keysAboveCurrent)
                    {
                        var rTile = UsedTileList.FirstOrDefault(s => s.PuzzleIndex == kkey.PuzzleIndex && s.TileNumber == kkey.TileNumber && s.Degrees == kkey.Degrees);
                        //See if one of the used keys (starting at the end) has been used too much
                        if (Initiator.CheckedTileDictionary[kkey] >= DynamicCheckedTileDictionary[kkey])
                        {
                            RemoveAndResetPuzzleTile(rTile);
                            //break;
                        }
                    }
                    //RemoveAndResetPuzzleTile(lastTile);
                }
                return;
            }

            foreach (var key in allKeys.OrderByDescending(s => s.PuzzleIndex))
            {
                var relevantTile = UsedTileList.FirstOrDefault(s =>
                    s.TileNumber == key.TileNumber && s.Degrees == key.Degrees);
                var relevantLayer = PuzzleIndexCounter[key.PuzzleIndex];
                var oriDepthAmount = OriginalDepthCounter[relevantLayer];
                var dynamicKeyCount = DynamicCheckedTileDictionary[key];
                
                //else
                {
                    //Only up if there is an actual checked solution
                    if (allKeys.Count == SubmittedPuzzleTileCount)
                    {
                        if (CheckedTileDictionary[key] + 1 <= dynamicKeyCount)
                        {
                            CheckedTileDictionary[key]++;
                        }

                        //if (CheckedTileDictionary[key] > 0)
                        //{
                        //    var keysAboveCurrentlayer = allKeys.Where(s => PuzzleIndexCounter[s.PuzzleIndex] != relevantTile.PuzzleDepthCounter);
                        //    var tileNumbersAboveCurrentLayer = keysAboveCurrentlayer.Select(s => s.TileNumber);
                        //    var tilesInSameLayer = CheckedTileDictionary.Keys.Where(s => s.PuzzleIndex == key.PuzzleIndex && !tileNumbersAboveCurrentLayer.Contains(s.TileNumber));
                        //    var allSameAmount = tilesInSameLayer.All(a => CheckedTileDictionary[a] == CheckedTileDictionary[key]);

                        //    if (allSameAmount)
                        //    {
                        //        if (DynamicCheckedTileDictionary[key] + oriDepthAmount <= AmountOfMaximumTriesPerTile)
                        //        {
                        //            foreach (var otKey in tilesInSameLayer)
                        //            {
                        //                DynamicCheckedTileDictionary[new UsedTileDictionaryKey(key.PuzzleIndex, otKey.TileNumber, otKey.Degrees)] += oriDepthAmount;
                        //            }
                        //        }
                        //    }
                        //}

                        //foreach (var akey in allKeys.OrderByDescending(s => s.PuzzleIndex))
                        //{
                        //var relevantTile = UsedTileList.FirstOrDefault(s =>
                        //s.TileNumber == akey.TileNumber && s.Degrees == akey.Degrees);

                        //See if one of the used keys (starting at the end) has been used too much
                        if (Initiator.CheckedTileDictionary[key] >= DynamicCheckedTileDictionary[key])
                        {
                            //UsedPuzzleTilesIndices.Remove(key.PuzzleIndex);
                            //UsedTileList.Remove(relevantTile);
                            //UsedTileDictionary[key.TileNumber] = false;

                            //var keysAboveCurrentlayer = allKeys.Where(s => PuzzleIndexCounter[s.PuzzleIndex] != relevantTile.PuzzleDepthCounter);
                            //var tileNumbersAboveCurrentLayer = keysAboveCurrentlayer.Select(s => s.TileNumber);
                            //var tilesInSameLayer = CheckedTileDictionary.Keys.Where(s => s.PuzzleIndex == key.PuzzleIndex && !tileNumbersAboveCurrentLayer.Contains(s.TileNumber));

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

                            RemoveAndResetPuzzleTile(relevantTile);

                            //relevantTile.PuzzleDepthCounter = 0;
                            //relevantTile.PuzzleIndex = -1;
                        }
                        //}
                    }
                }



                //See if one of the used keys (starting at the end) has been used too much
                if (Initiator.CheckedTileDictionary[key] >= dynamicKeyCount && didBailOut)
                {
                    RemoveAndResetPuzzleTile(relevantTile);
                    break;
                }
            }
        }

        public bool EarlyBailOut()
        {
            var tile = IsDefinitiveRoadListPossibleForTile();
            if (!tile) return true;

            var bridge = IsDefinitiveRoadListPossibleForBridge();
            if (!bridge) return true;

            var icons = IsDefinitiveRoadListPossibleForIcons();
            if (!icons) return true;

            var pagoda = IsDefinitiveRoadListPossibleForPagoda();
            if (!pagoda) return true;

            var yinYang = IsDefinitiveRoadListPossibleForYinYang();
            if (!yinYang) return true;

            return false;
        }

        public bool DoEarlyBailOut()
        {
            var bailOut = false;

            FillPuzzleRoads(UsedTileList);

            if (DefinitivePuzzleRoads.Count > 0)
            {
                bailOut = EarlyBailOut();
            }

            if (bailOut)
            {
                var bailOutString = String.Format("Bailing out at: {0}", String.Join(" AND ", UsedTileList.Select(s => s.ToString())));
                Debug.WriteLine(bailOutString);
                //Upvote all tiles in current and deeper layers with corresponding amounts?
                var allKeys = UsedTileList/*.Where(s => s.PuzzleDepthCounter >= UsedTileList.Count)*/
                    .Select(s => new UsedTileDictionaryKey(s.PuzzleIndex, s.TileNumber, s.Degrees)).ToList();

                var x = "bla";
                CheckTileCountAndUpIfNeeded(true);
                return true;
                //There is still a problem when the first bailOut is the second tile, we add += original layer.
                //The next time the 2nd tile is checked again, the amount is upped by += layer, and it will check again
                foreach (var key in allKeys.OrderByDescending(s => s.PuzzleIndex))
                {
                    var relevantTile = UsedTileList.FirstOrDefault(s =>
                        s.TileNumber == key.TileNumber && s.Degrees == key.Degrees);
                    var relevantLayer = PuzzleIndexCounter[key.PuzzleIndex];

                    if (Initiator.CheckedTileDictionary[key] >= OriginalDepthCounter[relevantLayer])
                    {
                        return false;
                    }

                    if (relevantTile.PuzzleDepthCounter >= allKeys.Count)
                    {
                        Initiator.CheckedTileDictionary[key] += OriginalDepthCounter[relevantLayer];
                        if (DynamicCheckedTileDictionary[key] < AmountOfMaximumTriesPerTile)
                        {
                            DynamicCheckedTileDictionary[key] += OriginalDepthCounter[relevantLayer];
                        }

                        //var aboveLayer = OriginalDepthCounter.FirstOrDefault(s => s.Key > relevantTile.PuzzleDepthCounter);
                        var usedTileNumbersAboveThisLayer = allKeys.Where(s => s.PuzzleIndex <= DepthToIndex[relevantLayer]).Select(a => a.TileNumber).ToList();
                        var relevantIndex = DepthToIndex[relevantLayer];
                        var everyKeyExceptUsedOnes = DynamicCheckedTileDictionary.Where(s =>
                            s.Key.PuzzleIndex > relevantIndex &&
                            !usedTileNumbersAboveThisLayer.Contains(s.Key.TileNumber)).ToList();

                        foreach (var keyEx in everyKeyExceptUsedOnes)
                        {
                            //Update the count for the next round
                            var layer = PuzzleIndexCounter[keyEx.Key.PuzzleIndex];
                            Initiator.CheckedTileDictionary[keyEx.Key] += OriginalDepthCounter[layer];
                            DynamicCheckedTileDictionary[keyEx.Key] += OriginalDepthCounter[layer];
                        }

                        RemoveAndResetPuzzleTile(relevantTile);
                        //UsedTileList.Remove(relevantTile);
                        //UsedPuzzleTilesIndices.Remove(key.PuzzleIndex);
                        //relevantTile.PuzzleDepthCounter = 0;
                        //relevantTile.PuzzleIndex = -1;
                        //UsedTileDictionary[key.TileNumber] = false;
                    }
                    else
                    {
                        //var layer = PuzzleIndexCounter[key.PuzzleIndex];
                        Initiator.CheckedTileDictionary[key] += OriginalDepthCounter[relevantLayer + 1];

                        if (Initiator.CheckedTileDictionary[key] >= DynamicCheckedTileDictionary[key])
                        {
                            //var relevantTile = UsedTileList.FirstOrDefault(s =>
                            //s.TileNumber == notKey.TileNumber && s.Degrees == notKey.Degrees);
                            RemoveAndResetPuzzleTile(relevantTile);
                            //UsedTileList.Remove(relevantTile);
                            //UsedPuzzleTilesIndices.Remove(key.PuzzleIndex);
                            //relevantTile.PuzzleDepthCounter = 0;
                            //relevantTile.PuzzleIndex = -1;
                            //UsedTileDictionary[key.TileNumber] = false;
                        }
                    }
                }

                //We bailed out, clear list and break
                //var allNotKeys = UsedTileList.Where(s => s.PuzzleDepthCounter <= UsedTileList.Count)
                //.Select(s => new UsedTileDictionaryKey(s.PuzzleIndex, s.TileNumber, s.Degrees)).ToList();
                //foreach (var notKey in allNotKeys)
                //{

                //}

                return true;
            }

            return false;
        }

        private void RemoveAndResetPuzzleTile(Tile puzzleTile)
        {
            UsedTileList.Remove(puzzleTile);
            UsedPuzzleTilesIndices.Remove(puzzleTile.PuzzleIndex);
            puzzleTile.PuzzleDepthCounter = 0;
            puzzleTile.PuzzleIndex = -1;
            UsedTileDictionary[puzzleTile.TileNumber] = false;
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
                                s.SpecialConditions.Any(sc => sc.Key == toSolve.Condition));

                returnValues.Add(findRoad);
            }

            return returnValues.All(s => s);
        }

        public bool DoesDefinitiveRoadListSolveYinYang()
        {
            var returnValues = new HashSet<bool>();

            if (Initiator.YinYangConditionsToSolve.Count == 0) return true;

            foreach (var toSolve in Initiator.YinYangConditionsToSolve)
            {
                var findRoad = DefinitivePuzzleRoads.Any(s =>
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position) &&
                                s.SpecialConditions.Any(sc => sc.Key == toSolve.Condition));

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

        #region Possibilities
        public bool IsDefinitiveRoadListPossibleForTile()
        {
            var returnValues = new HashSet<bool>();

            if (Initiator.TileConditionsToSolve.Count == 0) return true;

            foreach (var toSolve in Initiator.TileConditionsToSolve)
            {
                var findRoad = DefinitivePuzzleRoads.Any(s =>
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position) &&
                                s.PuzzleIndexArray.Count < toSolve.Amount);

                if (findRoad)
                {
                    return false;
                    //toSolve.SolveState = SolveState.Invalid;
                }
                //returnValues.Add(findRoad);
            }

            return true;
        }

        public bool IsDefinitiveRoadListPossibleForBridge()
        {
            var returnValues = new HashSet<bool>();

            if (Initiator.BridgeConditionsToSolve.Count == 0) return true;

            foreach (var toSolve in Initiator.BridgeConditionsToSolve)
            {
                var findRoad = DefinitivePuzzleRoads.Any(s =>
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position) &&
                                (s.SpecialConditions.Any(sc => sc.Value < toSolve.Amount &&
                                    sc.Key == toSolve.Condition) ||
                                !s.SpecialConditions.Any(sc => sc.Key == toSolve.Condition)));

                if (findRoad)
                {
                    return false;
                    //toSolve.SolveState = SolveState.Invalid;
                }
                //returnValues.Add(findRoad);
            }

            return true;
        }

        public bool IsDefinitiveRoadListPossibleForIcons()
        {
            var returnValues = new HashSet<bool>();

            if (Initiator.IconConditionsToSolve.Count == 0) return true;

            foreach (var toSolve in Initiator.IconConditionsToSolve)
            {
                var conditionOne = toSolve.First();
                var conditionTwo = toSolve.Last();

                var findRoad = DefinitivePuzzleRoads.Any(s =>
                                (s.StartsOrEndsAt(conditionOne.PuzzleIndex, conditionOne.Position) &&
                                !s.StartsOrEndsAt(conditionTwo.PuzzleIndex, conditionTwo.Position)) ||
                                (s.StartsOrEndsAt(conditionTwo.PuzzleIndex, conditionTwo.Position) &&
                                !s.StartsOrEndsAt(conditionOne.PuzzleIndex, conditionOne.Position)));

                if (findRoad)
                {
                    return false;
                    //toSolve.SolveState = SolveState.Invalid;
                }
                //returnValues.Add(findRoad);
            }

            return true;
        }

        public bool IsDefinitiveRoadListPossibleForPagoda()
        {
            var returnValues = new HashSet<bool>();

            if (Initiator.PagodaConditionsToSolve.Count == 0) return true;

            foreach (var toSolve in Initiator.PagodaConditionsToSolve)
            {
                var findRoad = DefinitivePuzzleRoads.Any(s =>
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position) &&
                                !s.SpecialConditions.Any(sc => sc.Key == toSolve.Condition));

                if (findRoad)
                {
                    return false;
                    //toSolve.SolveState = SolveState.Invalid;
                }
                //returnValues.Add(findRoad);
            }

            return true;
        }

        public bool IsDefinitiveRoadListPossibleForYinYang()
        {
            var returnValues = new HashSet<bool>();

            if (Initiator.YinYangConditionsToSolve.Count == 0) return true;

            foreach (var toSolve in Initiator.YinYangConditionsToSolve)
            {
                var findRoad = DefinitivePuzzleRoads.Any(s =>
                                s.StartsOrEndsAt(toSolve.PuzzleIndex, toSolve.Position) &&
                                !s.SpecialConditions.Any(sc => sc.Key == toSolve.Condition));

                if (findRoad)
                {
                    return false;
                    //toSolve.SolveState = SolveState.Invalid;
                }
                //returnValues.Add(findRoad);
            }

            return true;
        }


        #endregion
    }
}