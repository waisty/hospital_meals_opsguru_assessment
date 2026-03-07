using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital.Kitchen.Core.Contracts
{
    public class Enums
    {
        public enum TrayState
        {
            //the order of this enum must be maintained
            Pending = 0,
            PreparationStarted = 1,
            AccuracyValidated = 2,
            EnRoute = 3,
            Delivered = 4,
            Retrieved = 5
        }
    }
}
