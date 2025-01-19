using DataObjects.Activities;
using DataObjects.Tcx;
using StravaSDK.Types;

namespace StravaSDK
{
    public static class Extensions
    {
        public static bool HasValidTrackPoints(this TrainingCenterDatabase_t tcx)
        {
            if (tcx == null) return false;
            if (tcx.Activities == null) return false;
            if (tcx.Activities.Activity.Length == 0) return false;
            if (tcx.Activities.Activity.First().Lap == null) return false;
            if (tcx.Activities.Activity.First().Lap.Length == 0) return false;
            if (tcx.Activities.Activity.First().Lap.First().Track.Length == 0) return false;
            return true;
        }


        public static StravaActivityType ConvertToStravaActivityType(this ActivityType activityType)
        {
            switch (activityType)
            {
                case ActivityType.Weight:
                    return StravaActivityType.WeightTraining;
                case ActivityType.Training:
                    return StravaActivityType.Workout;
                case ActivityType.Walk:
                    return StravaActivityType.Walk;
                case ActivityType.Run:
                    return StravaActivityType.Run;
                case ActivityType.Aerobics:
                    return StravaActivityType.Workout;
                case ActivityType.Treadmill:
                    return StravaActivityType.Run;
                case ActivityType.IceSkate:
                    return StravaActivityType.IceSkate;
                case ActivityType.VirtualRide:
                case ActivityType.Spinning:
                    return StravaActivityType.VirtualRide;
                case ActivityType.Bicycle:
                    return StravaActivityType.Ride;
                default:
                    return StravaActivityType.Workout;
            }
        }
        public static StravaSportType ConvertToStravaSportType(this ActivityType activityType)
        {
            switch (activityType)
            {
                case ActivityType.Weight:
                    return StravaSportType.WeightTraining;
                case ActivityType.Training:
                    return StravaSportType.Workout;
                case ActivityType.Walk:
                    return StravaSportType.Walk;
                case ActivityType.Run:
                    return StravaSportType.Run;
                case ActivityType.Aerobics:
                    return StravaSportType.Workout;
                case ActivityType.Treadmill:
                    return StravaSportType.VirtualRun;
                case ActivityType.IceSkate:
                    return StravaSportType.IceSkate;
                case ActivityType.Bicycle:
                    return StravaSportType.Ride;
                case ActivityType.Elliptic:
                    return StravaSportType.Elliptical;
                case ActivityType.VirtualRide:
                case ActivityType.Spinning:
                    return StravaSportType.VirtualRide;
                default:
                    return StravaSportType.Workout;
            }
        }
    }
}
