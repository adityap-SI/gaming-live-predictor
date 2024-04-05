using System;
using System.Collections.Generic;
using System.Text;

namespace ICC.Predictor.Contracts.Enums
{
    class Enums
    {
    }

    public enum QuestionStatus
    {
        All = -2,
        Unpublished = 0,
        Published = 1,
        Locked = 2,
        Resolved = 3,
        Delete = -1,
        Notification = -3,
        Points_Calculation = -4
    }
}
