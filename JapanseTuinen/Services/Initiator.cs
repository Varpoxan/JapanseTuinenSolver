using JapanseTuinen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JapanseTuinen.Services
{
    public class Initiator
    {
        private List<Tile> _tileList { get; set; }

        public List<Tile> TileList
        {
            get
            {
                return _tileList ?? GetTiles();
            }
        }

        public TileViewModel GetTileVM()
        {
            var tileVM = new TileViewModel();
            tileVM.TileList = TileList;

            return tileVM;
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
                new Road(3, 9, Condition.YinYang),
                new Road(7, 9, Condition.YinYang),
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

    }
}