using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JapanseTuinen.Models
{
    public class Tile
    {
        public int PuzzleIndex { get; set; }
        public int TileNumber { get; set; }
        public List<Tile> TileRotationList { get; set; }
        public int Degrees { get; set; }
        public List<Road> RoadList { get; set; }

        public Tile(int tileNumber, int degrees)
        {
            this.TileNumber = tileNumber;
            this.PuzzleIndex = -1;
            this.Degrees = degrees;
            this.TileRotationList = new List<Tile>(3);
            this.RoadList = new List<Road>();
        }

        public Tile GetRotation(int degrees)
        {
            return this.TileRotationList.FirstOrDefault(s => s.Degrees == degrees);
        }
    }

    public class SimpleTileIndex
    {
        public int PuzzleIndex { get; set; }
        public int Position { get; set; }
        public Orientation Orientation { get; set; }
        public SpecialCondition SpecialCondition { get; set; }

        public SimpleTileIndex(int pIndex, int pos, Orientation ori, SpecialCondition spCon)
        {
            this.PuzzleIndex = pIndex;
            this.Position = pos;
            this.Orientation = ori;
            this.SpecialCondition = spCon;
        }
    }

    public class TileIndex
    {
        public int Position { get; set; }
        public List<TileInfo> TileInfoList { get; set; }


        public TileIndex()
        {
            this.TileInfoList = new List<TileInfo>(2);
        }
    }

    public class TileInfo
    {
        public Orientation Orientation { get; set; }
        public SpecialCondition SpecialCondition { get; set; }
    }

    public class Road
    {
        public int StartPosition { get; set; }
        public Orientation StartOrientation { get; set; }
        public int EndPosition { get; set; }
        public Orientation EndOrientation { get; set; }
        public Condition RoadAttribute { get; set; }

        public Road(Road road)
        {
            this.StartPosition = road.StartPosition;
            this.StartOrientation = road.StartOrientation;
            this.EndPosition = road.EndPosition;
            this.EndOrientation = road.EndOrientation;
            this.RoadAttribute = road.RoadAttribute;
        }

        public Road(int startPosition, Orientation startOrientation, int endPosition, Orientation endOrientation)
        {
            this.StartPosition = startPosition;
            this.StartOrientation = startOrientation;
            this.EndPosition = endPosition;
            this.EndOrientation = endOrientation;
            this.RoadAttribute = Condition.None;
        }

        public Road(int startPosition, Orientation startOrientation, int endPosition, Orientation endOrientation, Condition roadAttribute)
        {
            this.StartPosition = startPosition;
            this.StartOrientation = startOrientation;
            this.EndPosition = endPosition;
            this.EndOrientation = endOrientation;
            this.RoadAttribute = roadAttribute;
        }

        public override string ToString()
        {
            return String.Format("{0}-{1} to {2}-{3}", StartPosition, StartOrientation, EndPosition, EndOrientation);
        }

        public void SwitchStartToEnd()
        {
            var newStartPos = this.EndPosition;
            var newEndPos = this.StartPosition;
            this.StartPosition = newStartPos;
            this.EndPosition = newEndPos;

            var newStartOri = this.EndOrientation;
            var newEndOri = this.StartOrientation;
            this.StartOrientation = newStartOri;
            this.EndOrientation = newEndOri;
        }

        public int StartsOrEndsAtAmount(Orientation orientation)
        {
            var amount = 0;

            if (StartsAt(orientation)) { amount++; }
            if (EndsAt(orientation)) { amount++; }

            return amount;
        }

        public bool StartsOrEndsAt(params Orientation[] orientations)
        {
            return orientations.Contains(StartOrientation) || orientations.Contains(EndOrientation);
        }

        public bool StartsAt(params Orientation[] orientations)
        {
            return orientations.Contains(StartOrientation);
        }

        public bool EndsAt(params Orientation[] orientations)
        {
            return orientations.Contains(EndOrientation);
        }

        public bool StartsOrEndsAt(Orientation orientation)
        {
            return StartOrientation == orientation || EndOrientation == orientation;
        }

        public bool StartsOrEndsAt(int position)
        {
            return StartPosition == position || EndPosition == position;
        }

        public bool StraightHorizontal
        {
            get
            {
                return (StartOrientation == Orientation.Left && EndOrientation == Orientation.Right) ||
                    (StartOrientation == Orientation.Right && EndOrientation == Orientation.Left);
            }
        }

        public bool StraightVertical
        {
            get
            {
                return (StartOrientation == Orientation.Top && EndOrientation == Orientation.Bottom) ||
                    (StartOrientation == Orientation.Bottom && EndOrientation == Orientation.Top);
            }
        }

        public Road GetRotatedRoad(int degrees)
        {
            if (degrees > 0)
            {
                var newStartPos = GetNewPosition(this.StartPosition, degrees);
                var newEndPos = GetNewPosition(this.EndPosition, degrees);
                var newStartOrientation = GetNewOrientation(this.StartOrientation, degrees);
                var newEndOrientation = GetNewOrientation(this.EndOrientation, degrees);
                var newRoad = new Road(newStartPos, newStartOrientation, newEndPos, newEndOrientation, this.RoadAttribute);
                return newRoad;
            }
            return null;
        }

        public string HtmlClass
        {
            get
            {
                var returnValue = String.Format("road-{0}{1}-to-{2}{3}", StartPosition, StartOrientation.ToString()[0], EndPosition, EndOrientation.ToString()[0]);
                return returnValue;
            }
        }

        public int GetNewPosition(int position, int degrees)
        {
            if (degrees == 90)
            {
                switch (position)
                {
                    case 0:
                        return 1;
                    case 1:
                        return 3;
                    case 2:
                        return 0;
                    case 3:
                        return 2;
                }
            }
            else if (degrees == 180)
            {
                switch (position)
                {
                    case 0:
                        return 3;
                    case 1:
                        return 2;
                    case 2:
                        return 1;
                    case 3:
                        return 0;
                }
            }
            else if (degrees == 270)
            {
                switch (position)
                {
                    case 0:
                        return 2;
                    case 1:
                        return 0;
                    case 2:
                        return 3;
                    case 3:
                        return 1;
                }
            }

            return 4; //This make sure it crashes.
        }

        public Orientation GetNewOrientation(Orientation orientation, int degrees)
        {
            if (degrees == 90)
            {
                switch (orientation)
                {
                    case Orientation.Top:
                        return Orientation.Right;
                    case Orientation.Right:
                        return Orientation.Bottom;
                    case Orientation.Bottom:
                        return Orientation.Left;
                    case Orientation.Left:
                        return Orientation.Top;
                }
            }
            else if (degrees == 180)
            {
                switch (orientation)
                {
                    case Orientation.Top:
                        return Orientation.Bottom;
                    case Orientation.Right:
                        return Orientation.Left;
                    case Orientation.Bottom:
                        return Orientation.Top;
                    case Orientation.Left:
                        return Orientation.Right;
                }
            }
            else if (degrees == 270)
            {
                switch (orientation)
                {
                    case Orientation.Top:
                        return Orientation.Left;
                    case Orientation.Right:
                        return Orientation.Top;
                    case Orientation.Bottom:
                        return Orientation.Right;
                    case Orientation.Left:
                        return Orientation.Bottom;
                }
            }

            return Orientation.None;
        }

        public bool DefinitiveEndingRoad(int puzzleIndex)
        {
            var definitiveEnding = false;
            switch (puzzleIndex)
            {
                case 1:
                    if (EndsAt(Orientation.Left, Orientation.Top))
                    {
                        this.SwitchStartToEnd();
                    }
                    definitiveEnding = !StartsOrEndsAt(Orientation.Bottom) && !StartsOrEndsAt(Orientation.Right);
                    break;
                case 2:
                    if (EndsAt(Orientation.Top))
                    {
                        this.SwitchStartToEnd();
                    }
                    definitiveEnding = ((EndPosition == 0 && EndOrientation == Orientation.Top) ||
                            EndPosition == 1 && EndOrientation == Orientation.Top);
                    break;
                case 3:
                    if (EndsAt(Orientation.Top, Orientation.Right))
                    {
                        this.SwitchStartToEnd();
                    }
                    definitiveEnding = ((EndPosition == 0 && EndOrientation == Orientation.Top) ||
                            (EndPosition == 3 && EndOrientation == Orientation.Right) ||
                            EndPosition == 1);
                    break;
                case 4:
                    if (EndsAt(Orientation.Bottom, Orientation.Left))
                    {
                        this.SwitchStartToEnd();
                    }
                    definitiveEnding = !StartsOrEndsAt(Orientation.Top) && !StartsOrEndsAt(Orientation.Right);
                    break;
                case 5:
                    if (EndsAt(Orientation.Bottom))
                    {
                        this.SwitchStartToEnd();
                    }
                    definitiveEnding = ((EndPosition == 2 && EndOrientation == Orientation.Bottom) ||
                            EndPosition == 3 && EndOrientation == Orientation.Bottom);
                    break;
                case 6:
                    if (EndsAt(Orientation.Top) || EndsAt(Orientation.Right))
                    {
                        this.SwitchStartToEnd();
                    }
                    definitiveEnding = ((EndPosition == 1 && EndOrientation == Orientation.Right) ||
                            (EndPosition == 2 && EndOrientation == Orientation.Bottom) ||
                            EndPosition == 3);
                    break;
            }

            return definitiveEnding;
        }
    }
}