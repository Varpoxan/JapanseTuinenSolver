using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JapanseTuinen.Models
{
    public class Road
    {
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }
        public Condition RoadAttribute { get; set; }

        public Road(Road road)
        {
            this.StartPosition = road.StartPosition;
            this.EndPosition = road.EndPosition;
            this.RoadAttribute = road.RoadAttribute;
        }

        public Road(int startPosition, int endPosition)
        {
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
            this.RoadAttribute = Condition.None;
        }

        public Road(int startPosition, int endPosition, Condition roadAttribute)
        {
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
            this.RoadAttribute = roadAttribute;
        }

        public override string ToString()
        {
            return String.Format("{0} to {1}", StartPosition, EndPosition);
        }

        public void SwitchStartToEnd()
        {
            var newStartPos = this.EndPosition;
            var newEndPos = this.StartPosition;
            this.StartPosition = newStartPos;
            this.EndPosition = newEndPos;
        }

        public int StartsOrEndsAtAmount(Orientation orientation)
        {
            var amount = 0;

            if (StartsAt(orientation)) { amount++; }
            if (EndsAt(orientation)) { amount++; }

            return amount;
        }

        public Road GetRotatedRoad(int degrees)
        {
            if (degrees > 0)
            {
                var newStartPos = GetNewPosition(this.StartPosition, degrees);
                var newEndPos = GetNewPosition(this.EndPosition, degrees);
                var newRoad = new Road(newStartPos, newEndPos, this.RoadAttribute);
                return newRoad;
            }
            else
            {
                return this;
            }
        }

        public bool StartsOrEndsAt(int position)
        {
            return StartPosition == position || EndPosition == position;
        }

        public bool StartsAndEndsAtSameOrientation
        {
            get
            {
                return ((StartPosition == 1 || StartPosition == 2) && (EndPosition == 1 || EndPosition == 2)) ||
                        ((StartPosition == 3 || StartPosition == 4) && (EndPosition == 3 || EndPosition == 4)) ||
                        ((StartPosition == 5 || StartPosition == 6) && (EndPosition == 5 || EndPosition == 6)) ||
                        ((StartPosition == 7 || StartPosition == 8) && (EndPosition == 7 || EndPosition == 8));
            }
        }

        public bool StartsOrEndsAt(Orientation orientation)
        {
            return StartsAt(orientation) || EndsAt(orientation);
        }

        public bool StartsAt(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Top:
                    return (StartPosition == 1 || StartPosition == 2);
                case Orientation.Right:
                    return (StartPosition == 3 || StartPosition == 4);
                case Orientation.Bottom:
                    return (StartPosition == 5 || StartPosition == 6);
                case Orientation.Left:
                    return (StartPosition == 7 || StartPosition == 8);
            }
            return false;
        }

        public bool EndsAt(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Top:
                    return (EndPosition == 1 || EndPosition == 2);
                case Orientation.Right:
                    return (EndPosition == 3 || EndPosition == 4);
                case Orientation.Bottom:
                    return (EndPosition == 5 || EndPosition == 6);
                case Orientation.Left:
                    return (EndPosition == 7 || EndPosition == 8);
            }
            return false;
        }

        public int GetNewPosition(int position, int degrees)
        {
            if (position == 9)
            {
                return 9;
            }
            var newPos = position + ((degrees / 90) * 2);
            if (newPos > 8)
            {
                newPos -= 8;
            }
            return newPos;
        }

        public bool DefinitiveEndingRoad(int puzzleIndex)
        {
            var definitiveEnding = false;
            switch (puzzleIndex)
            {
                case 1:
                    definitiveEnding = !StartsOrEndsAt(Orientation.Bottom) && !StartsOrEndsAt(Orientation.Right);
                    break;
                case 2:
                    definitiveEnding = (StartPosition == 1 && EndPosition == 2) || (StartPosition == 2 && EndPosition == 1);
                    break;
                case 3:
                    definitiveEnding = !StartsOrEndsAt(Orientation.Bottom) && !StartsOrEndsAt(Orientation.Left);
                    break;
                case 4:
                    definitiveEnding = !StartsOrEndsAt(Orientation.Top) && !StartsOrEndsAt(Orientation.Right);
                    break;
                case 5:
                    definitiveEnding = (StartPosition == 5 && EndPosition == 6) || (StartPosition == 6 && EndPosition == 5);
                    break;
                case 6:
                    definitiveEnding = !StartsOrEndsAt(Orientation.Top) && !StartsOrEndsAt(Orientation.Left);
                    break;
            }

            return definitiveEnding;
        }
    }
}