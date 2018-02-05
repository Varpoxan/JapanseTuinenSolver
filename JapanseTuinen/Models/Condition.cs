using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JapanseTuinen.Models
{
    public enum Condition
    {
        Tile,
        Bridge,
        Pagoda,
        YinYang,
        Tree,
        Flower,
        Butterfly,
        Gate,
        None
    }

    public static class Conditions
    {
        public static bool IsIconCondition(this Condition condition)
        {
            switch (condition)
            {
                case Condition.Tree:
                case Condition.Flower:
                case Condition.Gate:
                case Condition.Butterfly:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsSvgIcon(this Condition condition)
        {
            switch(condition)
            {
                case Condition.Tree:
                case Condition.Flower:
                case Condition.Gate:
                case Condition.Butterfly:
                case Condition.Pagoda:
                case Condition.YinYang:
                    return true;
                default:
                    return false;
            }
        }
    }

    public enum SolveState
    {
        Open,
        Solved,
        Invalid
    }
}