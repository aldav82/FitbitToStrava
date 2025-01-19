using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects.Activities
{
    public enum ActivityType
    {
        FitbitAutodetect = 15000,
        Weight = 91042,
        Training = 91046,
        Walk = 90013,
        Run = 90009,
        Aerobics = 91047,
        Treadmill = 20049,
        IceSkate = 91052,
        Elliptic = 20047,
        Bicycle = 90001,
        Spinning= 55001,
        // Custom Activities Not Supported by Fitbit
        VirtualRide = 10020047
    }
}
