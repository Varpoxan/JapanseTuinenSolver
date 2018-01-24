using JapanseTuinen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JapanseTuinen.Services
{
    public partial class PuzzleService
    {
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

            int endWhileCount = 0;
            OpenPuzzleRoads = OpenPuzzleRoads.OrderBy(s => s.RoadStartsOrEndsAtPuzzleEdge == false).ToList();
            while (OpenPuzzleRoads.Any() && endWhileCount <= 30)
            {
                if (!FindRoadEndingOnCurrentStart(OpenPuzzleRoads[0]))
                {
                    if (OpenPuzzleRoads.Count > 1)
                        FindRoadEndingOnCurrentStart(OpenPuzzleRoads[1]);
                };
                endWhileCount++;
                if (endWhileCount >= 29)
                {
                    //Dit is niet goed...
                }
            }

            return DefinitivePuzzleRoads;
        }

        public PuzzleRoad CreatePuzzleRoad(Road road, int startPuzzleIndex)
        {
            var newPuzzleRoad = new PuzzleRoad(startPuzzleIndex, road);

            var amountOfOpenRoadEnds = AmountOfOpenRoadEnds(newPuzzleRoad);

            if (amountOfOpenRoadEnds == 0)
            {
                newPuzzleRoad.EndPuzzleIndex = newPuzzleRoad.StartPuzzleIndex;
                newPuzzleRoad.SpecialConditions.Add(new SpecialCondition(newPuzzleRoad.RoadAttribute));
                DefinitivePuzzleRoads.Add(newPuzzleRoad);
            }
            else
            {
                newPuzzleRoad.RoadStartsOrEndsAtPuzzleEdge = amountOfOpenRoadEnds <= 1;
                OpenPuzzleRoads.Add(newPuzzleRoad);
            }

            return newPuzzleRoad;
        }

        public bool FindRoadEndingOnCurrentStart(PuzzleRoad road)
        {
            var newPuzzleIndex = road.EndPuzzleIndex.HasValue ? road.EndPuzzleIndex.Value : road.StartPuzzleIndex;

            //var tileString = String.Join(" + ", UsedTileList.OrderBy(s => s.PuzzleIndex).Select(s => s.ToString()));
            //var test = UsedTileList;

            if (road.StartsOrEndsAt(1))
            {
                var findRoad = Enumerable.Empty<PuzzleRoad>();
                if (road.EndPosition == 1)
                {
                    findRoad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) + 3 &&
                                                        s.StartsOrEndsAt(6));
                    
                }
                if (!findRoad.Any() && road.StartPosition == 1)
                {
                    findRoad = OpenPuzzleRoads.Where(s => road.StartPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) + 3 &&
                                                        s.StartsOrEndsAt(6));
                }

                if (findRoad.Count() == 1)
                {
                    
                    var relevantRoad = findRoad.First();
                    if (!CheckForRoundabout(road, relevantRoad))
                    {
                        road.CombineRoad(relevantRoad);
                        ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    }
                    return true;
                }
            }
            if (road.StartsOrEndsAt(2))
            {
                var findRoad = Enumerable.Empty<PuzzleRoad>();
                if (road.EndPosition == 2)
                {
                    findRoad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) + 3 &&
                                                        s.StartsOrEndsAt(5));
                    
                }
                if (!findRoad.Any() && road.StartPosition == 2)
                {
                    findRoad = OpenPuzzleRoads.Where(s => road.StartPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) + 3 &&
                                                        s.StartsOrEndsAt(5));
                }

                if (findRoad.Count() == 1)
                {
                    var relevantRoad = findRoad.First();
                    if (!CheckForRoundabout(road, relevantRoad))
                    {
                        road.CombineRoad(relevantRoad);
                        ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    }
                    return true;
                }
            }
            if (road.StartsOrEndsAt(3))
            {
                var findRoad = Enumerable.Empty<PuzzleRoad>();
                if (road.EndPosition == 3 && newPuzzleIndex != 3) // Otherwise puzzle index 3 looks to the right (puzzle index 4) for roads
                {
                    findRoad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) - 1 &&
                                                        s.StartsOrEndsAt(8));
                    
                }
                if (!findRoad.Any() && road.StartPosition == 3 && road.StartPuzzleIndex != 3)
                {
                    findRoad = OpenPuzzleRoads.Where(s => road.StartPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) - 1 &&
                                                        s.StartsOrEndsAt(8));
                }

                if (findRoad.Count() == 1)
                {
                    var relevantRoad = findRoad.First();
                    if (!CheckForRoundabout(road, relevantRoad))
                    {
                        road.CombineRoad(relevantRoad);
                        ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    }
                    return true;
                }
            }
            if (road.StartsOrEndsAt(4))
            {
                var findRoad = Enumerable.Empty<PuzzleRoad>();
                if (road.EndPosition == 4 && newPuzzleIndex != 3) // Otherwise puzzle index 3 looks to the right (puzzle index 4) for roads
                {
                    findRoad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) - 1 &&
                                                        s.StartsOrEndsAt(7));
                    
                }
                if (!findRoad.Any() && road.StartPosition == 4 && road.StartPuzzleIndex != 3)
                {
                    findRoad = OpenPuzzleRoads.Where(s => road.StartPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) - 1 &&
                                                        s.StartsOrEndsAt(7));
                }

                if (findRoad.Count() == 1)
                {
                    var relevantRoad = findRoad.First();
                    if (!CheckForRoundabout(road, relevantRoad))
                    {
                        road.CombineRoad(relevantRoad);
                        ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    }
                    return true;
                }
            }
            if (road.StartsOrEndsAt(5))
            {
                var findRoad = Enumerable.Empty<PuzzleRoad>();
                if (road.EndPosition == 5)
                {
                    findRoad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) - 3 &&
                                                        s.StartsOrEndsAt(2));
                    
                }
                if (!findRoad.Any() && road.StartPosition == 5)
                {
                    findRoad = OpenPuzzleRoads.Where(s => road.StartPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) - 3 &&
                                                        s.StartsOrEndsAt(2));
                }

                if (findRoad.Count() == 1)
                {
                    var relevantRoad = findRoad.First();
                    if (!CheckForRoundabout(road, relevantRoad))
                    {
                        road.CombineRoad(relevantRoad);
                        ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    }
                    return true;
                }

            }
            if (road.StartsOrEndsAt(6))
            {
                var findRoad = Enumerable.Empty<PuzzleRoad>();
                if (road.EndPosition == 6)
                {
                    findRoad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) - 3 &&
                                                        s.StartsOrEndsAt(1));
                    
                }
                if (!findRoad.Any() && road.StartPosition == 6)
                {
                    findRoad = OpenPuzzleRoads.Where(s => road.StartPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) - 3 &&
                                                        s.StartsOrEndsAt(1));
                }

                if (findRoad.Count() == 1)
                {
                    var relevantRoad = findRoad.First();
                    if (!CheckForRoundabout(road, relevantRoad))
                    {
                        road.CombineRoad(relevantRoad);
                        ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    }
                    return true;
                }
            }
            if (road.StartsOrEndsAt(7))
            {
                var findRoad = Enumerable.Empty<PuzzleRoad>();
                if (road.EndPosition == 7 && newPuzzleIndex != 4) // Otherwise puzzle index 4 looks to the left (puzzle index 3) for roads
                {
                    findRoad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) + 1 &&
                                                        s.StartsOrEndsAt(4));
                    
                }
                if (!findRoad.Any() && road.StartPosition == 7 && road.StartPuzzleIndex != 4)
                {
                    findRoad = OpenPuzzleRoads.Where(s => road.StartPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) + 1 &&
                                                        s.StartsOrEndsAt(4));
                }

                if (findRoad.Count() == 1)
                {
                    var relevantRoad = findRoad.First();
                    if (!CheckForRoundabout(road, relevantRoad))
                    {
                        road.CombineRoad(relevantRoad);
                        ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    }
                    return true;
                }
            }
            if (road.StartsOrEndsAt(8))
            {
                var findRoad = Enumerable.Empty<PuzzleRoad>();
                if (road.EndPosition == 8 && newPuzzleIndex != 4) // Otherwise puzzle index 4 looks to the left (puzzle index 3) for roads
                {
                    findRoad = OpenPuzzleRoads.Where(s => newPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) + 1 &&
                                                        s.StartsOrEndsAt(3));
                }
                if (!findRoad.Any() && road.StartPosition == 8 && road.StartPuzzleIndex != 4)
                {
                    findRoad = OpenPuzzleRoads.Where(s => road.StartPuzzleIndex == (s.EndPuzzleIndex.HasValue ? s.EndPuzzleIndex.Value : s.StartPuzzleIndex) + 1 &&
                                                        s.StartsOrEndsAt(3));
                }

                if (findRoad.Count() == 1)
                {
                    var relevantRoad = findRoad.First();
                    if (!CheckForRoundabout(road, relevantRoad))
                    {
                        road.CombineRoad(relevantRoad);
                        ChangePuzzleRoadListsBasedOnEndings(road, relevantRoad);
                    }
                    return true;
                }
            }

            if (OpenPuzzleRoads.Count == 1)
            {
                //Maybe check if this road connects to itself?
                //Clear it for now, see what happens.
                OpenPuzzleRoads.Clear();
            }

            //Het gaat hier HELEMAAL niet goed. Er is geen weg gevonden!?
            return false;
        }

        public void ChangePuzzleRoadListsBasedOnEndings(PuzzleRoad road, PuzzleRoad relevantRoad)
        {
            var amountOfOpenRoadEnds = AmountOfOpenRoadEnds(relevantRoad);
            if (amountOfOpenRoadEnds == 2)
            {
                //The relevantroad is still open, we need to further connect roads to the road object.
                OpenPuzzleRoads.Remove(relevantRoad);
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

        public bool CheckForRoundabout(PuzzleRoad road, PuzzleRoad relevantRoad)
        {
            if (road.StartsAndEndsAtSameOrientation && !road.EndPuzzleIndex.HasValue &&
                relevantRoad.StartsAndEndsAtSameOrientation && !relevantRoad.EndPuzzleIndex.HasValue)
            {
                OpenPuzzleRoads.Remove(road);
                OpenPuzzleRoads.Remove(relevantRoad);
                return true;
            }
            return false;
        }

        public int AmountOfOpenRoadEnds(PuzzleRoad puzzleRoad)
        {
            int amount = 0;
            var usingPuzzleIndex = puzzleRoad.StartPuzzleIndex;
            if (puzzleRoad.DefinitiveEndingRoad(usingPuzzleIndex))
            {
                return 0;
            }

            switch (usingPuzzleIndex)
            {
                case 1:
                    if (!Initiator.SubmittedPuzzleTilesIndices.Contains(2) && !Initiator.SubmittedPuzzleTilesIndices.Contains(4))
                    {
                        return 0;
                    }
                    if (Initiator.SubmittedPuzzleTilesIndices.Contains(4))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Bottom))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Bottom);
                    }
                    if (Initiator.SubmittedPuzzleTilesIndices.Contains(2))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Right))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Right);
                    }
                    break;
                case 2:
                    if (!Initiator.SubmittedPuzzleTilesIndices.Contains(1) && !Initiator.SubmittedPuzzleTilesIndices.Contains(3) && !Initiator.SubmittedPuzzleTilesIndices.Contains(5))
                    {
                        return 0;
                    }
                    if (Initiator.SubmittedPuzzleTilesIndices.Contains(1))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Left))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Left);
                    }
                    if (Initiator.SubmittedPuzzleTilesIndices.Contains(3))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Right))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Right);
                    }
                    if (Initiator.SubmittedPuzzleTilesIndices.Contains(5))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Bottom))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Bottom);
                    }
                    break;
                case 3:
                    if (!Initiator.SubmittedPuzzleTilesIndices.Contains(2) && !Initiator.SubmittedPuzzleTilesIndices.Contains(6))
                    {
                        return 0;
                    }
                    if (Initiator.SubmittedPuzzleTilesIndices.Contains(2))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Left))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Left);
                    }
                    if (Initiator.SubmittedPuzzleTilesIndices.Contains(6))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Bottom))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Bottom);
                    }
                    break;
                case 4:
                    if (!Initiator.SubmittedPuzzleTilesIndices.Contains(1) && !Initiator.SubmittedPuzzleTilesIndices.Contains(5))
                    {
                        return 0;
                    }
                    if (Initiator.SubmittedPuzzleTilesIndices.Contains(1))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Top))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Top);
                    }
                    if (Initiator.SubmittedPuzzleTilesIndices.Contains(5))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Right))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Right);
                    }
                    break;
                case 5:
                    if (!Initiator.SubmittedPuzzleTilesIndices.Contains(4) && !Initiator.SubmittedPuzzleTilesIndices.Contains(6) && !Initiator.SubmittedPuzzleTilesIndices.Contains(2))
                    {
                        return 0;
                    }
                    if (Initiator.SubmittedPuzzleTilesIndices.Contains(4))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Left))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Left);
                    }
                    if (Initiator.SubmittedPuzzleTilesIndices.Contains(6))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Right))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Right);
                    }
                    if (Initiator.SubmittedPuzzleTilesIndices.Contains(2))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Top))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Top);
                    }
                    break;
                case 6:
                    if (!Initiator.SubmittedPuzzleTilesIndices.Contains(3) && !Initiator.SubmittedPuzzleTilesIndices.Contains(5))
                    {
                        return 0;
                    }
                    if (Initiator.SubmittedPuzzleTilesIndices.Contains(3))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Top))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Top);
                    }
                    if (Initiator.SubmittedPuzzleTilesIndices.Contains(5))
                    {
                        if (puzzleRoad.StartsAt(Orientation.Left))
                            puzzleRoad.SwitchStartToEnd();

                        amount += puzzleRoad.StartsOrEndsAtAmount(Orientation.Left);
                    }
                    break;
            }

            return amount;
        }
    }
}