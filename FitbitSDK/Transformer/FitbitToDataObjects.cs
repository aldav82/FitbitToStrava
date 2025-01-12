using DataObjects.Activities;
using Fitbit.Api.Portable.Models;
using Fitbit.Api.Portable.OAuth2;
using RestSharp;
using System.Xml.Serialization;

namespace FitbitSDK.Transformer
{
    public static class FitbitToDataObjects
    {
        public static async Task<BaseActivity> RetrieveActivityAsync(Activities activity, string accessToken)
        {
            switch (activity.ActivityTypeId)
            {
                case 90013:
                    //Walk
                    return await FillTcxActivity<Walk>(activity, accessToken);
                case 90009:
                    //Run
                    return await FillTcxActivity<Run>(activity, accessToken);
                case 15000:
                    //Fitbit Autodetect
                    return await FillTcxActivity<Walk>(activity, accessToken);

                case 91042:
                    // Weights
                    return FillActivity<Weights>(activity);
                case 91046:
                // Core Training
                case 91047:
                    // Aerobics
                    return FillActivity<Aerobics>(activity);
                case 20049:
                    // Treadmill
                    return FillActivity<Treadmill>(activity);
                case 91052:
                    // IceSkate
                    return await FillTcxActivity<IceSkate>(activity, accessToken);
                default:
                    throw new NotSupportedException($"Activity type not supported {activity.ActivityTypeId}");

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
