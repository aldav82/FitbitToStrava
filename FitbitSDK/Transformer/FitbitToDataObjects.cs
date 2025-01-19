using DataObjects.Activities;
using Fitbit.Api.Portable.Models;
using RestSharp;

namespace FitbitSDK.Transformer
{
    public static class FitbitToDataObjects
    {
        public static async Task<BaseActivity> RetrieveActivityAsync(Activities activity, string accessToken)
        {
            switch ((ActivityType)activity.ActivityTypeId)
            {
                case ActivityType.Walk:
                    return await FillTcxActivity<Walk>(activity, accessToken);
                case ActivityType.Run:
                    return await FillTcxActivity<Run>(activity, accessToken);
                case ActivityType.FitbitAutodetect:
                    return await FillTcxActivity<Walk>(activity, accessToken);

                case ActivityType.Weight:
                    return FillActivity<Weights>(activity);
                case ActivityType.Training:
                case ActivityType.Aerobics:
                    return FillActivity<Aerobics>(activity);
                case ActivityType.Treadmill:
                    return FillActivity<Treadmill>(activity);
                case ActivityType.Elliptic:
                    return FillActivity<Elliptic>(activity);
                case ActivityType.IceSkate:
                    return await FillTcxActivity<IceSkate>(activity, accessToken);
                case ActivityType.Bicycle:
                    return await FillTcxActivity<Bicycle>(activity, accessToken);
                default:
                    return FillActivity<Aerobics>(activity);

            }
        }

        private static BaseActivity FillActivity<T>(Activities activity) where T : BaseActivity, new()
        {
            T activityData = new T();
            activityData.Name = activity.ActivityName;
            activityData.StartDate = activity.OriginalStartTime.DateTime;
            activityData.Duration = TimeSpan.FromMilliseconds(activity.Duration);
            activityData.ActivityId = activity.LogId;
            activityData.Distance = activity.GetDistanceInMeters();
            return activityData;
        }

        public static float GetDistanceInMeters(this Activities activity)
        {
            if (activity.DistanceUnit == "Kilometer")
            {
                return (float)(activity.Distance * 1000);
            }
            return ((float)activity.Distance);
        }
       

        public static async Task<BaseActivity> FillTcxActivity<T>(Activities activity, string accessToken) where T : BaseActivity, new()
        {

            var walk = FillActivity<T>(activity);
            if (!string.IsNullOrEmpty(activity.TcxLink))
            {
                var client = new RestClient(activity.TcxLink);
                var request = new RestRequest();
                request.AddHeader("Authorization", $"Bearer {accessToken}");

                var response = await client.ExecuteGetAsync(request);
                if (response.IsSuccessful)
                {
                    var data = response.Content;
                    walk.TcxContent = data;
                }
            }
            return walk;
        }
        
    }

}
