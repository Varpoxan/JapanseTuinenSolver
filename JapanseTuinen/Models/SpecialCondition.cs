using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JapanseTuinen.Models
{
    public class SpecialCondition
    {
        public Condition Condition { get; set; }
        public int? Amount { get; set; }

        public SpecialCondition(Condition condition, int? amount = null)
        {
            this.Condition = condition;
            this.Amount = amount;
        }

        public SpecialCondition()
        {

        }
    }
}