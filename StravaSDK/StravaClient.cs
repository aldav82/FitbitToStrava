using DataObjects.Activities;
using DataObjects.Tcx;
using DataObjects.Tools;
using Microsoft.Extensions.Logging;
using RestSharp;
using StravaSDK.Types;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace StravaSDK
{
    public class StravaClient : IStravaClient
    {
        private readonly string fitbitUrlActivities = "https://www.strava.com/api/v3/activities";
        private readonly string fitbitUrlUploads = "https://www.strava.com/api/v3/uploads";

        private readonly string authToken;
        private readonly ILogger<StravaClient> _logger;

        public StravaClient(string authToken, ILogger<StravaClient> logger)
        {
            this.authToken = authToken;
            _logger = logger;
        }

        public async Task UpdateActivity(long stravaActivityId, BaseActivity activityInfo)
        {
            var client = new RestClient($"{fitbitUrlActivities}/{stravaActivityId}");
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {this.authToken}");
            string url = fitbitUrlActivities;

            StravaUpdateData updateData = new StravaUpdateData()
            {
                sport_type = activityInfo.ActivityType.ConvertToStravaSportType().ToString(),
                gear_id = "none",
                description = "Recorded with Fitbit and migrated using my own personal FitbitToStrava Migration Service. " + activityInfo.Description,
                name = activityInfo.Name
            };
            request.AddBody(updateData);
            var response = await client.PutAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var error = response.Content;
                _logger.LogError($"Error updating activity. Status Code {response.StatusCode}. Message {error}");
            }

        }

        public async Task<StravaUploadData[]> CreateActivities(List<BaseActivity> activities)
        {
            List<StravaUploadData> resultList = new List<StravaUploadData>();
            foreach (var activity in activities)
            {
                var saved = await CreateActivityAsync(activity);
                resultList.Add(saved);
            }
            return resultList.ToArray();
        }

        public async Task<StravaUpload> RetrieveUploadStatus(long stravaUploadId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.authToken);

            // Send POST request to Strava Upload API
            var response = await client.GetAsync($"{this.fitbitUrlUploads}/{stravaUploadId}");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<StravaUpload>(content);

        }

        public async Task<StravaUploadData> CreateNonTcxActivityAsync(BaseActivity activity)
        {
            StravaUploadData data = new StravaUploadData()
            {
                ActivityId = activity.ActivityId
            };
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {this.authToken}");

            switch (activity.ActivityType)
            {
                case ActivityType.Weight:
                case ActivityType.Aerobics:
                    FillNonDistanceExcerciseInformation(request, activity);
                    break;
                case ActivityType.Walk:
                case ActivityType.Treadmill:
                case ActivityType.IceSkate:
                    FillDistanceExcerciseInformation(request, activity);
                    break;
            }

            var client = new RestClient(fitbitUrlActivities);

            var response = await client.ExecutePostAsync(request);

            if (response.IsSuccessful)
            {
                data.Success = true;
            }
            else
            {
                _logger.LogError($"Error creating activity ID {activity.ActivityId}, type: {activity.ActivityType}. Error code: {response.StatusCode}, Message: {response.ErrorMessage}");
                data.Success = false;
            }
            return data;
        }

        private async Task<StravaUploadData> CreateActivityAsync(BaseActivity activity)
        {
            switch (activity.ActivityType)
            {
                case ActivityType.Weight:
                case ActivityType.Aerobics:
                case ActivityType.Walk:
                case ActivityType.IceSkate:
                case ActivityType.Run:
                case ActivityType.Treadmill:
                    if (activity.ContainsTcx)
                    {

                        var tcx = activity.TcxContent.DeserializeXmlFromString<TrainingCenterDatabase_t>();

                        if (!tcx.HasValidTrackPoints())
                        {
                            return await CreateNonTcxActivityAsync(activity);
                        }
                        else
                        {
                            return await CreateTcxActivityAsync(activity);
                        }
                    }
                    else
                    {
                        return await CreateNonTcxActivityAsync(activity);
                    }
                default:
                    throw new NotSupportedException($"Activity type not supported {activity.ActivityType}");
            }
        }

        public async Task<StravaUploadData> CreateTcxActivityAsync(BaseActivity activity)
        {
            if (!activity.ContainsTcx)
            {
                throw new NotSupportedException("Trying to create a TCX activity without TCX information");
            }

            StravaUploadData data = new StravaUploadData();


            // Prepare multipart form-data content
            var content = new MultipartFormDataContent
            {
                { new StringContent("tcx"), "data_type" }, // Data type parameter
                { new StringContent(activity.Name), "name" }, // Data type parameter
                { new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(activity.TcxContent??"")), "file", $"{activity.ActivityId}.tcx" } // TCX file content
            };

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.authToken);

            // Send POST request to Strava Upload API
            var response = await client.PostAsync(this.fitbitUrlUploads, content);

            if (response.IsSuccessStatusCode)
            {
                // Read and return response content
                string contentStr = await response.Content.ReadAsStringAsync();
                var parsed = JsonDocument.Parse(contentStr);
                var root = parsed.RootElement;

                data.UploadId = root.GetProperty("id").GetInt64();
                data.ActivityId = activity.ActivityId;
                data.Status = root.GetProperty("status").GetString();
                data.Error = root.GetProperty("error").GetString();
                data.Success = true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error uploading TCX. Status Code {response.StatusCode}. Message {error}");
                data.Success = false;
            }
            return data;
        }

        private static void FillNonDistanceExcerciseInformation(RestRequest request, BaseActivity activity, ActivityType? activityType = null)
        {
            if (activityType == null)
            {
                activityType = activity.ActivityType;
            }
            request.AddParameter(StravaParamName.name.ToString(), activity.Name);
            request.AddParameter(StravaParamName.sport_type.ToString(), activityType.Value.ConvertToStravaSportType().ToString());
            request.AddParameter(StravaParamName.type.ToString(), activityType.Value.ConvertToStravaActivityType().ToString());
            request.AddParameter(StravaParamName.start_date_local.ToString(), activity.StartDate.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture));
            request.AddParameter(StravaParamName.elapsed_time.ToString(), $"{activity.Duration.TotalSeconds}");
            request.AddParameter(StravaParamName.description.ToString(), activity.Description);
        }
        private static void FillDistanceExcerciseInformation(RestRequest request, BaseActivity activity, ActivityType? activityType = null)
        {
            FillNonDistanceExcerciseInformation(request, activity, activityType);
            request.AddParameter(StravaParamName.distance.ToString(), activity.Distance);
        }
    }
}
