using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JapanseTuinen.Models
{
    public class HtmlPuzzleHelper
    {
        #region HtmlHelpers
        public int GetRowNumberByIndex(int index)
        {
            if (index <= 3)
            {
                return 1;
            }
            else if (index >= 4 && index <= 6)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        public int GetColumnNumberByIndex(int index)
        {
            if (index == 1 || index == 4 || index == 7)
            {
                return 1;
            }
            else if (index == 2 || index == 5 || index == 8)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        public string GetAbsLeftByIndex(int index)
        {
            var row = GetRowNumberByIndex(index);
            var col = GetColumnNumberByIndex(index);
            if (col > 1)
            {
                return String.Format("{0}px", (col - 1) * 200);
            }
            else
            {
                return "0";
            }
        }

        public string GetAbsTopByIndex(int index)
        {
            var row = GetRowNumberByIndex(index);
            var col = GetColumnNumberByIndex(index);
            if (row > 1)
            {
                return String.Format("{0}px", (row - 1) * 200);
            }
            else
            {
                return "0";
            }
        }

        public string GetRowClass(int index)
        {
            if (index <= 3)
            {
                return "row-one";
            }
            else if (index >= 4 && index <= 6)
            {
                return "row-two";
            }
            else
            {
                return "row-three";
            }
        }

        public string GetColumnClass(int index)
        {
            if (index == 1 || index == 4 || index == 7)
            {
                return "column-one";
            }
            else if (index == 2 || index == 5 || index == 8)
            {
                return "column-two";
            }
            else
            {
                return "column-three";
            }
        }

        public string GetRoadOrientation(int roadIndex)
        {
            return roadIndex == 1 || roadIndex == 2 || roadIndex == 5 || roadIndex == 6 ? "road-end-vertical" : "road-end-horizontal";
        }

        public string GetRoadEndSide(int roadIndex)
        {
            var init = "road-end-";
            if (roadIndex == 1 || roadIndex == 2)
            {
                init += "top";
            }
            else if (roadIndex == 3 || roadIndex == 4)
            {
                init += "right";
            }
            else if (roadIndex == 5 || roadIndex == 6)
            {
                init += "bottom";
            }
            else if (roadIndex == 7 || roadIndex == 8)
            {
                init += "left";
            }
            return init;
        }
        #endregion
    }
}