using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital.Meals.Core.Contracts
{
    public class Enums
    {
        public enum MealRequestAppprovalStatus
        {
            //explicitly setting the enum values so they don't get moved around by mistake in the future
            Pending = 0,
            Analyzing = 1,
            Rejected = 2,
            Accepted = 3
        }
    }
}
