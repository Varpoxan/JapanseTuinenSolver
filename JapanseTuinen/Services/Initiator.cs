using JapanseTuinen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JapanseTuinen.Services
{
    public class Initiator
    {
        public PuzzleViewModel PuzzleVM { get; set; }
        public List<Tile> TileList { get; set; }
        public Dictionary<UsedTileDictionaryKey, int> CheckedTileDictionary { get; set; }
        public Dictionary<UsedTileDictionaryKey, int> DynamicCheckedTileDictionary { get; set; }
        public HashSet<int> SubmittedPuzzleTilesIndices { get; set; }
        public int SubmittedPuzzleTileCount { get; set; }
        public int AmountOfTotalSolutions { get; set; }
        public int AmountOfCheckedSolutions { get; set; }
        public int AmountOfMaximumTriesPerTile { get; set; }
        public Dictionary<int, int> PuzzleIndexCounter { get; set; }
        public Dictionary<int, int> DepthToIndex { get; set; }
        public Dictionary<int, int> OriginalDepthCounter { get; set; }
        public List<SimpleTileIndex> SimpleConditionsList { get; set; }

        public void Initialize(PuzzleViewModel puzzleVM)
        {
            this.PuzzleVM = puzzleVM;
            TileList = GetTiles();
            SubmittedPuzzleTilesIndices = GetSubmittedPuzzleTilesIndices();
            SubmittedPuzzleTileCount = SubmittedPuzzleTilesIndices.Count;
            PuzzleIndexCounter = GetPuzzleIndexCounter();
            DepthToIndex = GetDepthToIndexDictionary();
            CheckedTileDictionary = GetCheckedTileDictionary();
            DynamicCheckedTileDictionary = GetDynamicCheckedTileDictionary();
            AmountOfTotalSolutions = GetTotalAmountOfSolutions();
            AmountOfMaximumTriesPerTile = GetAmountOfMaximumTriesPerTile();
            OriginalDepthCounter = GetOriginalDepthCounter();
            SimpleConditionsList = GetSimpleConditionsList();
        }

        public TileViewModel GetTileVM()
        {
            var tileVM = new TileViewModel();
            tileVM.TileList = TileList;

            return tileVM;
        }

        private List<SimpleTileIndex> GetSimpleConditionsList()
        {
            return PuzzleVM.PuzzleTileList.SelectMany(s => s.SimpleTileIndexList).ToList();
        }

        private List<Tile> GetTiles()
        {
            var returnList = new List<Tile>
            {
                GetTileOne(),
                GetTileTwo(),
                GetTileThree(),
                GetTileFour(),
                GetTileFive(),
                GetTileSix(),
                GetTileSeven()
            };

            return returnList;
        }

        private Tile GetTileOne()
        {
            var tile = new Tile(1, 0)
            {
                RoadList = {
                    new Road(1, 4),
                    new Road(8, 3, Condition.Bridge),
                    new Road(2, 5, Condition.Bridge),
                    new Road(7, 6)
                }
            };

            var allOtherRotations = GetAllRotations(tile);
            tile.TotalTileRotationList.AddRange(allOtherRotations);

            return tile;
        }

        private Tile GetTileTwo()
        {
            var tile = new Tile(2, 0)
            {
                RoadList = {
                new Road(1, 7),
                new Road(8, 2),
                new Road(3, 6, Condition.Pagoda),
                new Road(4, 5)
            }
            };

            var allOtherRotations = GetAllRotations(tile);
            tile.TotalTileRotationList.AddRange(allOtherRotations);

            return tile;
        }

        private Tile GetTileThree()
        {
            var tile = new Tile(3, 0)
            {
                RoadList = {
                new Road(1, 2),
                new Road(9, 3, Condition.YinYang),
                new Road(9, 7, Condition.YinYang),
                new Road(4, 5),
                new Road(6, 8)
            }
            };

            var allOtherRotations = GetAllRotations(tile);
            tile.TotalTileRotationList.AddRange(allOtherRotations);

            return tile;
        }

        private Tile GetTileFour()
        {
            var tile = new Tile(4, 0)
            {
                RoadList = {
                new Road(1, 3),
                new Road(2, 5, Condition.Bridge),
                new Road(4, 6),
                new Road(7, 8)
            }
            };

            var allOtherRotations = GetAllRotations(tile);
            tile.TotalTileRotationList.AddRange(allOtherRotations);

            return tile;
        }

        private Tile GetTileFive()
        {
            var tile = new Tile(5, 0)
            {
                RoadList = {
                new Road(1, 2),
                new Road(3, 8, Condition.Bridge),
                new Road(4, 5),
                new Road(6, 7)
            }
            };

            var allOtherRotations = GetAllRotations(tile);
            tile.TotalTileRotationList.AddRange(allOtherRotations);

            return tile;
        }

        private Tile GetTileSix()
        {
            var tile = new Tile(6, 0)
            {
                RoadList = {
                new Road(1, 7),
                new Road(2, 4),
                new Road(5, 8),
                new Road(6, 3)
            }
            };

            var allOtherRotations = GetAllRotations(tile);
            tile.TotalTileRotationList.AddRange(allOtherRotations);

            return tile;
        }

        private Tile GetTileSeven()
        {
            var tile = new Tile(7, 0)
            {
                RoadList = {
                new Road(1, 6, Condition.Bridge),
                new Road(2, 7),
                new Road(3, 4),
                new Road(5, 8)
            }
            };

            var allRotations = GetAllRotations(tile);
            tile.TotalTileRotationList.AddRange(allRotations);

            return tile;
        }

        private List<Tile> GetAllRotations(Tile initialRotation)
        {
            var returnList = new List<Tile>();
            var degreeList = new List<int> { 0, 90, 180, 270 };

            foreach (var rot in degreeList)
            {
                var tileRotation = new Tile(initialRotation.TileNumber, rot);

                foreach (var road in initialRotation.RoadList)
                {
                    var newRoad = road.GetRotatedRoad(rot);
                    tileRotation.RoadList.Add(newRoad);
                }

                returnList.Add(tileRotation);
            }

            return returnList;
        }

        private Dictionary<UsedTileDictionaryKey, int> GetCheckedTileDictionary()
        {
            var returnDictionary = new Dictionary<UsedTileDictionaryKey, int>();
            foreach (var depthCounter in SubmittedPuzzleTilesIndices)
            {
                foreach (var tile in TileList)
                {
                    //var firstTileKey = new UsedTileDictionaryKey(usedPIndex, tile.TileNumber, 0);
                    //CheckedTileDictionary.Add(firstTileKey, 0);

                    foreach (var tileRot in tile.TotalTileRotationList)
                    {
                        var tileKey = new UsedTileDictionaryKey(depthCounter, tile.TileNumber, tileRot.Degrees);
                        returnDictionary.Add(tileKey, 0);
                    }
                }
            }

            return returnDictionary;
        }

        private Dictionary<UsedTileDictionaryKey, int> GetDynamicCheckedTileDictionary()
        {
            var returnDictionary = new Dictionary<UsedTileDictionaryKey, int>();
            int pIndexCounter = 0;
            foreach (var depthCounter in SubmittedPuzzleTilesIndices)
            {
                pIndexCounter++;
                foreach (var tile in TileList)
                {
                    foreach (var tileRot in tile.TotalTileRotationList)
                    {
                        var tileKey = new UsedTileDictionaryKey(depthCounter, tile.TileNumber, tileRot.Degrees);
                        //Ok, so this should probably be calculated soon. But i need more time.
                        if (SubmittedPuzzleTileCount == 3)
                        {
                            if (pIndexCounter == 1)
                            {
                                returnDictionary.Add(tileKey, 480);
                            }
                            else if (pIndexCounter == 2)
                            {
                                returnDictionary.Add(tileKey, 20);
                            }
                            else if (pIndexCounter == 3)
                            {
                                returnDictionary.Add(tileKey, 1);
                            }
                        }
                        //Ok, so this should probably be calculated soon. But i need more time.
                        if (SubmittedPuzzleTileCount == 2)
                        {
                            if (pIndexCounter == 1)
                            {
                                returnDictionary.Add(tileKey, 24);
                            }
                            else if (pIndexCounter == 2)
                            {
                                returnDictionary.Add(tileKey, 1);
                            }
                        }
                        //Ok, so this should probably be calculated soon. But i need more time.
                        if (SubmittedPuzzleTileCount == 4)
                        {
                            if (pIndexCounter == 1)
                            {
                                returnDictionary.Add(tileKey, 7680);
                            }
                            if (pIndexCounter == 2)
                            {
                                returnDictionary.Add(tileKey, 320);
                            }
                            if (pIndexCounter == 3)
                            {
                                returnDictionary.Add(tileKey, 16);
                            }
                            if (pIndexCounter == 4)
                            {
                                returnDictionary.Add(tileKey, 1);
                            }
                        }
                    }
                }
            }

            return returnDictionary;
        }

        private int GetTotalAmountOfSolutions()
        {
            var relevantCount = 1;
            for (int i = 0; i < SubmittedPuzzleTilesIndices.Count; i++)
            {
                relevantCount *= (TileList.Count - i) * 4;
            }

            return relevantCount;
        }

        private int GetAmountOfMaximumTriesPerTile()
        {
            var returnInt = AmountOfTotalSolutions / (TileList.Count * 4);
            return returnInt;
        }

        private Dictionary<int, int> GetPuzzleIndexCounter()
        {
            var returnDictionary = new Dictionary<int, int>();
            var pzCount = 1;
            foreach (var pz in SubmittedPuzzleTilesIndices)
            {
                returnDictionary.Add(pz, pzCount);
                pzCount++;
            }

            return returnDictionary;
        }

        private Dictionary<int, int> GetDepthToIndexDictionary()
        {
            var returnDictionary = new Dictionary<int, int>();
            var pzCount = 1;
            foreach (var pz in SubmittedPuzzleTilesIndices)
            {
                returnDictionary.Add(pzCount, pz);
                pzCount++;
            }

            return returnDictionary;
        }

        private Dictionary<int, int> GetOriginalDepthCounter()
        {
            var returnDictionary = new Dictionary<int, int>();
            //Ok, so this should probably be calculated soon. But i need more time.
            if (SubmittedPuzzleTilesIndices.Count == 1)
            {
                returnDictionary.Add(1, 1);
            }
            if (SubmittedPuzzleTilesIndices.Count == 2)
            {
                returnDictionary.Add(1, 24);
                returnDictionary.Add(2, 1);
            }
            if (SubmittedPuzzleTilesIndices.Count == 3)
            {
                returnDictionary.Add(1, 480);
                returnDictionary.Add(2, 20);
                returnDictionary.Add(3, 1);
            }
            if (SubmittedPuzzleTilesIndices.Count == 4)
            {
                returnDictionary.Add(1, 7680);
                returnDictionary.Add(2, 320);
                returnDictionary.Add(3, 16);
                returnDictionary.Add(4, 1);
            }

            return returnDictionary;
        }

        private HashSet<int> GetSubmittedPuzzleTilesIndices()
        {
            var returnList = new HashSet<int>(PuzzleVM.PuzzleTileList.Select(s => s.Index));
            return returnList;
        }
    }
}