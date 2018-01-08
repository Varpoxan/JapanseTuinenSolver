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
                    new Road(0, Orientation.Top, 3, Orientation.Right),
                    new Road(0, Orientation.Left, 1, Orientation.Right, Condition.Bridge),
                    new Road(1, Orientation.Top, 3, Orientation.Bottom, Condition.Bridge),
                    new Road(2, Orientation.Left, 2, Orientation.Bottom)
                }
            };

            var allOtherRotations = GetAllRotations(tile);
            tile.TileRotationList.AddRange(allOtherRotations);

            return tile;
        }

        private Tile GetTileTwo()
        {
            var tile = new Tile(2, 0)
            {
                RoadList = {
                new Road(0, Orientation.Top, 2, Orientation.Left),
                new Road(0, Orientation.Left, 1, Orientation.Top),
                new Road(1, Orientation.Right, 2, Orientation.Bottom, Condition.Pagoda),
                new Road(3, Orientation.Right, 3, Orientation.Bottom)
            }
            };

            var allOtherRotations = GetAllRotations(tile);
            tile.TileRotationList.AddRange(allOtherRotations);

            return tile;
        }

        private Tile GetTileThree()
        {
            var tile = new Tile(3, 0)
            {
                RoadList = {
                new Road(0, Orientation.Top, 1, Orientation.Top),
                new Road(1, Orientation.Right, 1, Orientation.None, Condition.YinYang),
                new Road(2, Orientation.Left, 2, Orientation.None, Condition.YinYang),
                new Road(3, Orientation.Right, 3, Orientation.Bottom),
                new Road(2, Orientation.Bottom, 0, Orientation.Left)
            }
            };

            var allOtherRotations = GetAllRotations(tile);
            tile.TileRotationList.AddRange(allOtherRotations);

            return tile;
        }

        private Tile GetTileFour()
        {
            var tile = new Tile(4, 0)
            {
                RoadList = {
                new Road(0, Orientation.Top, 1, Orientation.Right),
                new Road(1, Orientation.Top, 3, Orientation.Bottom, Condition.Bridge),
                new Road(3, Orientation.Right, 2, Orientation.Bottom),
                new Road(2, Orientation.Left, 0, Orientation.Left)
            }
            };

            var allOtherRotations = GetAllRotations(tile);
            tile.TileRotationList.AddRange(allOtherRotations);

            return tile;
        }

        private Tile GetTileFive()
        {
            var tile = new Tile(5, 0)
            {
                RoadList = {
                new Road(0, Orientation.Top, 1, Orientation.Top),
                new Road(1, Orientation.Right, 0, Orientation.Left, Condition.Bridge),
                new Road(3, Orientation.Right, 3, Orientation.Bottom),
                new Road(2, Orientation.Bottom, 2, Orientation.Left)
            }
            };

            var allOtherRotations = GetAllRotations(tile);
            tile.TileRotationList.AddRange(allOtherRotations);

            return tile;
        }

        private Tile GetTileSix()
        {
            var tile = new Tile(6, 0)
            {
                RoadList = {
                new Road(0, Orientation.Top, 2, Orientation.Left),
                new Road(1, Orientation.Top, 3, Orientation.Right),
                new Road(3, Orientation.Bottom, 0, Orientation.Left),
                new Road(2, Orientation.Bottom, 1, Orientation.Right)
            }
            };

            var allOtherRotations = GetAllRotations(tile);
            tile.TileRotationList.AddRange(allOtherRotations);

            return tile;
        }

        private Tile GetTileSeven()
        {
            var tile = new Tile(7, 0)
            {
                RoadList = {
                new Road(0, Orientation.Top, 2, Orientation.Bottom, Condition.Bridge),
                new Road(1, Orientation.Top, 2, Orientation.Left),
                new Road(1, Orientation.Right, 3, Orientation.Right),
                new Road(3, Orientation.Bottom, 0, Orientation.Left)
            }
            };

            var allOtherRotations = GetAllRotations(tile);
            tile.TileRotationList.AddRange(allOtherRotations);

            return tile;
        }

        private List<Tile> GetAllRotations(Tile initialRotation)
        {
            var returnList = new List<Tile>();
            var degreeList = new List<int> { 90, 180, 270 };

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