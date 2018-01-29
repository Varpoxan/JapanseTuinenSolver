using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JapanseTuinen.Models.JSON
{
    public class RoadCondition
    {
        public int roadindex { get; set; }
        public Condition condition { get; set; }
        public int? amount { get; set; }

        //public bool IsIconCondition
        //{
        //    get
        //    {
        //        return condition.IsIconCondition();
        //    }
        //}

        public bool IsSvgCondition
        {
            get
            {
                return condition.IsSvgIcon();
            }
        }

        public bool IsTileOrBridge
        {
            get
            {
                return condition == Condition.Tile || condition == Condition.Bridge;
            }
        }

        public string GetClass()
        {
            if (condition.IsIconCondition())
            {
                return String.Format("condition-choice svg-icon {0}-icon", GetCondition());
            }
            else
            {
                return String.Format("condition-choice {0}-road-end", GetCondition());
            }
            /*
             condition-choice svg-icon ' +
                        iconClass +
                        '-icon" src="/Content/Icons/' + iconClass +
                        '.svg" data-position="' + position +
                        '" data-condition="' + iconClass + '" />');
             */
        }

        public string GetSrc()
        {
            return String.Format("/Content/Icons/{0}.svg", GetCondition());
        }

        public string GetCondition()
        {
            return condition.ToString().ToLower();
        }
    }

    public class Puzzletile
    {
        public int number { get; set; }
        public List<RoadCondition> conditions { get; set; }
    }

    public class Puzzle : PuzzleViewModel
    {
        public string name { get; set; }
        public List<Puzzletile> puzzletile { get; set; }
    }

    public class PuzzleSet
    {
        public string name { get; set; }
        public string version { get; set; }
        public List<Puzzle> puzzles { get; set; }
    }
}