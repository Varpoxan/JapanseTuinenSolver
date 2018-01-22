using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JapanseTuinen.Models
{
    public class PuzzleRoad : Road
    {
        public int StartPuzzleIndex { get; set; }
        public int? EndPuzzleIndex { get; set; }
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
            return String.Format("PI: {0} - {1} to {2}", EndPuzzleIndex.HasValue ? EndPuzzleIndex.Value : StartPuzzleIndex, StartPosition, EndPosition);
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
            var roadHasUpdated = false;

            if (Math.Abs(this.StartPosition - puzzleRoad.StartPosition) == 5 &&
                (this.StartPosition + puzzleRoad.StartPosition == 7 || this.StartPosition + puzzleRoad.StartPosition == 11))
            {
                this.EndPosition = puzzleRoad.EndPosition != this.StartPosition ? puzzleRoad.EndPosition : puzzleRoad.StartPosition;
                roadHasUpdated = true;
            }
            else if (Math.Abs(this.StartPosition - puzzleRoad.EndPosition) == 5 &&
                (this.StartPosition + puzzleRoad.EndPosition == 7 || this.StartPosition + puzzleRoad.EndPosition == 11))
            {
                this.EndPosition = puzzleRoad.StartPosition != this.StartPosition ? puzzleRoad.StartPosition : puzzleRoad.EndPosition;
                roadHasUpdated = true;
            }
            else if (Math.Abs(this.EndPosition - puzzleRoad.EndPosition) == 5 &&
                (this.EndPosition + puzzleRoad.EndPosition == 7 || this.EndPosition + puzzleRoad.EndPosition == 11))
            {
                this.EndPosition = puzzleRoad.StartPosition != this.StartPosition ? puzzleRoad.StartPosition : puzzleRoad.EndPosition;
                roadHasUpdated = true;
            }
            else if (Math.Abs(this.EndPosition - puzzleRoad.StartPosition) == 5 &&
                (this.EndPosition + puzzleRoad.StartPosition == 7 || this.EndPosition + puzzleRoad.StartPosition == 11))
            {
                this.EndPosition = puzzleRoad.EndPosition != this.StartPosition ? puzzleRoad.EndPosition : puzzleRoad.StartPosition;
                roadHasUpdated = true;
            }
            else if (Math.Abs(this.StartPosition - puzzleRoad.StartPosition) == 3 &&
                (this.StartPosition + puzzleRoad.StartPosition == 7 || this.StartPosition + puzzleRoad.StartPosition == 11))
            {
                this.EndPosition = puzzleRoad.EndPosition != this.StartPosition ? puzzleRoad.EndPosition : puzzleRoad.StartPosition;
                roadHasUpdated = true;
            }
            else if (Math.Abs(this.StartPosition - puzzleRoad.EndPosition) == 3 &&
                (this.StartPosition + puzzleRoad.EndPosition == 7 || this.StartPosition + puzzleRoad.EndPosition == 11))
            {
                this.EndPosition = puzzleRoad.StartPosition != this.StartPosition ? puzzleRoad.StartPosition : puzzleRoad.EndPosition;
                roadHasUpdated = true;
            }
            else if (Math.Abs(this.EndPosition - puzzleRoad.EndPosition) == 3 &&
                (this.EndPosition + puzzleRoad.EndPosition == 7 || this.EndPosition + puzzleRoad.EndPosition == 11))
            {
                this.EndPosition = puzzleRoad.StartPosition != this.StartPosition ? puzzleRoad.StartPosition : puzzleRoad.EndPosition;
                roadHasUpdated = true;
            }
            else if (Math.Abs(this.EndPosition - puzzleRoad.StartPosition) == 3 &&
                (this.EndPosition + puzzleRoad.StartPosition == 7 || this.EndPosition + puzzleRoad.StartPosition == 11))
            {
                this.EndPosition = puzzleRoad.EndPosition != this.StartPosition ? puzzleRoad.EndPosition : puzzleRoad.StartPosition;
                roadHasUpdated = true;
            }

            if (!roadHasUpdated) //Road is NOT updated! We need to review our method!
            {

            }

            this.EndPuzzleIndex = puzzleRoad.StartPuzzleIndex;
            this.PuzzleIndexArray.Add(puzzleRoad.StartPuzzleIndex);
        }

        public bool StartsOrEndsAt(int puzzleIndex, int position)
        {
            return (this.StartPuzzleIndex == puzzleIndex && this.StartPosition == position) ||
                (this.EndPuzzleIndex == puzzleIndex && this.EndPosition == position);
        }
    }
}